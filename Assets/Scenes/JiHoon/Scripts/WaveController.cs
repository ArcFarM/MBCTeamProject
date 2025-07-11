using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MainGame.Manager;
using MainGame.Card;
using MainGame.UI;
using TMPro;

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

        [Header("Wave 표시 UI")]
        public TextMeshProUGUI waveDisplayText;  // Wave / 01 텍스트

        [Header("카드 설정")]
        public int initialCardCount = 5;
        public int cardsPerWave = 3;

        [Header("카드 선택 시스템")]  
        public GameObject cardSelectPanel;   // 카드 선택 패널
        public CardPool cardPool;            // 카드 풀

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

        // 각 스폰 포인트별로 선택된 경로를 저장
        private Dictionary<int, Transform[]> selectedPathsBySpawnPoint = new Dictionary<int, Transform[]>();

        // ★ WaveControllerClone에서 추가 ★
        public int enemyCount = 0;  // 남아 있는 적의 수

        void Start()
        {
            // ★ 카드 선택 패널 초기화 ★
            if (cardSelectPanel != null)
                cardSelectPanel.SetActive(false);

            placementManager.placementEnabled = true;
            cardManager.AddRandomCards(initialCardCount);
            startWaveButton.onClick.AddListener(StartWave);

            // 게임 시작 시 초기 웨이브 표시
            OnWaveChanged?.Invoke(CurrentWaveNumber);
            UpdateWaveDisplay();
        }

        void StartWave()
        {
            if (isWaveRunning) return;

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

            // ★ enemyCount 사용 방식으로 변경 (WaveControllerClone) ★
            while (enemyCount > 0)
            {
                yield return null;
            }

            // 웨이브 완료 처리
            OnWaveComplete();
        }

        void SpawnGroup(EnemyGroupConfig group)
        {
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
            int maxPerRow = 5;

            for (int i = 0; i < group.enemyCount; i++)
            {
                int row = i / maxPerRow;
                int col = i % maxPerRow;

                int itemsInThisRow = Mathf.Min(maxPerRow, group.enemyCount - row * maxPerRow);
                float xOffset = (col - (itemsInThisRow - 1) / 2f) * spacing;
                float yOffset = -row * spacing;

                positions.Add(new Vector3(xOffset, yOffset, 0));
            }

            // 적 스폰
            for (int i = 0; i < group.enemyCount; i++)
            {
                var enemyPrefab = group.enemyPrefabs[i % group.enemyPrefabs.Count];
                var position = basePosition + positions[i];

                var enemy = Instantiate(enemyPrefab, position, Quaternion.identity, enemyContainer);

                // ★ enemyCount 증가 (적이 생성될 때마다) ★
                enemyCount++;

                var spriteRenderer = enemy.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingOrder = 10;
                }

                var movement = enemy.GetComponent<EnemyMovement>();

                if (movement != null)
                {
                    if (i == 0)
                    {
                        movement.SetAsLeader();
                        enemyGroup.SetLeader(movement);
                    }
                    else
                    {
                        movement.SetAsFollower(positions[i]);
                    }

                    int spawnIndex = (int)group.spawnPosition;
                    Transform[] pathToUse;

                    if (selectedPathsBySpawnPoint.ContainsKey(spawnIndex))
                    {
                        pathToUse = selectedPathsBySpawnPoint[spawnIndex];
                    }
                    else
                    {
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

            // ★ 카드 선택 시스템 사용 (WaveControllerClone) ★
            StartCoroutine(CardSelect());
        }

        // ★ WaveControllerClone의 카드 선택 코루틴 ★
        IEnumerator CardSelect()
        {
            // 카드 선택 패널을 활성화
            if (cardSelectPanel == null || cardPool == null)
            {
                Debug.LogError("카드 선택에 필요한 UI 또는 CardPool 참조가 설정되지 않았습니다!");
                PrepareNextWave();
                yield break;
            }

            cardSelectPanel.SetActive(true);

            // 설정된 수만큼 카드를 뽑아서 패널에 등록
            List<PolicyCard_new> spawnedCards = new();
            for (int i = 0; i < cardsPerWave; i++)
            {
                PolicyCard_new newCard = cardPool.GetCard();
                if (newCard != null)
                {
                    newCard.transform.SetParent(cardSelectPanel.transform, false);
                    spawnedCards.Add(newCard);
                }
            }

            // 플레이어가 카드를 선택할 때까지 대기
            PolicyCard_new selectedCard = null;
            while (selectedCard == null)
            {
                foreach (PolicyCard_new card in spawnedCards)
                {
                    if (!card.gameObject.activeSelf)
                    {
                        selectedCard = card;
                    }
                }
                yield return null;
            }

            // 선택되지 않은 나머지 카드들을 파괴
            foreach (var unselectedCard in spawnedCards)
            {
                if (unselectedCard != null)
                {
                    Destroy(unselectedCard.gameObject);
                }
            }

            // 패널을 다시 숨김
            cardSelectPanel.SetActive(false);

            // 모든 정리가 끝난 후 다음 웨이브를 준비
            PrepareNextWave();
        }

        void PrepareNextWave()
        {
            // 다음 웨이브로
            currentWaveIndex = (currentWaveIndex + 1) % waveConfigs.Count;

            // 웨이브 변경 이벤트 발생
            OnWaveCompleted?.Invoke(currentWaveIndex);
            OnWaveChanged?.Invoke(CurrentWaveNumber);
            UpdateWaveDisplay();  // Wave 표시 업데이트

            // UI 복구
            startWaveButton.interactable = true;
        }

        // Wave 표시 업데이트 메서드
        void UpdateWaveDisplay()
        {
            if (waveDisplayText != null)
            {
                waveDisplayText.text = $"Wave / {CurrentWaveNumber:D2}";
            }
        }

        public string GetWaveDisplayText()
        {
            return $"Wave {CurrentWaveNumber} / {TotalWaves:D2}";
        }

        // ★ 적이 죽었을 때 호출되어야 하는 메서드 ★
        public void OnEnemyDeath()
        {
            enemyCount--;
        }
    }
}