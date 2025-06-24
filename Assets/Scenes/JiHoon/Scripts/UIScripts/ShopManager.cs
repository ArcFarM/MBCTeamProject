using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("UI 패널")]
    public GameObject shopPanel; // 상점 UI 패널
    

    [Header("플레이어 골드")]
    public int playerGold = 1000;
    public TextMeshProUGUI goldText;

    [Header("데이터")]
    public List<ItemData> allItems;
    public Transform gridParent;
    public GameObject itemButtonPrefab;

    [Header("우측 UI")]
    public Button buyButton;

    [Header("추가 UI")]
    public TextMeshProUGUI effectText;         // 왼쪽 상단 설명용
    public Image npcIllustration;    // 오른쪽 상단 일러스트용

    private ItemData selectedItem;

    private void Start()
    {
        UpdateGoldUI();
        PopulateGrid();

        // 최초엔 비활성화
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

    // 아이템 선택 시 호출되는 함수
    public void SelectItem(ItemData item)
    {
        selectedItem = item;
        buyButton.interactable = true;

        // 1) Effect 영역에 설명 띄우기
        if (effectText != null)
            effectText.text = item.description;

        // 2) NPC ILUST 영역에 일러스트 띄우기
        if (npcIllustration != null)
            npcIllustration.sprite = item.illustration;
    }

    private void ClearDetail()
    {
        buyButton.interactable = false;
        if (effectText != null)
            effectText.text = "";            // 설명 초기화
        if (npcIllustration != null)
            npcIllustration.sprite = null;   // 일러스트 숨기기
    }

    public void OnBuyButton()
    {
        if (selectedItem == null) return;

        if (playerGold >= selectedItem.price)
        {
            playerGold -= selectedItem.price;
            UpdateGoldUI();
            Debug.Log($"{selectedItem.itemName} 구매! 남은 골드: {playerGold}");
        }
        else
        {
            Debug.Log("골드가 부족합니다!");
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
        ClearDetail(); // 상점 닫을 때 선택된 아이템 정보 초기화
    }
}