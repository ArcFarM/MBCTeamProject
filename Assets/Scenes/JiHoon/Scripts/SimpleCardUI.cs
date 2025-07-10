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

    [Header("툴팁 설정")]
    public GameObject tooltipPrefab;            // 툴팁 프리팹 (Inspector에서 할당)
    public Vector2 tooltipOffset = new Vector2(200f, 0f);  // 호버 카드로부터의 오프셋

    // 호버 관련 필드
    private Sprite hoverSprite;                 // 마우스 오버 시 표시할 스프라이트
    private Sprite originalSprite;              // 원본 스프라이트 저장용
    private GameObject currentTooltip;          // 현재 표시 중인 툴팁

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
        parentDeck.OnCardHover(cardIndex, true);

        // 호버 스프라이트가 설정되어 있으면 이미지 변경
        if (hoverSprite != null && cardIcon != null)
        {
            cardIcon.sprite = hoverSprite;
        }

        // 툴팁 표시
        ShowTooltip();
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

        // 툴팁 숨기기
        HideTooltip();
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

    // 툴팁 표시
    private void ShowTooltip()
    {
        if (tooltipPrefab == null) return;

        // 캔버스 찾기
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }

        if (canvas != null && currentTooltip == null)
        {
            // 툴팁 생성
            currentTooltip = Instantiate(tooltipPrefab, canvas.transform);

            // 위치 설정 (호버된 카드의 오른쪽)
            RectTransform tooltipRect = currentTooltip.GetComponent<RectTransform>();
            if (tooltipRect != null)
            {
                // 카드의 월드 좌표를 스크린 좌표로 변환
                Vector3 cardWorldPos = transform.position;
                Vector3 cardScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, cardWorldPos);

                // 툴팁 위치 설정
                tooltipRect.position = cardScreenPos + (Vector3)tooltipOffset;

                // 화면 밖으로 나가지 않도록 조정
                ClampToScreen(tooltipRect);
            }

            // 툴팁 내용 설정 (필요한 경우)
            SetTooltipContent();
        }
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
            pos.x -= (maxX - Screen.width);
        }
        if (minX < 0)
        {
            pos.x += -minX;
        }
        if (maxY > Screen.height)
        {
            pos.y -= (maxY - Screen.height);
        }
        if (minY < 0)
        {
            pos.y += -minY;
        }

        tooltipRect.position = pos;
    }

    // 툴팁 내용 설정
    private void SetTooltipContent()
    {
        if (currentTooltip == null || originalCard == null) return;

        // 툴팁에 이미지가 있다면 설정
        Image tooltipImage = currentTooltip.GetComponentInChildren<Image>();
        if (tooltipImage != null)
        {
            // 상점 아이템인 경우
            if (originalCard.isFromShop && originalCard.shopItemData != null)
            {
                // illustration을 툴팁 이미지로 사용
                if (originalCard.shopItemData.illustration != null)
                {
                    tooltipImage.sprite = originalCard.shopItemData.illustration;
                }
            }
            // 프리셋 유닛인 경우 - 별도의 툴팁 이미지가 있다면 여기서 설정
        }

        // 툴팁에 텍스트가 있다면 설정 (추후 확장용)
        // Text tooltipText = currentTooltip.GetComponentInChildren<Text>();
        // if (tooltipText != null) { ... }
    }
}

