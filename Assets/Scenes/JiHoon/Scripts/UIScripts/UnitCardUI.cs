using JiHoon;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class UnitCardUI : MonoBehaviour, IPointerClickHandler
{
    public Image cardImage;
    [HideInInspector] public int presetIndex;
    [HideInInspector] public UnitPlacementManager placementMgr;

    // 구매한 아이템 정보 저장용
    [HideInInspector] public bool isFromShop = false;
    [HideInInspector] public ItemData shopItemData;

    public void Init(int idx, Sprite sprite, UnitPlacementManager mgr)
    {
        presetIndex = idx;
        cardImage.sprite = sprite;
        placementMgr = mgr;
        isFromShop = false;
        shopItemData = null;
    }

    public void OnPointerClick(PointerEventData e)
    {
        placementMgr.OnClickSelectUmit(this);
    }
    public void InitFromShopItem(ItemData item, UnitPlacementManager mgr)
    {
        // 구매한 아이템 정보 저장
        cardImage.sprite = item.icon;
        placementMgr = mgr;
        // unitPrefab 정보도 어딘가에 저장해야 함
    }
    
}