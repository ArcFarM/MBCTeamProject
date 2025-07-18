using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MainGame.Manager;
using MainGame.Card;
using MainGame.UI;
using TMPro;

namespace JiHoon {
    public class WaveController : SingletonManager<WaveController> {
        [Header("적 스포너")]
        public EnemySpawnerManager spawner;

        [Header("컨테이너")]
        public Transform enemyContainer;

        [Header("UI")]
        public Button startWaveButton;
        public UnitCardManager cardManager;
        public UnitPlacementManager placementManager;

        [Header("Wave 표시 UI")]
        public TextMeshProUGUI waveDisplayText;

        [Header("카드 설정")]
        public int initialCardCount = 5;
        public int cardsPerWave = 3;

        [Header("카드 선택 시스템")]
        public GameObject cardSelectPanel;
        public CardPool cardPool;

        [Header("웨이브 설정")]
        public List<WaveConfig> waveConfigs;

        // 외부에서 접근 가능한 프로퍼티
        public int CurrentWaveNumber => currentWaveIndex + 1;
        public int TotalWaves => waveConfigs.Count;
        public bool IsWaveRunning => isWaveRunning;

        // 웨이브 변경 이벤트
        public static System.Action<int> OnWaveChanged;
        public static System.Action<int> OnWaveStarted;
        public static System.Action<int> OnWaveCompleted;

        private int currentWaveIndex = 0;
        private bool isWaveRunning = false;
        public int enemyCount = 0; // 남아 있는 적의 수

        // 각 스폰 포인트별로 선택된 경로를 저장
        private Dictionary<int, Transform[]> selectedPathsBySpawnPoint = new Dictionary<int, Transform[]>();

        void Start() {
            if (cardSelectPanel != null)
                cardSelectPanel.SetActive(false);

            placementManager.placementEnabled = true;
            cardManager.AddRandomCards(initialCardCount);
            startWaveButton.onClick.AddListener(StartWave);

            OnWaveChanged?.Invoke(CurrentWaveNumber);
            UpdateWaveDisplay();
        }

        void StartWave() {
            if (isWaveRunning) return;

            OnWaveStarted?.Invoke(CurrentWaveNumber);
            startWaveButton.interactable = false;
            placementManager.placementEnabled = false; // 웨이브 시작 시 배치 비활성화
            StartCoroutine(RunWave());
        }

        IEnumerator RunWave() {
            isWaveRunning = true;
            if (currentWaveIndex >= waveConfigs.Count) {
                Debug.LogWarning("모든 웨이브가 종료되었습니다.");
                // TODO: 게임 승리 처리
                yield break;
            }

            var config = waveConfigs[currentWaveIndex];
            selectedPathsBySpawnPoint.Clear();

            foreach (var group in config.enemyGroups) {
                yield return StartCoroutine(SpawnGroup(group));
                yield return new WaitForSeconds(group.delayAfterGroup);
            }

            // 모든 적이 처치될 때까지 대기
            while (enemyCount > 0) {
                yield return null;
            }

            // 웨이브 완료 처리
            OnWaveComplete();
        }

        IEnumerator SpawnGroup(EnemyGroupConfig group) {
            int spawnIndex = (int)group.spawnPosition;
            var spawnData = spawner.GetSpawnData(spawnIndex);
            if (spawnData != null) {
                yield return StartCoroutine(SpawnGroupAtPoint(group, spawnData, spawnIndex));
            }
            else {
                Debug.LogError($"SpawnPointData를 찾을 수 없습니다: index {spawnIndex}");
            }
        }

        IEnumerator SpawnGroupAtPoint(EnemyGroupConfig group, SpawnPointData spawnData, int spawnIndex) {
            if (spawnData.spawnPoint == null) yield break;

            var basePosition = spawnData.spawnPoint.position;
            var groupObj = new GameObject($"EnemyGroup_{group.groupName}_{spawnData.name}");
            groupObj.transform.SetParent(enemyContainer, false);

            var enemyGroup = groupObj.AddComponent<EnemyGroup>();

            if (!selectedPathsBySpawnPoint.ContainsKey(spawnIndex)) {
                selectedPathsBySpawnPoint[spawnIndex] = spawnData.GetRandomPath();
            }
            Transform[] pathToUse = selectedPathsBySpawnPoint[spawnIndex];

            for (int i = 0; i < group.enemyCount; i++) {
                var enemyPrefab = group.enemyPrefabs[i % group.enemyPrefabs.Count];

                float spacing = group.enemySpacing;
                int row = i / 5;
                int col = i % 5;
                Vector3 formationOffset = new Vector3(col * spacing, -row * spacing, 0);

                var spawnPosition = basePosition + formationOffset;
                var enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, groupObj.transform);

                enemyCount++;

                var movement = enemy.GetComponent<EnemyMovement>();
                if (movement != null) {
                    movement.SetPath(pathToUse);
                    movement.SetGroup(enemyGroup);
                }
                enemyGroup.AddMember(movement);

                // EnemySpawnerManager에 설정된 간격만큼 대기
                if (spawner.spawnInterval > 0) {
                    yield return new WaitForSeconds(spawner.spawnInterval);
                }
            }
        }

        void OnWaveComplete() {
            isWaveRunning = false;
            OnWaveCompleted?.Invoke(CurrentWaveNumber);
            CheckPenalty();
            StartCoroutine(CardSelect());
        }

        IEnumerator CardSelect() {
            if (cardSelectPanel == null || cardPool == null) {
                Debug.LogError("카드 선택에 필요한 UI 또는 CardPool 참조가 설정되지 않았습니다!");
                PrepareNextWave();
                yield break;
            }

            cardSelectPanel.SetActive(true);

            List<PolicyCard_new> spawnedCards = new List<PolicyCard_new>();
            for (int i = 0; i < cardsPerWave; i++) {
                PolicyCard_new newCard = cardPool.GetCard();
                if (newCard != null) {
                    newCard.transform.SetParent(cardSelectPanel.transform, false);
                    spawnedCards.Add(newCard);
                }
            }

            PolicyCard_new selectedCard = null;
            while (selectedCard == null) {
                foreach (PolicyCard_new card in spawnedCards) {
                    if (card != null && !card.gameObject.activeSelf) {
                        selectedCard = card;
                        break;
                    }
                }
                yield return null;
            }

            foreach (var unselectedCard in spawnedCards) {
                if (unselectedCard != null && unselectedCard.gameObject.activeSelf) {
                    Destroy(unselectedCard.gameObject);
                }
            }

            cardSelectPanel.SetActive(false);
            CardPool.ClearCardNames(); // 카드 선택 후 중복 방지용 해시셋 초기화
            PrepareNextWave();
        }

        void PrepareNextWave() {
            currentWaveIndex++;
            OnWaveChanged?.Invoke(CurrentWaveNumber);
            UpdateWaveDisplay();

            startWaveButton.interactable = true;
            placementManager.placementEnabled = true;
        }

        void UpdateWaveDisplay() {
            if (waveDisplayText != null) {
                waveDisplayText.text = $"Wave / {CurrentWaveNumber:D2}";
            }
        }

        public void OnEnemyDeath() {
            if (enemyCount > 0) {
                enemyCount--;
            }
            Debug.Log($"적 처리! 남은 적: {enemyCount}");
        }

        void CheckPenalty() {
            Debug.Log("유닛 페널티 정산");
            StatManager.Instance.CalcPenalty();
        }
    }
}