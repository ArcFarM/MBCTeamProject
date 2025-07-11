using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MainGame.Manager;
using TMPro;  // ★ 추가 ★

namespace JiHoon
{
    public class WaveController : SingletonManager<WaveController>
    {
        [Header("적 스포너")]
        public EnemySpawnerManager spawner;

        [Header("컨테이너")]
        public Transform enemyContainer;

        [Header("UI")]
        public Button startWaveButton;
        public UnitCardManager cardManager;
        public UnitPlacementManager placementManager;

        [Header("Wave 표시 UI")]  // ★ 추가 ★
        public TextMeshProUGUI waveDisplayText;  // Wave / 01 텍스트

        [Header("카드 설정")]
        public int initialCardCount = 5;
        public int cardsPerWave = 3;

        [Header("웨이브 설정")]
        public List<WaveConfig> waveConfigs;

        // ★ 추가: 외부에서 접근 가능한 프로퍼티 ★
        public int CurrentWaveNumber => currentWaveIndex + 1;  // 1부터 시작하는 웨이브 번호
        public int TotalWaves => waveConfigs.Count;
        public bool IsWaveRunning => isWaveRunning;

        // ★ 추가: 웨이브 변경 이벤트 ★
        public static System.Action<int> OnWaveChanged;
        public static System.Action<int> OnWaveStarted;
        public static System.Action<int> OnWaveCompleted;

        private int currentWaveIndex = 0;
        private bool isWaveRunning = false;

        // 각 스폰 포인트별로 선택된 경로를 저장
        private Dictionary<int, Transform[]> selectedPathsBySpawnPoint = new Dictionary<int, Transform[]>();

        void Start()
        {
            placementManager.placementEnabled = true;
            cardManager.AddRandomCards(initialCardCount);
            startWaveButton.onClick.AddListener(StartWave);

            // ★ 추가: 게임 시작 시 초기 웨이브 표시 ★
            OnWaveChanged?.Invoke(CurrentWaveNumber);
            UpdateWaveDisplay();  // ★ 직접 UI 업데이트 ★
        }

        void StartWave()
        {
            if (isWaveRunning) return;

            // ★ 추가: 웨이브 시작 알림 ★
            OnWaveStarted?.Invoke(CurrentWaveNumber);

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
            groupObj.transform.SetParent(enemyContainer);
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
                var enemy = Instantiate(enemyPrefab, position, Quaternion.identity, enemyContainer);

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

            // ★ 추가: 웨이브 완료 및 변경 이벤트 발생 ★
            OnWaveCompleted?.Invoke(currentWaveIndex);  // 완료된 웨이브 인덱스
            OnWaveChanged?.Invoke(CurrentWaveNumber);    // 다음 웨이브 번호 (1부터 시작)
            UpdateWaveDisplay();  // ★ 직접 UI 업데이트 ★

            // UI 복구
            startWaveButton.interactable = true;
        }

        // ★ 추가: 수동으로 웨이브 정보를 가져올 수 있는 메서드 ★
        public string GetWaveDisplayText()
        {
            return $"Wave {CurrentWaveNumber} / {TotalWaves:D2}";
        }

        // ★ 새로 추가: Wave 표시 업데이트 메서드 ★
        void UpdateWaveDisplay()
        {
            if (waveDisplayText != null)
            {
                // Wave / 01, Wave / 02 형식으로 표시
                waveDisplayText.text = $"Wave / {CurrentWaveNumber:D2}";
            }
        }
    }
}