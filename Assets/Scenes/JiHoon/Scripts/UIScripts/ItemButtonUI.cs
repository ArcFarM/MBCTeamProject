using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButtonUI : MonoBehaviour
{
    public Image iconImage; // ������ �̹��� ������Ʈ
    public TextMeshProUGUI priceText; // ���� �ؽ�Ʈ ������Ʈ
    private ItemData data;
    private ShopManager shop;

    public void Initialize(ItemData item, ShopManager shopManager)
    {
        data = item; // ������ ������ ����
        shop = shopManager; // ShopManager ����
        iconImage.sprite = data.icon; // ������ �̹��� ����
        priceText.text = item.price.ToString(); // ���� �ؽ�Ʈ ����

        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    public void OnClick()
    {
        shop.SelectItem(data); // ShopManager�� SelectItem �޼��� ȣ��
    }
}


