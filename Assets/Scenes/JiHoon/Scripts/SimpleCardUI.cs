using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using JiHoon;

public class SimpleCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("카드 UI 요소")]
    public Image cardIcon;                      // 카드 아이콘 이미지

    // 호버 관련 필드
    private Sprite hoverSprite;                 // 마우스 오버 시 표시할 스프라이트
    private Sprite originalSprite;              // 원본 스프라이트 저장용

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
            originalSprite = cardIcon.sprite;  // 원본 스프라이트 저장
            cardIcon.raycastTarget = true;
        }

        transform.localScale = Vector3.one;
    }

    // 호버 스프라이트 설정 메서드
    public void SetHoverSprite(Sprite sprite)
    {
        hoverSprite = sprite;
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
        // parentDeck에 자신의 참조도 함께 전달
        parentDeck.OnCardHover(cardIndex, true, this);

        // 호버 스프라이트가 설정되어 있으면 이미지 변경
        if (hoverSprite != null && cardIcon != null)
        {
            cardIcon.sprite = hoverSprite;
        }
    }

    // 마우스 호버 종료
    public void OnPointerExit(PointerEventData eventData)
    {
        parentDeck.OnCardHover(cardIndex, false);

        // 원본 스프라이트로 복원
        if (originalSprite != null && cardIcon != null)
        {
            cardIcon.sprite = originalSprite;
        }
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