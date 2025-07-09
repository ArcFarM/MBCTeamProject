using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JiHoon
{
    public class ShopManager : MonoBehaviour
    {
        [Header("카드 덱 매니저")]
        public UnitCardManager cardManager;     // 카드 관리자 참조

        [Header("UI 패널")]
        public GameObject shopPanel;            // 상점 UI 패널

        [Header("플레이어 자원")]
        public int playerDiscontent = 50;       // 불만 자원
        public int playerGold = 1000;          // 골드
        public int playerDominance = 50;       // 지배 자원
        public int playerChaos = 50;           // 혼돈 자원
        public TextMeshProUGUI goldText;       // 골드 표시 텍스트

        [Header("데이터")]
        public List<ItemData> allItems;        // 상점에 표시할 모든 아이템
        public Transform gridParent;           // 아이템 버튼들의 부모 오브젝트
        public GameObject itemButtonPrefab;    // 아이템 버튼 프리팹

        [Header("우측 UI")]
        public Button buyButton;               // 구매 버튼

        [Header("추가 UI")]
        public TextMeshProUGUI effectText;     // 아이템 설명 텍스트
        public Image npcIllustration;          // NPC 일러스트

        private ItemData selectedItem;         // 현재 선택된 아이템

        private void Start()
        {
            UpdateGoldUI();
            PopulateGrid();
            ClearDetail();
            buyButton.onClick.RemoveAllListeners();
            
        }

        // 상점 아이템 그리드 생성
        private void PopulateGrid()
        {
            var shuffled = new List<ItemData>(allItems);
            ShuffleList(shuffled);  // 무작위 순서로 섞기

            // 각 아이템에 대해 버튼 생성
            foreach (var item in shuffled)
            {
                var go = Instantiate(itemButtonPrefab, gridParent);
                var ui = go.GetComponent<ItemButtonUI>();
                ui.Initialize(item, this);
            }
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

            // 자원 확인
            bool canBuy =
                 playerGold >= selectedItem.price
              && playerDiscontent >= selectedItem.discontent
              && playerDominance >= selectedItem.dominace
              && playerChaos >= selectedItem.chaos;

            if (!canBuy)
            {
                Debug.Log("자원이 부족합니다!");
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

            // 구매 처리 - 자원 차감
            playerGold -= selectedItem.price;
            playerDiscontent -= selectedItem.discontent;
            playerDominance -= selectedItem.dominace;
            playerChaos -= selectedItem.chaos;
            UpdateGoldUI();

            // 유닛 카드 추가
            if (selectedItem.itemType == ItemType.Unit && selectedItem.unitPrefab != null)
            {
                cardManager.AddCardFromShopItem(selectedItem);
            }

            ClearDetail();
        }

        // 골드 UI 업데이트
        private void UpdateGoldUI()
        {
            goldText.text = $"Gold : {playerGold}";
        }

        // 상점 열기/닫기
        public void OpenShop() => shopPanel.SetActive(true);
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