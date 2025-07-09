using JiHoon;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitCardUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI")]
    public Image cardImage;

    [HideInInspector] public bool isFromShop = false;
    [HideInInspector] public ItemData shopItemData;
    [HideInInspector] public GameObject shopUnitPrefab;
    [HideInInspector] public int presetIndex;
    [HideInInspector] public UnitPlacementManager placementMgr;

    // 프리셋 카드 초기화
    public void Init(int idx, Sprite sprite, UnitPlacementManager mgr)
    {
        isFromShop = false;
        shopItemData = null;
        shopUnitPrefab = null;
        presetIndex = idx;
        cardImage.sprite = sprite;
        placementMgr = mgr;
    }

    // 상점 카드 초기화
    public void InitFromShopItem(ItemData item, UnitPlacementManager mgr)
    {
        isFromShop = true;
        shopItemData = item;
        shopUnitPrefab = item.unitPrefab;  // GameObject
        cardImage.sprite = item.icon;
        placementMgr = mgr;
    }

    // **모두 선택 로직만 호출** → 실제 스폰은 PlacementManager.Update 에서 처리
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