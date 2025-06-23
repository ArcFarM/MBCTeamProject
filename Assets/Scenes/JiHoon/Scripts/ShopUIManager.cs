using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    [Header("Panel Refences")]
    public GameObject shopPanel; // ���� �г�
    public Button openButton; // ���� ��ư
    public Button closeButton; // �ݱ� ��ư

    [Header("Card Grid")]
    public Transform cardGrid; // ī�� �׸��� �θ� ������Ʈ
    public GameObject cardSlotPrefab; // ī�� ������

    [Header("Other UI")]
    public Button buyButton; // ���� ��ư


    private void Start()
    {
        

        //ī�� ���� ä���(���� 9��)
        PopulateCard(9);

        

        shopPanel.SetActive(false); // ���� �� ���� �г� ��Ȱ��ȭ
    }

    void PopulateCard(int count)
    {
        //���� ���� ��� �����
        foreach (Transform t in cardGrid) Destroy(t.gameObject);

        for (int i = 0; i < count; i++)
        {
            //CardSlot �������� CardGrid �ٷ� �Ʒ� ����
            var slot = Instantiate(cardSlotPrefab, cardGrid, false);
            
        }

    }
    
    public void OpenShop()
    {
        shopPanel.SetActive(true); // ���� �г� Ȱ��ȭ
    }
    public void CloseShop()
    {
        shopPanel.SetActive(false); // ���� �г� ��Ȱ��ȭ
    }

    public void BuyButton()
    {
        Debug.Log("���� ��ư Ŭ����");
    }

}
