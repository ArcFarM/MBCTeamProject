using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JiHoon
{
    public class SimpleCardDeck : MonoBehaviour
    {
        [Header("카드덱 설정")]
        public Transform cardContainer;      // 카드들이 배치될 부모 오브젝트
        public GameObject cardPrefab;        // 카드 UI 프리팹

        [Header("카드 배치 설정")]
        public float cardSpacing = 60f;      // 카드 간 기본 간격
        public float hoverHeight = 50f;      // 호버 시 카드가 올라가는 높이
        public float hoverScale = 1.2f;      // 호버 시 카드 크기 배율
        public float hoverSpacing = 120f;    // 호버 시 카드 간격
        public float animationSpeed = 5f;    // 애니메이션 속도

        [Header("툴팁 설정")]
        public GameObject tooltipPrefab;            // 툴팁 프리팹
        public Vector2 tooltipOffset = new Vector2(20f, -20f);  // 마우스로부터의 오프셋

        private List<SimpleCardUI> cards = new List<SimpleCardUI>();  // 현재 표시중인 카드 리스트
        private int hoveredCardIndex = -1;                            // 현재 호버중인 카드 인덱스
        private SimpleCardUI selectedCard = null;                     // 현재 선택된 카드
        private GameObject currentTooltip = null;                     // 현재 표시 중인 툴팁
        private SimpleCardUI currentHoveredCard = null;              // 현재 호버중인 카드 참조
        private UnitSpawner spawner;                                 // UnitSpawner 참조

        void Start()
        {
            var layoutGroup = GetComponent<HorizontalLayoutGroup>();
            if (layoutGroup) layoutGroup.enabled = false;
            UpdateCardPositions();

            // UnitSpawner 찾기
            spawner = FindFirstObjectByType<UnitSpawner>();
            if (spawner == null)
            {
                Debug.LogWarning("UnitSpawner를 찾을 수 없습니다!");
            }
        }

        void Update()
        {
            // 툴팁이 표시중이면 마우스를 따라가도록 업데이트
            if (currentTooltip != null && currentTooltip.activeSelf)
            {
                UpdateTooltipPosition();
            }
        }

        // 툴팁 위치를 마우스 위치로 업데이트
        private void UpdateTooltipPosition()
        {
            if (currentTooltip == null) return;

            RectTransform tooltipRect = currentTooltip.GetComponent<RectTransform>();
            if (tooltipRect != null)
            {
                // 마우스 위치 + 오프셋
                Vector2 mousePos = Input.mousePosition;
                Vector2 tooltipPos = mousePos + tooltipOffset;

                tooltipRect.position = tooltipPos;

                // 화면 밖으로 나가지 않도록 조정
                ClampToScreen(tooltipRect);
            }
        }

        // 카드가 선택되었을 때 호출
        public void OnCardSelected(SimpleCardUI card)
        {
            selectedCard = card;
        }

        // 카드가 배치되었을 때 호출 (유닛 설치 완료)
        public void OnCardPlaced()
        {
            if (selectedCard != null)
            {
                // UnitCardManager에서 원본 카드 제거
                var cardManager = FindFirstObjectByType<UnitCardManager>();
                if (cardManager != null && selectedCard.originalCard != null)
                {
                    cardManager.RemoveCard(selectedCard.originalCard);
                }

                // UI 덱에서 카드 제거
                RemoveCard(selectedCard.cardIndex);
                selectedCard = null;
            }
        }

        // 새 카드를 덱에 추가
        public void AddCard(UnitCardUI cardUI)
        {
            var cardObject = Instantiate(cardPrefab, cardContainer);
            var card = cardObject.GetComponent<SimpleCardUI>();

            if (card == null)
                card = cardObject.AddComponent<SimpleCardUI>();

            // 원본 카드 정보를 새 UI 카드에 복사
            var originalUI = cardObject.GetComponent<UnitCardUI>();
            if (originalUI != null)
            {
                originalUI.Init(cardUI.presetIndex, cardUI.cardImage.sprite, cardUI.placementMgr);
                originalUI.isFromShop = cardUI.isFromShop;
                originalUI.shopItemData = cardUI.shopItemData;
                originalUI.shopUnitPrefab = cardUI.shopUnitPrefab;
                originalUI.hoverSprite = cardUI.hoverSprite;  // 호버 스프라이트 복사
            }

            // SimpleCardUI 설정 및 리스트에 추가
            card.Setup(cardUI, cards.Count, this);

            // 호버 스프라이트 설정
            if (!cardUI.isFromShop && spawner != null)
            {
                var presets = spawner.unitPresets;
                if (cardUI.presetIndex >= 0 && cardUI.presetIndex < presets.Length)
                {
                    var cardData = presets[cardUI.presetIndex].cardData;
                    if (cardData != null && cardData.hoverIcon != null)
                    {
                        card.SetHoverSprite(cardData.hoverIcon);
                    }
                }
            }
            else if (cardUI.hoverSprite != null)
            {
                card.SetHoverSprite(cardUI.hoverSprite);
            }

            cards.Add(card);
            UpdateCardPositions();
        }

        // 특정 인덱스의 카드를 제거
        public void RemoveCard(int index)
        {
            if (index >= 0 && index < cards.Count)
            {
                Destroy(cards[index].gameObject);
                cards.RemoveAt(index);

                // 남은 카드들의 인덱스 재정렬
                for (int i = 0; i < cards.Count; i++)
                    cards[i].cardIndex = i;

                UpdateCardPositions();
            }
        }

        // 카드 호버 상태 변경 (SimpleCardUI 참조 추가)
        public void OnCardHover(int cardIndex, bool isHovering, SimpleCardUI hoveredCard = null)
        {
            hoveredCardIndex = isHovering ? cardIndex : -1;
            currentHoveredCard = isHovering ? hoveredCard : null;
            UpdateCardPositions();

            // 툴팁 표시/숨김
            if (isHovering && hoveredCard != null)
            {
                ShowTooltip(hoveredCard);
            }
            else
            {
                HideTooltip();
            }
        }

        // 툴팁 표시
        private void ShowTooltip(SimpleCardUI card)
        {
            if (tooltipPrefab == null) return;

            // 기존 툴팁 제거
            HideTooltip();

            // Canvas를 찾아서 툴팁을 Canvas의 자식으로 생성
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                canvas = FindObjectOfType<Canvas>();
            }

            if (canvas != null)
            {
                currentTooltip = Instantiate(tooltipPrefab, canvas.transform);

                // 툴팁이 마우스 이벤트를 받지 않도록 설정
                CanvasGroup tooltipCanvasGroup = currentTooltip.GetComponent<CanvasGroup>();
                if (tooltipCanvasGroup == null)
                {
                    tooltipCanvasGroup = currentTooltip.AddComponent<CanvasGroup>();
                }
                tooltipCanvasGroup.blocksRaycasts = false; // 마우스 이벤트 무시
                tooltipCanvasGroup.interactable = false;

                // 초기 위치 설정 (마우스 위치)
                UpdateTooltipPosition();

                // 툴팁 내용 설정
                SetTooltipContent(card);
            }
        }

        // 툴팁이 화면 밖으로 나가지 않도록 조정
        private void ClampToScreen(RectTransform tooltipRect)
        {
            Vector3[] corners = new Vector3[4];
            tooltipRect.GetWorldCorners(corners);

            float minX = corners[0].x;
            float maxX = corners[2].x;
            float minY = corners[0].y;
            float maxY = corners[2].y;

            Vector3 pos = tooltipRect.position;

            if (maxX > Screen.width)
            {
                pos.x -= (maxX - Screen.width + 10);
            }
            if (minX < 0)
            {
                pos.x += (-minX + 10);
            }
            if (maxY > Screen.height)
            {
                pos.y -= (maxY - Screen.height + 10);
            }
            if (minY < 0)
            {
                pos.y += (-minY + 10);
            }

            tooltipRect.position = pos;
        }

        // 툴팁 숨기기
        private void HideTooltip()
        {
            if (currentTooltip != null)
            {
                Destroy(currentTooltip);
                currentTooltip = null;
            }
        }

        // 툴팁 내용 설정
        private void SetTooltipContent(SimpleCardUI card)
        {
            if (currentTooltip == null || card.originalCard == null) return;

            Image tooltipImage = currentTooltip.GetComponent<Image>();
            if (tooltipImage == null)
            {
                tooltipImage = currentTooltip.GetComponentInChildren<Image>();
            }

            var tooltipTextTMP = currentTooltip.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            var tooltipTextLegacy = currentTooltip.GetComponentInChildren<UnityEngine.UI.Text>();

            // 상점 카드인 경우 - UnitCardData 사용
            if (card.originalCard.isFromShop &&
                card.originalCard.shopItemData != null &&
                card.originalCard.shopItemData.unitCardData != null)
            {
                var unitCardData = card.originalCard.shopItemData.unitCardData;

                // UnitCardData의 툴팁 이미지 사용
                if (tooltipImage != null && unitCardData.tooltipImage != null)
                {
                    tooltipImage.sprite = unitCardData.tooltipImage;
                    tooltipImage.enabled = true;
                }

                // UnitCardData의 툴팁 텍스트 사용
                if (!string.IsNullOrEmpty(unitCardData.tooltipText))
                {
                    if (tooltipTextTMP != null)
                    {
                        tooltipTextTMP.text = unitCardData.tooltipText;
                    }
                    else if (tooltipTextLegacy != null)
                    {
                        tooltipTextLegacy.text = unitCardData.tooltipText;
                    }
                }
            }
            // 일반 프리셋 카드인 경우
            else if (!card.originalCard.isFromShop)
            {
                var spawner = FindFirstObjectByType<UnitSpawner>();
                if (spawner != null && spawner.unitPresets != null)
                {
                    var presets = spawner.unitPresets;
                    if (card.originalCard.presetIndex >= 0 &&
                        card.originalCard.presetIndex < presets.Length)
                    {
                        var cardData = presets[card.originalCard.presetIndex].cardData;
                        if (cardData != null)
                        {
                            if (tooltipImage != null && cardData.tooltipImage != null)
                            {
                                tooltipImage.sprite = cardData.tooltipImage;
                                tooltipImage.enabled = true;
                            }

                            if (!string.IsNullOrEmpty(cardData.tooltipText))
                            {
                                if (tooltipTextTMP != null)
                                {
                                    tooltipTextTMP.text = cardData.tooltipText;
                                }
                                else if (tooltipTextLegacy != null)
                                {
                                    tooltipTextLegacy.text = cardData.tooltipText;
                                }
                            }
                        }
                    }
                }
            }
        
    
            // 상점 아이템인 경우
            else if (card.originalCard.isFromShop && card.originalCard.shopItemData != null)
            {
                if (tooltipImage != null && card.originalCard.shopItemData.illustration != null)
                {
                    Debug.Log($"상점 아이템 툴팁 이미지 변경: {card.originalCard.shopItemData.illustration.name}");
                    tooltipImage.sprite = card.originalCard.shopItemData.illustration;
                    tooltipImage.enabled = true;
                }

                // 상점 아이템의 설명 텍스트 설정
                if (!string.IsNullOrEmpty(card.originalCard.shopItemData.description))
                {
                    if (tooltipTextTMP != null)
                    {
                        tooltipTextTMP.text = card.originalCard.shopItemData.description;
                    }
                    else if (tooltipTextLegacy != null)
                    {
                        tooltipTextLegacy.text = card.originalCard.shopItemData.description;
                    }
                }
            }
        }

        // 모든 카드 제거
        public void ClearAllCards()
        {
            HideTooltip(); // 툴팁도 제거

            foreach (var card in cards)
            {
                if (card != null)
                    Destroy(card.gameObject);
            }
            cards.Clear();
            hoveredCardIndex = -1;
        }

        void UpdateCardPositions()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                Vector3 targetPosition = CalculateCardPosition(i);
                float targetScale = (i == hoveredCardIndex) ? hoverScale : 1f;

                var card = cards[i];
                card.SetTargetTransform(targetPosition, 0f, targetScale);
                card.transform.SetSiblingIndex(i);
            }
        }

        Vector3 CalculateCardPosition(int index)
        {
            // 카드가 하나일 때도 기본 Y 위치를 유지
            if (cards.Count == 1)
            {
                Vector3 singleCardPos = Vector3.zero;
                if (index == hoveredCardIndex)
                    singleCardPos.y += hoverHeight;
                return singleCardPos;
            }

            float totalWidth = 0f;
            for (int i = 0; i < cards.Count - 1; i++)
            {
                float spacing = cardSpacing;
                if (hoveredCardIndex != -1)
                {
                    float distFromHover = Mathf.Abs(i - hoveredCardIndex);
                    if (distFromHover <= 2)
                    {
                        float influence = 1f - (distFromHover / 2f);
                        spacing = Mathf.Lerp(cardSpacing, hoverSpacing, influence);
                    }
                }
                totalWidth += spacing;
            }

            float startX = -totalWidth / 2f;
            float currentX = startX;

            for (int i = 0; i < index; i++)
            {
                float spacing = cardSpacing;
                if (hoveredCardIndex != -1)
                {
                    float distFromHover = Mathf.Abs(i - hoveredCardIndex);
                    if (distFromHover <= 2)
                    {
                        float influence = 1f - (distFromHover / 2f);
                        spacing = Mathf.Lerp(cardSpacing, hoverSpacing, influence);
                    }
                }
                currentX += spacing;
            }

            Vector3 position = new Vector3(currentX, 0, 0);

            if (index == hoveredCardIndex)
                position.y += hoverHeight;

            return position;
        }

        void OnDestroy()
        {
            HideTooltip(); // 오브젝트 파괴 시 툴팁도 제거
        }
    }
}