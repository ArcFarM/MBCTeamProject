using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace JiHoon
{
    public class WaveController : MonoBehaviour
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
            // 처음엔 웨이브 전이니 설치 허용
            placementMgr.placementEnabled = true;

            // 1) 게임 시작 시 카드 초기 지급
            cardManager.AddRandomCards(initialCardCount);

            // 2) 버튼 클릭 시 웨이브 시작
            startWaveButton.onClick.AddListener(OnStartClicked);
        }

        void OnStartClicked()
        {
            // 웨이브 시작 시 설치&재배치 모두 비허용
            placementMgr.placementEnabled = false;

            // 버튼 잠금
            startWaveButton.interactable = false;

            // 자동 재시작 중이면 취소
            if (_autoRoutine != null)
            {
                StopCoroutine(_autoRoutine);
                _autoRoutine = null;
            }

            // 현재 웨이브 에셋 가져오기
            var config = waveConfig[_currentWaveIndex];
            StartCoroutine(RunWave(config));
        }

        private IEnumerator RunWave(WaveConfig config)
        {
            // 웨이브 끝나고 대기 상태로 돌아올 때
            yield return new WaitUntil(() =>
                Object.FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None).Length == 0
            );

            

            // 1) 모든 EnemyInfo 에 대해 SpawnRoutine 코루틴을 동시에 시작
            var done = new bool[config.enemies.Count];
            for (int i = 0; i < config.enemies.Count; i++)
            {
                int idx = i;
                StartCoroutine(Wrapper());

                IEnumerator Wrapper()
                {
                    yield return SpawnRoutine(config.enemies[idx]);
                    done[idx] = true;      // i번 루틴이 끝났다고 표시
                }
            }

            // 2) 모든 SpawnRoutine이 끝날 때까지 대기
            yield return new WaitUntil(() => done.All(x => x));

            // 3) (보스 웨이브 시) 보스 스폰
            if (config.isBossWave && config.bossPrefab != null)
                Instantiate(config.bossPrefab, bossSpawnPoint.position, Quaternion.identity);

            // 4) 맵 위 모든 적이 사라질 때까지 대기
            yield return new WaitUntil(() =>
                Object.FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None).Length == 0
            );

            // 5) 웨이브 종료 처리
            cardManager.AddRandomCards(cardsPerWave);
            startWaveButton.interactable = true;
            _autoRoutine = StartCoroutine(AutoStartNext());
            _currentWaveIndex = (_currentWaveIndex + 1) % waveConfig.Count;

            // **설치/재배치 다시 허용**
            placementMgr.placementEnabled = true;
        }

        private IEnumerator SpawnRoutine(WaveEnemyInfo enemyInfo)
        {
            int spawned = 0;
            while (spawned < enemyInfo.count)
            {
                var manager = spawners[enemyInfo.spawnerIndex];
                manager.SpawnPrefabAt(0, enemyInfo.prefab);
                spawned++;
                yield return new WaitForSeconds(enemyInfo.spawnInterval);
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