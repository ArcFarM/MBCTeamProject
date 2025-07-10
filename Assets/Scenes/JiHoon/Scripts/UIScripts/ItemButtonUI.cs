using JiHoon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButtonUI : MonoBehaviour
{
    public Image iconImage; // 아이콘 이미지 컴포넌트
    public TextMeshProUGUI unrestText;      // ) 불만 텍스트 ???.
    public TextMeshProUGUI priceText; // 가격 텍스트 컴포넌트
    public TextMeshProUGUI dominanceText;        // ) 지배 텍스트 ???.
    public TextMeshProUGUI ManpowerText;        // ) 혼돈 텍스트 ???.
    private ItemData data;
    private ShopManager shop;

    public void Initialize(ItemData item, ShopManager shopManager)
    {
        data = item; // 아이템 데이터 설정
        shop = shopManager; // ShopManager 설정
        iconImage.sprite = data.icon; // 아이콘 이미지 설정
        unrestText.text = item.unrest.ToString();       // ) 불만 텍스트 ???.
        priceText.text = item.price.ToString(); // 가격 텍스트 설정
        dominanceText.text = item.dominance.ToString();       // ) 지배 텍스트 ???.
        ManpowerText.text = item.manpower.ToString();       // ) 혼돈 텍스트 ???.

        var button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }
    public void OnClick()
    {
        shop.SelectItem(data); // ShopManager의 SelectItem 메서드 호출
    }
}


