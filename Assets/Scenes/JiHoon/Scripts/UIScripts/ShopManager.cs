using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MainGame.Manager;

using MainGame.Enum;
using static UnityEngine.GraphicsBuffer;
using UnityEditor;

namespace JiHoon
{
    public class ShopManager : MonoBehaviour
    {
        [Header("카드 덱 매니저")]
        public UnitCardManager cardManager;     // 카드 관리자 참조

        [Header("UI 패널")]
        public GameObject shopPanel;            // 상점 UI 패널

        [Header("데이터")]
        public List<ItemData> allItems;        // 상점에 표시할 모든 아이템
        public Transform gridParent;           // 아이템 버튼들의 부모 오브젝트
        public GameObject itemButtonPrefab;    // 아이템 버튼 프리팹

        [Header("확률 시스템")]
        public ShopProbabilityTable probabilityTable;  // 확률표 ScriptableObject
        public bool useProbabilitySystem = true;       // 확률 시스템 사용 여부

        [Header("우측 UI")]
        public Button buyButton;               // 구매 버튼

        [Header("추가 UI")]
        public TextMeshProUGUI effectText;     // 아이템 설명 텍스트
        public Image npcIllustration;          // NPC 일러스트

        private ItemData selectedItem;         // 현재 선택된 아이템
        private List<ItemData> currentShopItems = new List<ItemData>(); // 현재 상점에 표시된 아이템들

        private void Start()
        {
            // StatManager 초기화를 기다린 후 상점 초기화
            StartCoroutine(DelayedInit());

            // Wave 변경 이벤트 구독 (확률 시스템 사용 시)
            if (useProbabilitySystem && WaveController.Instance != null)
            {
                WaveController.OnWaveChanged += OnWaveChanged;
            }
        }

        void OnDestroy()
        {
            // 이벤트 구독 해제
            if (useProbabilitySystem && WaveController.Instance != null)
            {
                WaveController.OnWaveChanged -= OnWaveChanged;
            }
        }

        private IEnumerator DelayedInit()
        {
            // StatManager가 초기화될 때까지 대기
            yield return new WaitForSeconds(0.5f);

            // 초기 상점 아이템 설정
            RefreshShopItems();

            ClearDetail();
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyButton);
        }

        // Wave가 변경될 때 호출
        private void OnWaveChanged(int newWaveNumber)
        {
            if (useProbabilitySystem)
            {
                RefreshShopItems();
            }
        }

        // 상점 아이템 새로고침
        private void RefreshShopItems()
        {
            // 기존 아이템 버튼들 제거
            foreach (Transform child in gridParent)
            {
                Destroy(child.gameObject);
            }

            // 현재 표시할 아이템 결정
            if (useProbabilitySystem && probabilityTable != null && WaveController.Instance != null)
            {
                // 확률 시스템 사용
                int currentWave = WaveController.Instance.CurrentWaveNumber;
                currentShopItems = probabilityTable.GetItemsForWave(currentWave);
                Debug.Log($"Wave {currentWave}: 확률표에 따라 {currentShopItems.Count}개의 아이템이 선택되었습니다.");
            }
            else
            {
                // 기존 방식 (모든 아이템 표시)
                currentShopItems = new List<ItemData>(allItems);
            }

            // 아이템을 섞어서 랜덤한 순서로 표시
            ShuffleList(currentShopItems);

            // 아이템 버튼 생성
            PopulateGrid();
        }

        // 아이템 선택 시 호출
        public void SelectItem(ItemData item)
        {
            selectedItem = item;
            buyButton.interactable = true;
            effectText.text = item.description;
            npcIllustration.sprite = item.illustration;
        }

        // UI 초기화 (선택 해제)
        private void ClearDetail()
        {
            buyButton.interactable = false;
            effectText.text = "";
            npcIllustration.sprite = null;
        }

