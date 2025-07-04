using JeaYoon;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JiHoon
{
    public class ShopManager : MonoBehaviour
    {
        [Header("UI 패널")]
        public GameObject shopPanel; // 상점 UI 패널


        [Header("플레이어 골드")]
        public int playerDiscontent = 50;       // ) 게임을 시작할 때 플레이어가 갖고 있는 불만 수치.
        public int playerGold = 1000;
        public int playerDominance = 50;       // ) 게임을 시작할 때 플레이어가 갖고 있는 지배 수치.
        public int playerChaos = 50;       // ) 게임을 시작할 때 플레이어가 갖고 있는 혼돈 수치.

        public TextMeshProUGUI goldText;

        [Header("데이터")]
        public List<ItemData> allItems; // 상점에 판매되는 아이템 데이터 리스트
        public Transform gridParent; // 아이템 버튼들이 배치될 부모 오브젝트
        public GameObject itemButtonPrefab; // 아이템 버튼 프리팹

        [Header("우측 UI")]
        public Button buyButton;

        [Header("추가 UI")]
        public TextMeshProUGUI effectText;         // 왼쪽 상단 설명용
        public Image npcIllustration;    // 오른쪽 상단 일러스트용

        private ItemData selectedItem;  // 현재 선택된 아이템

        /*
        [Header("상점 슬롯들 (파란 영역)")]      // ) .
        public List<StoreSlot> storeSlots;
        */

        [Header("보유 유닛 슬롯들")]
        public List<OwnedUnitSlot> ownedUnitSlots;      // ) 하단에 보유한 카드 슬롯들.

        public UnitData unitData;       // ) .
        public Image iconImage;         // ) .
        private ShopManager shopManager;        // ) .


        private void Start()
        {
            UpdateGoldUI(); // 플레이어 골드 UI 초기화
            PopulateGrid(); // 아이템 버튼 생성

            // 최초엔 비활성화
            ClearDetail();

            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyButton);
        }

        // 아이템 버튼들을 그리드에 채우는 함수
        private void PopulateGrid()
        {
            // 1) 기존 아이템들을 섞음.
            List<ItemData> shuffledItems = new List<ItemData>(allItems);
            ShuffleList(shuffledItems);
            // 2) 버튼 생성.
            foreach (var item in shuffledItems)
            {
                var go = Instantiate(itemButtonPrefab, gridParent);
                var ui = go.GetComponent<ItemButtonUI>();
                ui.Initialize(item, this);
            }
        }

        // 아이템 선택 시 호출되는 함수
        public void SelectItem(ItemData item)
        {
            selectedItem = item;
            buyButton.interactable = true;

            // 1) Effect 영역에 설명 띄우기
            if (effectText != null)
                effectText.text = item.description;

            // 2) NPC ILUST 영역에 일러스트 띄우기
            if (npcIllustration != null)
                npcIllustration.sprite = item.illustration;
        }

        private void ClearDetail()
        {
            buyButton.interactable = false;
            if (effectText != null)
                effectText.text = "";            // 설명 초기화
            if (npcIllustration != null)
                npcIllustration.sprite = null;   // 일러스트 숨기기
        }

        // 구매 버튼 클릭 시 호출되는 함수   
        public void OnBuyButton()
        {
            if (selectedItem == null) return;

            bool canBuy =
                playerGold >= selectedItem.price &&
                playerDiscontent >= selectedItem.discontent &&
                playerDominance >= selectedItem.discontent &&
                playerChaos >= selectedItem.discontent;

            if (!canBuy)
            {
                Debug.Log("구매에 필요한 자원이 부족합니다!");
                return;
            }

            // 자원 차감
            playerGold -= selectedItem.price;
            playerDiscontent -= selectedItem.discontent;
            playerDominance -= selectedItem.discontent;
            playerChaos -= selectedItem.discontent;

            UpdateGoldUI();
            Debug.Log($"{selectedItem.itemName} 구매 성공! 남은 골드: {playerGold}, 불만: {playerDiscontent}, 지배: {playerDominance}, 혼돈: {playerChaos}");
        }
            

        // 플레이어 골드 UI 업데이트 함수
        private void UpdateGoldUI()
        {
            if (goldText != null)
                goldText.text = $"Gold : {playerGold}";
        }
        public void OpenShop()
        {
            shopPanel.SetActive(true);
        }
        public void CloseShop()
        {
            shopPanel.SetActive(false);
            ClearDetail(); // 상점 닫을 때 선택된 아이템 정보 초기화
        }

        // ) Fisher-Yates 알고리즘 사용.
        private void ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rand = Random.Range(i, list.Count);
                T temp = list[i];
                list[i] = list[rand];
                list[rand] = temp;
            }
        }


        // ) SetUnit.
        public void SetUnit(UnitData data, ShopManager manager)
        {
            unitData = data;
            iconImage.sprite = data.icon;
            shopManager = manager;
        }

        // ) OnBuyClicked.
        void OnBuyClicked()
        {
            shopManager.BuyUnit(unitData);
        }

        // ) BuyUnit.
        public void BuyUnit(UnitData unitData)
        {
            // 보유 슬롯 중 빈 곳에 유닛 아이콘을 추가.
            foreach (var slot in ownedUnitSlots)
            {
                if (!slot.IsOccupied)
                {
                    slot.SetUnit(unitData);         // ) 유닛 정보 + 아이콘 설정.
                    slot.SetIllustration(unitData.illustration);        // ) 일러스트 표시.
                    break;
                }
            }
        }
    }
}