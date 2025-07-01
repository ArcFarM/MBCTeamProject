using JiHoon;
using UnityEngine;
using UnityEngine.UI;

public class UnitContextMenu : MonoBehaviour
{
    public Button moveButton;
    public Button sellButton;
    private GameObject _targetUnit;

    void Awake()
    {
        // 보통 메뉴는 처음에 꺼두고, Show() 할 때 켭니다.
        gameObject.SetActive(false);
    }

    /// <summary>
    /// unit을 대상으로, 화면좌표 screenPos에 메뉴를 띄웁니다.
    /// </summary>
    public void Show(GameObject unit, Vector2 screenPos)
    {
        _targetUnit = unit;

        // 화면상의 위치로 이동
        transform.SetParent(FindObjectOfType<Canvas>().transform, false);
        transform.position = screenPos;

        // 버튼 콜백 연결 (중복 방지)
        moveButton.onClick.RemoveAllListeners();
        sellButton.onClick.RemoveAllListeners();

        moveButton.onClick.AddListener(OnMoveClicked);
        sellButton.onClick.AddListener(OnSellClicked);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnMoveClicked()
    {
        Hide();
        UnitPlacementManager.Instance.BeginMove(_targetUnit);
    }

    private void OnSellClicked()
    {
        Hide();
        UnitPlacementManager.Instance.SellUnit(_targetUnit);
    }
}