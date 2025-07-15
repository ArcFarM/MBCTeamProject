using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MainGame.Manager;
using MainGame.Enum;

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

        [Header("우측 UI")]
        public Button buyButton;               // 구매 버튼

        [Header("추가 UI")]
        public TextMeshProUGUI effectText;     // 아이템 설명 텍스트
        public Image npcIllustration;          // NPC 일러스트

        private ItemData selectedItem;         // 현재 선택된 아이템

        private void Start()
        {
            // StatManager 초기화를 기다린 후 상점 초기화
            StartCoroutine(DelayedInit());
        }

        private IEnumerator DelayedInit()
        {
            // StatManager가 초기화될 때까지 대기
            yield return new WaitForSeconds(0.5f);

            PopulateGrid();
            ClearDetail();
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyButton);
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

        // 상점 열기/닫기
        public void OpenShop()
        {
            shopPanel.SetActive(true);
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