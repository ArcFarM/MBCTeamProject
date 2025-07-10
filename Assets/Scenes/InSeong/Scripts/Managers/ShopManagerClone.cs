using MainGame.UI;
using UnityEngine;
using JiHoon;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace MainGame.Manager
{
    public class ShopManagerClone : SingletonManager<ShopManagerClone> {
        #region Variables
        [Header("상점 관리 요소")]
        [SerializeField, Header("상점에 카드를 진열할 카드 관리자")]
        UnitCardManager ucManager;
        [SerializeField, Header("상점 UI")]
        GameObject shopPanel;

        [Header("상점 UI")]
        [SerializeField, Header("상점에 올릴 데이터")]
        List<ItemData> allItems;
        [SerializeField, Header("아이템이 등록될 공간")]
        Transform itemDisplay;
        [SerializeField, Header("등록될 아이템")]
        GameObject itemPrefab;
        [SerializeField, Header("정치 스탯들")]
        SetStats[] stats;
        [SerializeField, Header("구매 버튼")]
        Button buyButton;
        [SerializeField, Header("아이템 설명창")]
        TextMeshProUGUI itemDesc;
        [SerializeField, Header("NPC 일러스트")]
        Image npcIllust;
        [SerializeField, Header("현재 선택된 아이템")]
        ItemData selectedItem;




        #endregion
        #region Properties
        #endregion
        #region Unity Event Method
        #endregion
        #region Custom Method
        void DisplayItem() {
            //상점에 아이템을 배치
        }

        void SelectItem() {
            //아이템 클릭 시 선택 처리
        }

        void ResetSelect() {
            //아이템 선택 취소
        }

        void OnBuyButtonClicked() {
            //구매 버튼 클릭하여 손패에 카드 추가
            if (IsAffordable(selectedItem)) AddCard(selectedItem);
            /*
             TODO : else {//비용이 부족하여 구매 불가라는 안내}
             */
        }

        bool IsAffordable(ItemData id) {
            //구매 가능한 지 판정

            //비용이 불충분하거나 손패가 가득 찼다면 살 수 없다
            if (id == null) return false;
            if (!(
                stats[(int)Enum.Stats.Unrest].GetStat >= id.unrest
                && stats[(int)Enum.Stats.Money].GetStat >= id.price
                && stats[(int)Enum.Stats.Dominance].GetStat >= id.dominance
                && stats[(int)Enum.Stats.Manpower].GetStat >= id.manpower
                )) return false;

            if (ucManager.IsCardDeckFull()) return false;

            return true;
        }

        void AddCard(ItemData id) {
            //카드를 손패에 추가
            if (id == null) return;
            //일단 유닛 카드만 구매 가능하므로 유닛 타입이 아니면 잘못된 거임
            if (id.itemType != ItemType.Unit) return;
            if (id.unitPrefab == null) return;

            ucManager.AddCardFromShopItem(id);
        }

        void ToggleShopWindow() {
            if (shopPanel == null) return;
            //상점 창 껐다 켰다
            if (shopPanel.activeSelf) {
                shopPanel.SetActive(false);
                ResetSelect();
            } else {
                shopPanel.SetActive(true);
            }
        }
        
        public void ShuffleList<T>(List<T> list) {
            //상점 요소를 구매 가능한 요소 들 중에서 무작위로 찾아서 배치
        }
        #endregion
    }
}
