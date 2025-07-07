using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MainGame.Manager;

namespace JiHoon
{
    public class WaveController : SingletonManager<WaveController>
    {
        [Header("적 스포너들 (EnemySpawnerManager)")]
        public List<EnemySpawnerManager> spawners;

        [Header("보스 스폰 위치")]
        public Transform bossSpawnPoint;

        [Header("카드 매니저")]
        public UnitCardManager cardManager;

        [Header("웨이브 시작 버튼 UI")]
        public Button startWaveButton;

        [Header("첫 시작 시 지급할 카드 수")]
        public int initialCardCount = 5;

        [Header("웨이브당 카드 수")]
        public int cardsPerWave = 3;

        [Header("자동 재시작 대기 시간")]
        public float autoDelay = 50f;

        [Header("웨이브 설정 에셋 목록")]
        public List<WaveConfig> waveConfig;

        public UnitPlacementManager placementMgr;

        private int _currentWaveIndex = 0;
        private Coroutine _autoRoutine;

        void Start()
        {
            placementMgr.placementEnabled = true;
            cardManager.AddRandomCards(initialCardCount);
            startWaveButton.onClick.AddListener(OnStartClicked);
        }

        void OnStartClicked()
        {
            placementMgr.placementEnabled = false;
            startWaveButton.interactable = false;

            if (_autoRoutine != null)
            {
                StopCoroutine(_autoRoutine);
                _autoRoutine = null;
            }

            var config = waveConfig[_currentWaveIndex];
            StartCoroutine(RunWave(config));
        }

        private IEnumerator RunWave(WaveConfig config)
        {
            // 1) 이전 웨이브 끝날 때까지 대기
            yield return new WaitUntil(() =>
                Object.FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None).Length == 0);

            // 2) 그룹/기존 방식 분기
            if (config.useGroupSystem)
                yield return RunGroupWave(config);
            else
                yield return RunEnemyWave(config);

            // 3) 보스 스폰 (보스 웨이브 시)
            if (config.isBossWave && config.bossPrefab != null)
                Instantiate(config.bossPrefab, bossSpawnPoint.position, Quaternion.identity);

            // 4) 모든 적이 사라질 때까지 대기
            yield return new WaitUntil(() =>
                Object.FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None).Length == 0);

            // 5) 웨이브 종료 처리
            cardManager.AddRandomCards(cardsPerWave);
            startWaveButton.interactable = true;
            _autoRoutine = StartCoroutine(AutoStartNext());
            _currentWaveIndex = (_currentWaveIndex + 1) % waveConfig.Count;
            placementMgr.placementEnabled = true;
        }

        // 기존 enemies 리스트로 개별 파티 스폰
        private IEnumerator RunEnemyWave(WaveConfig config)
        {
            var done = new bool[config.enemies.Count];
            for (int i = 0; i < config.enemies.Count; i++)
            {
                int idx = i;
                StartCoroutine(Wrapper());
                IEnumerator Wrapper()
                {
                    yield return SpawnRoutine(config.enemies[idx]);
                    done[idx] = true;
                }
            }
            yield return new WaitUntil(() => done.All(x => x));
        }

        // 새로운 그룹 방식: 각 GroupConfig 별로 호출
        private IEnumerator RunGroupWave(WaveConfig config)
        {
            var routines = new List<Coroutine>();
            foreach (var grp in config.groups)
                routines.Add(StartCoroutine(SpawnGroupRoutine(grp)));

            foreach (var c in routines)
                yield return c;
        }

        // 그룹 하나를 스폰하는 코루틴
        private IEnumerator SpawnGroupRoutine(GroupConfig grp)
        {
            if (grp.enemyTypes.Count == 0)
                yield break;

            // 1) 스포너 & 시작점
            var first = grp.enemyTypes[0];
            var manager = spawners[first.spawnerIndex];
            var spawnInfo = manager.spawnPoints[0];
            Vector3 origin = spawnInfo.spawnPoint.position;

            // 2) viaPoints 배열 준비 (파티 분기)
            int branchIdx = Random.value < 0.5f ? 0 : 1;
            var viaRoot = spawnInfo.viaPoints[branchIdx];
            var viaArray = viaRoot
              .GetComponentsInChildren<Transform>()
              .Where(t => t != viaRoot)
              .ToArray();

            // 3) 그룹 루트 & 포메이션 세팅
            var root = new GameObject($"Group_{grp.groupId}");
            root.transform.position = origin;
            var eg = root.AddComponent<EnemyGroup>();
            eg.SetFormation(grp.formation, grp.formationSpacing);

            // 4) 멤버 하나씩 스폰 → AddMember 호출로 자동 배치
            for (int i = 0; i < grp.maxMembersPerGroup; i++)
            {
                var typeInfo = grp.enemyTypes[i % grp.enemyTypes.Count];

                // 스폰은 그룹 루트 위치에서
                var go = Instantiate(
                    typeInfo.prefab,
                    origin,
                    Quaternion.identity
                );
                var mv = go.GetComponent<EnemyMovement>();
                mv.startPoint = spawnInfo.spawnPoint;
                mv.viaPoints = viaArray;
                mv.endPoint = manager.endPoint;

                // AddMember 안에서
                //  - mv.SetGroup(this)
                //  - 처음 멤버면 리더, 나머진 포로워
                //  - UpdateFormation() 호출 → 부모(root) 안에서 자동 배치
                eg.AddMember(mv);

                yield return new WaitForSeconds(typeInfo.spawnInterval);
            }

            // 5) 한 프레임 쉬고 이동 시작
            yield return null;
        }

        // 하위 호환용 SpawnRoutine
        private IEnumerator SpawnRoutine(WaveEnemyInfo info)
        {
            var manager = spawners[info.spawnerIndex];
            var spawnInfo = manager.spawnPoints[0];
            var origin = spawnInfo.spawnPoint.position;
            var right = spawnInfo.spawnPoint.right;

            int branchIdx = Random.value < 0.5f ? 0 : 1;
            var branchGrp = spawnInfo.viaPoints[branchIdx];
            var partyVia = branchGrp
              .GetComponentsInChildren<Transform>()
              .Where(t => t != branchGrp)
              .ToArray();

            int spawned = 0;
            while (spawned < info.count)
            {
                int batch = Mathf.Min(info.groupCount, info.count - spawned);
                var groupObj = new GameObject($"EnemyGroup_{spawned}");
                var eg = groupObj.AddComponent<EnemyGroup>();
                eg.SetFormation(EnemyGroup.FormationType.Line, info.groupSpacing);

                for (int i = 0; i < batch; i++)
                {
                    float offsetX = (i - (batch - 1) * 0.5f) * info.groupSpacing;
                    Vector3 spawnPos = origin + right * offsetX;

                    var go = Instantiate(info.prefab, spawnPos, Quaternion.identity, manager.spawnContainer);
                    var mv = go.GetComponent<EnemyMovement>();
                    mv.startPoint = spawnInfo.spawnPoint;
                    mv.viaPoints = partyVia;
                    mv.endPoint = manager.endPoint;

                    eg.AddMember(mv);
                }

                spawned += batch;
                yield return new WaitForSeconds(info.groupInterval);
            }
        }

        private IEnumerator AutoStartNext()
        {
            yield return new WaitForSeconds(autoDelay);
            _autoRoutine = null;
            OnStartClicked();
        }
    }
}
