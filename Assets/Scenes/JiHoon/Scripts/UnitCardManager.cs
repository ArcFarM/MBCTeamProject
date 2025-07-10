using System.Collections.Generic;
using UnityEngine;

namespace JiHoon
{
    public class UnitCardManager : MonoBehaviour
    {
        [Header("UnitSpawner (unitPresets 담긴 컴포넌트)")]
        public UnitSpawner spawner;          // 유닛 프리셋 정보를 가진 스포너

        [Header("카드 UI 프리팹 (UnitCardUI 붙어 있어야 함)")]
        public GameObject cardUIPrefab;      // 카드 UI 프리팹

        [Header("유닛 프리셋 리스트 (순서대로 icon 포함)")]
        public List<UnitPreset> unitPresets; // 유닛 프리셋 목록

        [Header("카드 덱을 표시할 부모 오브젝트")]
        public Transform deckContainer;      // 카드덱 UI 컨테이너

        [Header("배치 매니저")]
        public UnitPlacementManager placementMgr;  // 유닛 배치 관리자

        [Header(" 카드덱")]
        public SimpleCardDeck hearthstoneDeck;     //  UI 덱

        [Header("카드덱 모드 선택")]
        public bool useHearthstoneStyle = true;    // 하스스톤 스타일 사용 여부

        [Header("카드 개수 제한")]
        public int maxCardCount = 10;              // 최대 카드 보유 개수

        [Header("디버깅용 - 현재 카드 리스트")]
        [SerializeField] private List<GameObject> currentCards = new List<GameObject>();  // 현재 보유중인 카드 목록

        void Start()
        {
            if (placementMgr == null)
                placementMgr = FindFirstObjectByType<UnitPlacementManager>();

            if (hearthstoneDeck == null && useHearthstoneStyle)
            {
                hearthstoneDeck = deckContainer.GetComponent<SimpleCardDeck>();
                if (hearthstoneDeck == null)
                    useHearthstoneStyle = false;
            }

            if (useHearthstoneStyle && hearthstoneDeck != null)
            {
                var layoutGroup = deckContainer.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
                if (layoutGroup) layoutGroup.enabled = false;
            }
        }

        // 랜덤하게 카드를 추가 (게임 시작시나 웨이브 보상)
        public void AddRandomCards(int count)
        {
            var presets = spawner.unitPresets;
            int total = presets.Length;

            for (int i = 0; i < count; i++)
            {
                if (currentCards.Count >= maxCardCount)
                    break;

                int idx = Random.Range(0, total);

                // cardData가 null인지 확인
                if (presets[idx].cardData == null) continue;

                var cardData = presets[idx].cardData;

                if (useHearthstoneStyle && hearthstoneDeck != null)
                {
                    // 원본 카드 생성 (UnitCardManager가 관리)
                    var originalCard = Instantiate(cardUIPrefab);
                    originalCard.name = $"Card_Preset{idx}_{currentCards.Count}";
                    var originalUI = originalCard.GetComponent<UnitCardUI>();

                    // hoverIcon도 함께 전달
                    originalUI.Init(idx, cardData.unitIcon, cardData.hoverIcon, placementMgr);

                    currentCards.Add(originalCard);
                    originalCard.SetActive(false);  // UI에는 표시하지 않음

                    // SimpleCardDeck에 UI 표시 요청
                    hearthstoneDeck.AddCard(originalUI);
                }
                else
                {
                    // 기존 방식 (직접 UI에 표시)
                    var go = Instantiate(cardUIPrefab, deckContainer);
                    var ui = go.GetComponent<UnitCardUI>();

                    // hoverIcon도 함께 전달
                    ui.Init(idx, cardData.unitIcon, cardData.hoverIcon, placementMgr);
                    currentCards.Add(go);
                }
            }
        }



        // 상점에서 구매한 카드 추가
        public void AddCardFromShopItem(ItemData item)
        {
            if (item == null || item.unitPrefab == null)
                return;

            CleanupNullCards();  // null 카드 정리

            if (currentCards.Count >= maxCardCount)
                return;

            if (useHearthstoneStyle && hearthstoneDeck != null)
            {
                // 원본 카드 생성
                var originalCard = Instantiate(cardUIPrefab);
                originalCard.name = $"Card_{item.itemName}_{currentCards.Count}";
                var originalUI = originalCard.GetComponent<UnitCardUI>();
                originalUI.InitFromShopItem(item, placementMgr);

                currentCards.Add(originalCard);
                originalCard.SetActive(false);

                // UI 표시 요청
                hearthstoneDeck.AddCard(originalUI);
            }
            else
            {
                var go = Instantiate(cardUIPrefab, deckContainer);
                var ui = go.GetComponent<UnitCardUI>();
                ui.InitFromShopItem(item, placementMgr);
                currentCards.Add(go);
            }
        }

        // 카드 제거 (배치 완료시 호출)
        public void RemoveCard(UnitCardUI cardUI)
        {
            GameObject cardToRemove = null;
            int indexToRemove = -1;

            // 리스트에서 해당 카드 찾기
            for (int i = 0; i < currentCards.Count; i++)
            {
                if (currentCards[i] != null && currentCards[i].GetComponent<UnitCardUI>() == cardUI)
                {
                    cardToRemove = currentCards[i];
                    indexToRemove = i;
                    break;
                }
            }

            // 찾았으면 제거
            if (indexToRemove >= 0)
            {
                currentCards.RemoveAt(indexToRemove);
                Destroy(cardToRemove);
            }
        }

        // 모든 카드 제거
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

        // 현재 카드 개수 반환
        public int GetCardCount()
        {
            CleanupNullCards();
            return currentCards.Count;
        }

        // 카드덱이 가득 찼는지 확인
        public bool IsCardDeckFull()
        {
            CleanupNullCards();
            return currentCards.Count >= maxCardCount;
        }

        // null 카드 정리
        public void CleanupNullCards()
        {
            currentCards.RemoveAll(card => card == null);
        }

        // 남은 슬롯 개수 반환
        public int GetRemainingSlots()
        {
            return maxCardCount - currentCards.Count;
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