        // 구매 버튼 클릭 시
        public void OnBuyButton()
        {
            if (selectedItem == null) return;

            // StatManager에서 현재 스탯 가져오기
            var statManager = StatManager.Instance;
            if (statManager == null || statManager.statArr == null)
            {
                Debug.LogError("StatManager를 찾을 수 없습니다!");
                return;
            }

            // 현재 자원 확인 - 인덱스로 직접 접근
            int currentUnrest = 0;
            int currentGold = 0;
            int currentDominance = 0;
            int currentManpower = 0;

            // StatManager의 statArr 순서대로 직접 접근
            if (statManager.statArr.Length >= 4)
            {
                currentUnrest = statManager.statArr[0].GetStat;      // Element 0: Unrest
                currentGold = statManager.statArr[1].GetStat;        // Element 1: Finance (Gold)
                currentDominance = statManager.statArr[2].GetStat;   // Element 2: Dominance  
                currentManpower = statManager.statArr[3].GetStat;    // Element 3: Manpower

                Debug.Log($"현재 보유 - 불만: {currentUnrest}, 골드: {currentGold}, 지배: {currentDominance}, 인력: {currentManpower}");
            }
            else
            {
                Debug.LogError($"StatManager의 statArr 크기가 예상과 다릅니다: {statManager.statArr.Length}");
                return;
            }

            // 구매 가능 여부 확인
            bool canBuy = currentGold >= selectedItem.price
                       && currentUnrest >= selectedItem.unrest
                       && currentDominance >= selectedItem.dominance
                       && currentManpower >= selectedItem.manpower;

            if (!canBuy)
            {
                Debug.Log("자원이 부족합니다!");
                Debug.Log($"필요: 골드 {selectedItem.price}, 불만 {selectedItem.unrest}, 지배 {selectedItem.dominance}, 인력 {selectedItem.manpower}");
                return;
            }

            // 유닛 카드인 경우 덱 공간 확인
            if (selectedItem.itemType == ItemType.Unit && selectedItem.unitPrefab != null)
            {
                if (cardManager.IsCardDeckFull())
                {
                    Debug.Log("카드덱이 가득 찼습니다!");
                    return;
                }
            }

            // 구매 처리 - 인덱스로 직접 처리
            if (selectedItem.unrest > 0)
            {
                statManager.statArr[0].OnValueChange(-selectedItem.unrest);
                Debug.Log($"불만 {selectedItem.unrest} 차감");
            }
            if (selectedItem.price > 0)
            {
                statManager.statArr[1].OnValueChange(-selectedItem.price);
                Debug.Log($"골드 {selectedItem.price} 차감");
            }
            if (selectedItem.dominance > 0)
            {
                statManager.statArr[2].OnValueChange(-selectedItem.dominance);
                Debug.Log($"지배 {selectedItem.dominance} 차감");
            }
            if (selectedItem.manpower > 0)
            {
                statManager.statArr[3].OnValueChange(-selectedItem.manpower);
                Debug.Log($"인력 {selectedItem.manpower} 차감");
            }

            // 유닛 카드 추가
            if (selectedItem.itemType == ItemType.Unit && selectedItem.unitPrefab != null)
            {
                cardManager.AddCardFromShopItem(selectedItem);
            }

            ClearDetail();
            Debug.Log($"{selectedItem.itemName} 구매 완료!");
        }

        // 상점 아이템 그리드 생성
        private void PopulateGrid()
        {
            // 각 아이템에 대해 버튼 생성
            foreach (var item in currentShopItems)
            {
                var go = Instantiate(itemButtonPrefab, gridParent);
                var ui = go.GetComponent<ItemButtonUI>();
                ui.Initialize(item, this);
            }
        }

        // 상점 열기/닫기
        public void OpenShop()
        {
            shopPanel.SetActive(true);

            // 상점을 열 때마다 아이템 새로고침 (확률 시스템 사용 시)
            if (useProbabilitySystem)
            {
                RefreshShopItems();
            }
        }

        public void CloseShop()
        {
            shopPanel.SetActive(false);
            ClearDetail();
        }

        // 리스트를 무작위로 섞기
        private void ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int r = Random.Range(i, list.Count);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }
    }
}

