using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("UI �г�")]
    public GameObject shopPanel; // ���� UI �г�
    

    [Header("�÷��̾� ���")]
    public int playerGold = 1000;
    public TextMeshProUGUI goldText;

    [Header("������")]
    public List<ItemData> allItems;
    public Transform gridParent;
    public GameObject itemButtonPrefab;

    [Header("���� UI")]
    public Button buyButton;

    [Header("�߰� UI")]
    public TextMeshProUGUI effectText;         // ���� ��� �����
    public Image npcIllustration;    // ������ ��� �Ϸ���Ʈ��

    private ItemData selectedItem;

    private void Start()
    {
        UpdateGoldUI();
        PopulateGrid();

        // ���ʿ� ��Ȱ��ȭ
        ClearDetail();

        
    }

    private void PopulateGrid()
    {
        foreach (var item in allItems)
        {
            var go = Instantiate(itemButtonPrefab, gridParent);
            var ui = go.GetComponent<ItemButtonUI>();
            ui.Initialize(item, this);
        }
    }

    // ������ ���� �� ȣ��Ǵ� �Լ�
    public void SelectItem(ItemData item)
    {
        selectedItem = item;
        buyButton.interactable = true;

        // 1) Effect ������ ���� ����
        if (effectText != null)
            effectText.text = item.description;

        // 2) NPC ILUST ������ �Ϸ���Ʈ ����
        if (npcIllustration != null)
            npcIllustration.sprite = item.illustration;
    }

    private void ClearDetail()
    {
        buyButton.interactable = false;
        if (effectText != null)
            effectText.text = "";            // ���� �ʱ�ȭ
        if (npcIllustration != null)
            npcIllustration.sprite = null;   // �Ϸ���Ʈ �����
    }

    public void OnBuyButton()
    {
        if (selectedItem == null) return;

        if (playerGold >= selectedItem.price)
        {
            playerGold -= selectedItem.price;
            UpdateGoldUI();
            Debug.Log($"{selectedItem.itemName} ����! ���� ���: {playerGold}");
        }
        else
        {
            Debug.Log("��尡 �����մϴ�!");
        }
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = $"Gold : {playerGold}";
    }
    public void OpenShop()
    {
        shopPanel.SetActive(true);
    }
    public void CloseShop()
    {
        shopPanel.SetActive(false);
        ClearDetail(); // ���� ���� �� ���õ� ������ ���� �ʱ�ȭ
    }
}