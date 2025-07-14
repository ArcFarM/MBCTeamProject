using JiHoon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButtonUI : MonoBehaviour
{
    public Image iconImage; // 아이콘 이미지 컴포넌트
    private ItemData data;
    private ShopManager shop;

    public void Initialize(ItemData item, ShopManager shopManager)
    {
        data = item; // 아이템 데이터 설정
        shop = shopManager; // ShopManager 설정
        iconImage.sprite = data.icon; // 아이콘 이미지 설정

        var button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }
    public void OnClick()
    {
        shop.SelectItem(data); // ShopManager의 SelectItem 메서드 호출
    }
}