// 상점 아이템 확률 데이터를 저장하는 ScriptableObject
[CreateAssetMenu(fileName = "ShopProbabilityTable", menuName = "Shop/Probability Table")]
public class ShopProbabilityTable : ScriptableObject
{
    [System.Serializable]
    public class ItemProbability
    {
        public string itemName;
        public ItemData itemData;  // 실제 아이템 데이터 참조
        public float[] probabilities = new float[10]; // 스테이지 1-10의 확률
    }

    public List<ItemProbability> itemProbabilities = new List<ItemProbability>();

    // Wave(스테이지)에 따른 아이템 리스트 반환
    public List<ItemData> GetItemsForWave(int waveNumber)
    {
        List<ItemData> selectedItems = new List<ItemData>();

        // Wave 번호를 배열 인덱스로 변환 (1-based to 0-based)
        int waveIndex = Mathf.Clamp(waveNumber - 1, 0, 9);

        foreach (var itemProb in itemProbabilities)
        {
            if (itemProb.itemData == null) continue;

            float probability = itemProb.probabilities[waveIndex];

            // 확률이 0이면 스킵
            if (probability <= 0) continue;

            // 100% 확률인 경우 무조건 추가
            if (probability >= 100)
            {
                selectedItems.Add(itemProb.itemData);
            }
            else
            {
                // 확률에 따라 랜덤 선택
                float random = Random.Range(0f, 100f);
                if (random < probability)
                {
                    selectedItems.Add(itemProb.itemData);
                }
            }
        }

        return selectedItems;
    }
}

// Inspector에서 확률표를 편집하기 쉽게 하는 Custom Editor

#if UNITY_EDITOR


[CustomEditor(typeof(ShopProbabilityTable))]
public class ShopProbabilityTableEditor : Editor
{
    private Vector2 scrollPosition;

    public override void OnInspectorGUI()
    {
        ShopProbabilityTable table = (ShopProbabilityTable)target;

        EditorGUILayout.LabelField("상점 아이템 확률표", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // CSV 확률표와 동일한 항목들 표시
        string[] itemNames = {
            "용사", "기사단", "기사", "견습 기사", "견습 암살자",
            "오우거", "듀라한", "피 중독자", "스켈레톤", "슬라임",
            "마법사", "아처 마스터", "아처", "견습 아처", "피의 주인",
            "하급 뱀파이어", "리치", "스켈레트 아처", "역병 의사",
            "아처 타워", "마법 타워", "암살자"
        };

        // 모든 항목이 있는지 확인하고 없으면 추가
        foreach (string itemName in itemNames)
        {
            bool exists = false;
            foreach (var item in table.itemProbabilities)
            {
                if (item.itemName == itemName)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                var newItem = new ShopProbabilityTable.ItemProbability();
                newItem.itemName = itemName;
                table.itemProbabilities.Add(newItem);
            }
        }

        // 새 아이템 추가 버튼
        if (GUILayout.Button("새 아이템 추가"))
        {
            table.itemProbabilities.Add(new ShopProbabilityTable.ItemProbability());
        }

        EditorGUILayout.Space();

        // 스크롤 뷰 시작
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // 각 아이템별로 표시
        for (int i = 0; i < table.itemProbabilities.Count; i++)
        {
            var item = table.itemProbabilities[i];

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            item.itemName = EditorGUILayout.TextField("아이템 이름", item.itemName);
            if (GUILayout.Button("삭제", GUILayout.Width(50)))
            {
                table.itemProbabilities.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();

            item.itemData = (ItemData)EditorGUILayout.ObjectField("아이템 데이터",
                item.itemData, typeof(ItemData), false);

            EditorGUILayout.LabelField("Wave별 확률 (%)");

            // 확률 입력 필드들을 가로로 배치
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < 10; j++)
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(50));
                EditorGUILayout.LabelField($"W{j + 1}", GUILayout.Width(50));
                item.probabilities[j] = EditorGUILayout.FloatField(item.probabilities[j],
                    GUILayout.Width(50));
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(table);
        }
    }
}
#endif