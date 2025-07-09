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

        private List<SimpleCardUI> cards = new List<SimpleCardUI>();  // 현재 표시중인 카드 리스트
        private int hoveredCardIndex = -1;                            // 현재 호버중인 카드 인덱스
        private SimpleCardUI selectedCard = null;                     // 현재 선택된 카드

        void Start()
        {
            var layoutGroup = GetComponent<HorizontalLayoutGroup>();
            if (layoutGroup) layoutGroup.enabled = false;
            UpdateCardPositions();
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
            }

            // SimpleCardUI 설정 및 리스트에 추가
            card.Setup(cardUI, cards.Count, this);
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

        // 카드 호버 상태 변경
        public void OnCardHover(int cardIndex, bool isHovering)
        {
            hoveredCardIndex = isHovering ? cardIndex : -1;
            UpdateCardPositions();
        }

        // 모든 카드 제거
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
    }

    public class SimpleCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("카드 UI 요소")]
        public Image cardIcon;                      // 카드 아이콘 이미지

        [HideInInspector] public int cardIndex;     // 덱에서의 카드 순서
        [HideInInspector] public UnitCardUI originalCard;  // UnitCardManager가 관리하는 원본 카드 참조

        private SimpleCardDeck parentDeck;          // 부모 덱 참조
        private Vector3 targetPosition;             // 목표 위치
        private float targetScale = 1f;             // 목표 크기
        private Coroutine moveCoroutine;            // 이동 애니메이션 코루틴

        // 카드 초기 설정
        public void Setup(UnitCardUI source, int index, SimpleCardDeck deck)
        {
            originalCard = source;  // 원본 카드 참조 저장
            cardIndex = index;
            parentDeck = deck;

            if (cardIcon == null)
                cardIcon = GetComponent<Image>();

            if (cardIcon && source.cardImage)
            {
                cardIcon.sprite = source.cardImage.sprite;
                cardIcon.raycastTarget = true;
            }

            transform.localScale = Vector3.one;
        }

        // 카드의 목표 위치와 크기 설정
        public void SetTargetTransform(Vector3 position, float rotation, float scale)
        {
            targetPosition = position;
            targetScale = scale;

            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);

            moveCoroutine = StartCoroutine(MoveToTarget());
        }

        // 부드러운 이동 애니메이션
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

        // 마우스 호버 시작
        public void OnPointerEnter(PointerEventData eventData)
        {
            parentDeck.OnCardHover(cardIndex, true);
        }

        // 마우스 호버 종료
        public void OnPointerExit(PointerEventData eventData)
        {
            parentDeck.OnCardHover(cardIndex, false);
        }

        // 카드 클릭 시 배치 모드 활성화
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
                    parentDeck.OnCardSelected(this);  // 선택 알림
                    placementMgr.OnClickSelectUmit(originalCard);  // 배치 모드 시작
                }
            }
        }
    }
}