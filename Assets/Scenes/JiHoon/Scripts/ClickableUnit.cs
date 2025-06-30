using JiHoon;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableUnit : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // 스폰 중엔 메뉴 열지 않음
        if (!UnitPlacementManager.Instance.placementEnabled)
            return;

        // UnitPlacementManager가 인스펙터에서 할당받은 프리팹으로
        // 메뉴 인스턴스를 만들어두는 Menu 프로퍼티를 사용
        var menu = UnitPlacementManager.Instance.Menu;
        if (menu != null)
        {
            menu.Show(gameObject, eventData.position);
        }
        else
        {
            Debug.LogError("Context Menu Prefab이 UnitPlacementManager에 할당되지 않았습니다!");
        }
    }
}