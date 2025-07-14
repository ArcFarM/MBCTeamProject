
using JiHoon;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitCardUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI")]
    public Image cardImage;

    [Header("호버 시 표시할 스프라이트")]
    public Sprite hoverSprite;  // 새로 추가

    [HideInInspector] public bool isFromShop = false;
    [HideInInspector] public ItemData shopItemData;
    [HideInInspector] public GameObject shopUnitPrefab;
    [HideInInspector] public int presetIndex;
    [HideInInspector] public UnitPlacementManager placementMgr;

    // 프리셋 카드 초기화 (호버 스프라이트 추가)
    public void Init(int idx, Sprite sprite, Sprite hoverSprite, UnitPlacementManager mgr)
    {
        isFromShop = false;
        shopItemData = null;
        shopUnitPrefab = null;
        presetIndex = idx;
        cardImage.sprite = sprite;
        this.hoverSprite = hoverSprite;  // 호버 스프라이트 저장
        placementMgr = mgr;
    }

    // 기존 Init 메서드 (호환성 유지)
    public void Init(int idx, Sprite sprite, UnitPlacementManager mgr)
    {
        Init(idx, sprite, null, mgr);
    }

    // 상점 카드 초기화
    public void InitFromShopItem(ItemData item, UnitPlacementManager mgr)
    {
        isFromShop = true;
        shopItemData = item;
        shopUnitPrefab = item.unitPrefab;
        placementMgr = mgr;

        // 카드 이미지를 아이템의 아이콘으로 설정
        if (cardImage != null && item.icon != null)
        {
            cardImage.sprite = item.icon;
        }

        // 호버 이미지 설정 (일러스트 사용)
        if (item.illustration != null)
        {
            hoverSprite = item.illustration;
        }

        // 카드가 제대로 보이도록 설정
        gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (placementMgr == null)
        {
            Debug.LogError("PlacementMgr이 null입니다!");
            return;
        }

        placementMgr.OnClickSelectUmit(this);
    }
}