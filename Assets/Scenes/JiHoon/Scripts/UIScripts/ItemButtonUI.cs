using JiHoon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButtonUI : MonoBehaviour
{
    public Image iconImage; // 아이콘 이미지 컴포넌트
    public TextMeshProUGUI discontentText;      // ) 불만 텍스트 ???.
    public TextMeshProUGUI priceText; // 가격 텍스트 컴포넌트
    public TextMeshProUGUI dominaceText;        // ) 지배 텍스트 ???.
    public TextMeshProUGUI chaosText;        // ) 혼돈 텍스트 ???.
    private ItemData data;
    private ShopManager shop;

    public void Initialize(ItemData item, ShopManager shopManager)
    {
        data = item; // 아이템 데이터 설정
        shop = shopManager; // ShopManager 설정
        iconImage.sprite = data.icon; // 아이콘 이미지 설정
        discontentText.text = item.discontent.ToString();       // ) 불만 텍스트 ???.
        priceText.text = item.price.ToString(); // 가격 텍스트 설정
        dominaceText.text = item.dominace.ToString();       // ) 지배 텍스트 ???.
        chaosText.text = item.chaos.ToString();       // ) 혼돈 텍스트 ???.

        var button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }
    public void OnClick()
    {
        shop.SelectItem(data); // ShopManager의 SelectItem 메서드 호출
    }
}


