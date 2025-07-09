using JeaYoon;
using MainGame.Manager;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JiHoon
{
    public class ShopManager : MonoBehaviour
    {
        [Header("카드 덱 매니저")]
        public UnitCardManager cardManager;

        [Header("UI 패널")]
        public GameObject shopPanel;

        [Header("플레이어 자원")]
        public int playerDiscontent = 50;
        public int playerGold = 1000;
        public int playerDominance = 50;
        public int playerChaos = 50;
        public TextMeshProUGUI goldText;

        [Header("데이터")]
        public List<ItemData> allItems;
        public Transform gridParent;
        public GameObject itemButtonPrefab;

        [Header("우측 UI")]
        public Button buyButton;

        [Header("추가 UI")]
        public TextMeshProUGUI effectText;
        public Image npcIllustration;

        private ItemData selectedItem;

        private void Start()
        {
            UpdateGoldUI();
            PopulateGrid();
            ClearDetail();
            buyButton.onClick.RemoveAllListeners();
            
        }

        private void PopulateGrid()
        {
            var shuffled = new List<ItemData>(allItems);
            ShuffleList(shuffled);
            foreach (var item in shuffled)
            {
                var go = Instantiate(itemButtonPrefab, gridParent);
                var ui = go.GetComponent<ItemButtonUI>();
                ui.Initialize(item, this);
            }
        }

        public void SelectItem(ItemData item)
        {
            selectedItem = item;
            buyButton.interactable = true;
            effectText.text = item.description;
            npcIllustration.sprite = item.illustration;
        }

        private void ClearDetail()
        {
            buyButton.interactable = false;
            effectText.text = "";
            npcIllustration.sprite = null;
        }

        public void OnBuyButton()
        {
            if (selectedItem == null) return;

            bool canBuy =
                 playerGold >= selectedItem.price
              && playerDiscontent >= selectedItem.discontent
              && playerDominance >= selectedItem.dominace
              && playerChaos >= selectedItem.chaos;

            if (!canBuy)
            {
                Debug.Log("자원이 부족합니다!");
                return;
            }

            playerGold -= selectedItem.price;
            playerDiscontent -= selectedItem.discontent;
            playerDominance -= selectedItem.dominace;
            playerChaos -= selectedItem.chaos;
            UpdateGoldUI();

            if (selectedItem.itemType == ItemType.Unit && selectedItem.unitPrefab != null)
            {
                cardManager.AddCardFromShopItem(selectedItem);
            }
        }

        private void UpdateGoldUI()
        {
            goldText.text = $"Gold : {playerGold}";
        }

        public void OpenShop() => shopPanel.SetActive(true);
        public void CloseShop() { shopPanel.SetActive(false); ClearDetail(); }

        private void ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int r = Random.Range(i, list.Count);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }
    }
}
