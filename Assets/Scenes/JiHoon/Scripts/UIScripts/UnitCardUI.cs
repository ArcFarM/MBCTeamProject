
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
        cardImage.sprite = item.icon;
        hoverSprite = item.illustration;  // 상점 아이템은 illustration을 호버 이미지로 사용
        placementMgr = mgr;
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