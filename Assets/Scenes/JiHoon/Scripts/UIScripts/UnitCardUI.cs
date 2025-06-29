using JiHoon;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class UnitCardUI : MonoBehaviour, IPointerClickHandler
{
    public Image cardImage;            // 드래그로 UI 이미지 세팅
    [HideInInspector] public int presetIndex;
    [HideInInspector] public UnitPlacementManager placementMgr;

    public void Init(int idx, Sprite sprite, UnitPlacementManager mgr)
    {
        presetIndex = idx;
        cardImage.sprite = sprite;
        placementMgr = mgr;
    }

    public void OnPointerClick(PointerEventData e)
    {
        placementMgr.OnClickSelectUmit(this);
    }
}