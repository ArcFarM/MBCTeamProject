using System.Collections.Generic;
using UnityEngine;

namespace JiHoon
{
    public class UnitCardManager : MonoBehaviour
    {
        [Header("UnitSpawner (unitPresets 담긴 컴포넌트)")]
        public UnitSpawner spawner;

        [Header("카드 UI 프리팹 (UnitCardUI 붙어 있어야 함)")]
        public GameObject cardUIPrefab;

        [Header("유닛 프리셋 리스트 (순서대로 icon 포함)")]
        public List<UnitPreset> unitPresets;

        [Header("카드 덱을 표시할 부모 오브젝트")]
        public Transform deckContainer;

        [Header("배치 매니저")]
        public UnitPlacementManager placementMgr;

        [Header("하스스톤 스타일 카드덱")]
        public SimpleCardDeck hearthstoneDeck;

        [Header("카드덱 모드 선택")]
        public bool useHearthstoneStyle = true;

        // 현재 카드 목록 (관리용)
        private List<GameObject> currentCards = new List<GameObject>();

        void Start()
        {
            // placementMgr 자동 찾기
            if (placementMgr == null)
                placementMgr = FindFirstObjectByType<UnitPlacementManager>();

            // hearthstoneDeck 자동 찾기
            if (hearthstoneDeck == null && useHearthstoneStyle)
            {
                hearthstoneDeck = deckContainer.GetComponent<SimpleCardDeck>();
                if (hearthstoneDeck == null)
                    useHearthstoneStyle = false;
            }

            // 하스스톤 스타일 사용시 기존 레이아웃 비활성화
            if (useHearthstoneStyle && hearthstoneDeck != null)
            {
                var layoutGroup = deckContainer.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
                if (layoutGroup) layoutGroup.enabled = false;
            }
        }

        public void AddRandomCards(int count)
        {
            var presets = spawner.unitPresets;
            int total = presets.Length;

            for (int i = 0; i < count; i++)
            {
                int idx = Random.Range(0, total);

                if (useHearthstoneStyle && hearthstoneDeck != null)
                {
                    // 하스스톤 스타일
                    var tempGo = CreateTempCardUI(idx, presets[idx].icon, false, null);
                    var tempUI = tempGo.GetComponent<UnitCardUI>();

                    if (tempUI.placementMgr == null)
                        tempUI.placementMgr = placementMgr;

                    hearthstoneDeck.AddCard(tempUI);
                    tempGo.SetActive(false);
                    currentCards.Add(tempGo);
                }
                else
                {
                    // 기존 방식
                    var go = Instantiate(cardUIPrefab, deckContainer);
                    var ui = go.GetComponent<UnitCardUI>();
                    ui.Init(idx, presets[idx].icon, placementMgr);
                    currentCards.Add(go);
                }
            }
        }

        public void AddCardFromShopItem(ItemData item)
        {
            if (item == null || item.unitPrefab == null) return;

            if (useHearthstoneStyle && hearthstoneDeck != null)
            {
                // 하스스톤 스타일
                var tempGo = CreateTempCardUI(-1, item.icon, true, item);
                var tempUI = tempGo.GetComponent<UnitCardUI>();

                if (tempUI.placementMgr == null)
                    tempUI.placementMgr = placementMgr;

                hearthstoneDeck.AddCard(tempUI);
                tempGo.SetActive(false);
                currentCards.Add(tempGo);
            }
            else
            {
                // 기존 방식
                var go = Instantiate(cardUIPrefab, deckContainer);
                var ui = go.GetComponent<UnitCardUI>();
                ui.InitFromShopItem(item, placementMgr);
                currentCards.Add(go);
            }
        }

        private GameObject CreateTempCardUI(int presetIndex, Sprite icon, bool isFromShop, ItemData shopItem)
        {
            var go = Instantiate(cardUIPrefab);
            var ui = go.GetComponent<UnitCardUI>();

            if (isFromShop)
                ui.InitFromShopItem(shopItem, placementMgr);
            else
                ui.Init(presetIndex, icon, placementMgr);

            return go;
        }

        public void RemoveCard(UnitCardUI cardUI)
        {
            for (int i = currentCards.Count - 1; i >= 0; i--)
            {
                if (currentCards[i] != null)
                {
                    var ui = currentCards[i].GetComponent<UnitCardUI>();
                    if (ui == cardUI)
                    {
                        Destroy(currentCards[i]);
                        currentCards.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void ClearAllCards()
        {
            foreach (var card in currentCards)
            {
                if (card != null)
                    Destroy(card);
            }
            currentCards.Clear();

            if (useHearthstoneStyle && hearthstoneDeck != null)
                hearthstoneDeck.ClearAllCards();
        }

        public int GetCardCount()
        {
            return currentCards.Count;
        }

        public void SetHearthstoneMode(bool enable)
        {
            useHearthstoneStyle = enable;

            var layoutGroup = deckContainer.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();

            if (enable && hearthstoneDeck != null)
            {
                if (layoutGroup) layoutGroup.enabled = false;
                ConvertToHearthstoneStyle();
            }
            else
            {
                if (layoutGroup) layoutGroup.enabled = true;
                ConvertToClassicStyle();
            }
        }

        private void ConvertToHearthstoneStyle()
        {
            for (int i = 0; i < currentCards.Count; i++)
            {
                if (currentCards[i] != null)
                {
                    var ui = currentCards[i].GetComponent<UnitCardUI>();
                    hearthstoneDeck.AddCard(ui);
                    currentCards[i].SetActive(false);
                }
            }
        }

        private void ConvertToClassicStyle()
        {
            if (hearthstoneDeck != null)
                hearthstoneDeck.ClearAllCards();

            for (int i = 0; i < currentCards.Count; i++)
            {
                if (currentCards[i] != null)
                {
                    currentCards[i].SetActive(true);
                    currentCards[i].transform.SetParent(deckContainer);
                }
            }
        }
    }
}