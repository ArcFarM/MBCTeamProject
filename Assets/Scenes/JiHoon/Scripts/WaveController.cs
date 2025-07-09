using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MainGame.Manager;

namespace JiHoon
{
    public class WaveController : SingletonManager<WaveController>
    {
        [Header("적 스포너")]
        public EnemySpawnerManager spawner;

        [Header("UI")]
        public Button startWaveButton;
        public UnitCardManager cardManager;
        public UnitPlacementManager placementManager;

        [Header("카드 설정")]
        public int initialCardCount = 5;
        public int cardsPerWave = 3;

        [Header("웨이브 설정")]
        public List<WaveConfig> waveConfigs;

        private int currentWaveIndex = 0;
        private bool isWaveRunning = false;

        // 각 스폰 포인트별로 선택된 경로를 저장
        private Dictionary<int, Transform[]> selectedPathsBySpawnPoint = new Dictionary<int, Transform[]>();

        void Start()
        {
            placementManager.placementEnabled = true;
            cardManager.AddRandomCards(initialCardCount);
            startWaveButton.onClick.AddListener(StartWave);
        }

        void StartWave()
        {
            if (isWaveRunning) return;

            startWaveButton.interactable = false;
            StartCoroutine(RunWave());
        }

        IEnumerator RunWave()
        {
            isWaveRunning = true;
            var config = waveConfigs[currentWaveIndex];

            // 웨이브 시작 시 경로 초기화
            selectedPathsBySpawnPoint.Clear();

            // 그룹별로 스폰
            foreach (var group in config.enemyGroups)
            {
                SpawnGroup(group);
                yield return new WaitForSeconds(group.delayAfterGroup);
            }

            // 모든 적이 처치될 때까지 대기
            yield return new WaitUntil(() =>
                GameObject.FindObjectsOfType<EnemyMovement>().Length == 0);

            // 웨이브 완료 처리
            OnWaveComplete();
        }

        void SpawnGroup(EnemyGroupConfig group)
        {
            // SpawnPosition enum을 인덱스로 변환 (Top=0, Middle=1, Bottom=2)
            int spawnIndex = (int)group.spawnPosition;
            var spawnData = spawner.GetSpawnData(spawnIndex);
            SpawnGroupAtPoint(group, spawnData);
        }

        void SpawnGroupAtPoint(EnemyGroupConfig group, SpawnPointData spawnData)
        {
            if (spawnData == null || spawnData.spawnPoint == null) return;

            var basePosition = spawnData.spawnPoint.position;

            // 그룹 오브젝트 생성
            var groupObj = new GameObject($"EnemyGroup_{group.groupName}_{spawnData.name}");
            var enemyGroup = groupObj.AddComponent<EnemyGroup>();

            // 두 줄로 배치 (한 줄에 최대 5마리)
            var positions = new List<Vector3>();
            float spacing = group.enemySpacing;
            int maxPerRow = 5; // 한 줄에 최대 개수

            for (int i = 0; i < group.enemyCount; i++)
            {
                int row = i / maxPerRow; // 현재 줄 번호
                int col = i % maxPerRow; // 현재 줄에서의 위치

                // 각 줄의 중앙 정렬
                int itemsInThisRow = Mathf.Min(maxPerRow, group.enemyCount - row * maxPerRow);
                float xOffset = (col - (itemsInThisRow - 1) / 2f) * spacing;
                float yOffset = -row * spacing; // 두 번째 줄은 아래로

                positions.Add(new Vector3(xOffset, yOffset, 0));
            }

            // 적 스폰
            for (int i = 0; i < group.enemyCount; i++)
            {
                var enemyPrefab = group.enemyPrefabs[i % group.enemyPrefabs.Count];
                var position = basePosition + positions[i];

                // 2D 게임이므로 회전 없이 생성
                var enemy = Instantiate(enemyPrefab, position, Quaternion.identity);

                // 2D 스프라이트가 제대로 보이도록 설정
                var spriteRenderer = enemy.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingOrder = 10;
                }

                var movement = enemy.GetComponent<EnemyMovement>();

                if (movement != null)
                {
                    // 첫 번째를 리더로 설정
                    if (i == 0)
                    {
                        movement.SetAsLeader();
                        enemyGroup.SetLeader(movement);
                    }
                    else
                    {
                        movement.SetAsFollower(positions[i]);
                    }

                    // 해당 스폰 포인트의 경로 선택 또는 재사용
                    int spawnIndex = (int)group.spawnPosition;
                    Transform[] pathToUse;

                    if (selectedPathsBySpawnPoint.ContainsKey(spawnIndex))
                    {
                        // 이미 선택된 경로가 있으면 그것을 사용
                        pathToUse = selectedPathsBySpawnPoint[spawnIndex];
                    }
                    else
                    {
                        // 처음이면 랜덤으로 선택하고 저장
                        pathToUse = spawnData.GetRandomPath();
                        selectedPathsBySpawnPoint[spawnIndex] = pathToUse;
                    }

                    movement.SetPath(pathToUse);
                    enemyGroup.AddMember(movement);
                }
            }
        }

        void OnWaveComplete()
        {
            isWaveRunning = false;

            // 보상 지급
            cardManager.AddRandomCards(cardsPerWave);

            // 다음 웨이브로
            currentWaveIndex = (currentWaveIndex + 1) % waveConfigs.Count;

            // UI 복구
            startWaveButton.interactable = true;
        }
    }
}