using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

namespace JiHoon
{
    public class SimpleCardDeck : MonoBehaviour
    {
        [Header("카드덱 설정")]
        public Transform cardContainer;
        public GameObject cardPrefab;

        [Header("카드 배치 설정")]
        public float cardSpacing = 60f;
        public float hoverHeight = 50f;
        public float hoverScale = 1.2f;
        public float hoverSpacing = 120f;
        public float animationSpeed = 5f;

        private List<SimpleCardUI> cards = new List<SimpleCardUI>();
        private int hoveredCardIndex = -1;
        private SimpleCardUI selectedCard = null; // 현재 선택된 카드 추적

        void Start()
        {
            var layoutGroup = GetComponent<HorizontalLayoutGroup>();
            if (layoutGroup) layoutGroup.enabled = false;
            UpdateCardPositions();
        }

        public void OnCardSelected(SimpleCardUI card)
        {
            selectedCard = card;
        }

        public void OnCardPlaced()
        {
            if (selectedCard != null)
            {
                RemoveCard(selectedCard.cardIndex);
                selectedCard = null;
            }
        }

        public void AddCard(UnitCardUI cardUI)
        {
            var cardObject = Instantiate(cardPrefab, cardContainer);
            var card = cardObject.GetComponent<SimpleCardUI>();

            if (card == null)
                card = cardObject.AddComponent<SimpleCardUI>();

            // 원본 UnitCardUI 정보 복사
            var originalUI = cardObject.GetComponent<UnitCardUI>();
            if (originalUI != null)
            {
                originalUI.Init(cardUI.presetIndex, cardUI.cardImage.sprite, cardUI.placementMgr);
                originalUI.isFromShop = cardUI.isFromShop;
                originalUI.shopItemData = cardUI.shopItemData;
                originalUI.shopUnitPrefab = cardUI.shopUnitPrefab;
            }

            card.Setup(originalUI != null ? originalUI : cardUI, cards.Count, this);
            cards.Add(card);
            UpdateCardPositions();
        }

        public void RemoveCard(int index)
        {
            if (index >= 0 && index < cards.Count)
            {
                Destroy(cards[index].gameObject);
                cards.RemoveAt(index);

                // 인덱스 재정렬
                for (int i = 0; i < cards.Count; i++)
                    cards[i].cardIndex = i;

                UpdateCardPositions();
            }
        }

        public void OnCardHover(int cardIndex, bool isHovering)
        {
            hoveredCardIndex = isHovering ? cardIndex : -1;
            UpdateCardPositions();
        }

        public void ClearAllCards()
        {
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
            if (cards.Count == 1) return Vector3.zero;

            // 카드 총 너비 계산
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

            // 현재 카드의 X 위치 계산
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

            // 호버된 카드는 위로
            if (index == hoveredCardIndex)
                position.y += hoverHeight;

            return position;
        }
    }

    public class SimpleCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("카드 UI 요소")]
        public Image cardIcon;

        [HideInInspector] public int cardIndex;
        [HideInInspector] public UnitCardUI originalCard;

        private SimpleCardDeck parentDeck;
        private Vector3 targetPosition;
        private float targetScale = 1f;
        private Coroutine moveCoroutine;

        public void Setup(UnitCardUI source, int index, SimpleCardDeck deck)
        {
            originalCard = source;
            cardIndex = index;
            parentDeck = deck;

            // 카드 아이콘 설정
            if (cardIcon == null)
                cardIcon = GetComponent<Image>();

            if (cardIcon && source.cardImage)
            {
                cardIcon.sprite = source.cardImage.sprite;
                cardIcon.raycastTarget = true;
            }

            transform.localScale = Vector3.one;
        }

        public void SetTargetTransform(Vector3 position, float rotation, float scale)
        {
            targetPosition = position;
            targetScale = scale;

            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);

            moveCoroutine = StartCoroutine(MoveToTarget());
        }

        private IEnumerator MoveToTarget()
        {
            Vector3 startPos = transform.localPosition;
            Vector3 startScale = transform.localScale;
            Vector3 endScale = Vector3.one * targetScale;

            float speed = parentDeck.animationSpeed;
            float journey = 0f;

            while (journey <= 1f)
            {
                journey += Time.deltaTime * speed;
                float t = Mathf.SmoothStep(0f, 1f, journey);

                transform.localPosition = Vector3.Lerp(startPos, targetPosition, t);
                transform.localScale = Vector3.Lerp(startScale, endScale, t);

                yield return null;
            }

            transform.localPosition = targetPosition;
            transform.localScale = endScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            parentDeck.OnCardHover(cardIndex, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            parentDeck.OnCardHover(cardIndex, false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var placementMgr = FindFirstObjectByType<UnitPlacementManager>();

            if (placementMgr == null) return;

            if (!placementMgr.placementEnabled)
                placementMgr.placementEnabled = true;

            if (originalCard != null)
            {
                originalCard.placementMgr = placementMgr;

                if (originalCard.placementMgr != null)
                {
                    // 카드 선택을 알림
                    parentDeck.OnCardSelected(this);
                    placementMgr.OnClickSelectUmit(originalCard);
                }
            }
        }
    }
}