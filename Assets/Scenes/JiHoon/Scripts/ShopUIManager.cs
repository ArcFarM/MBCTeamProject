using UnityEngine;
using UnityEngine.UI;
namespace JiHoon
{

    public class ShopUIManager : MonoBehaviour
    {
        [Header("Panel Refences")]
        public GameObject shopPanel; // 상점 패널
        public Button openButton; // 상점 버튼
        public Button closeButton; // 닫기 버튼

        [Header("Card Grid")]
        public Transform cardGrid; // 카드 그리드 부모 오브젝트
        public GameObject cardSlotPrefab; // 카드 프리팹

        [Header("Other UI")]
        public Button buyButton; // 구매 버튼


        private void Start()
        {
            //카드 슬롯 채우기(샘플 9개)
            PopulateCard(9);

            shopPanel.SetActive(false); // 시작 시 상점 패널 비활성화
        }

        void PopulateCard(int count)
        {
            //기존 슬롯 모두 지우기
            foreach (Transform t in cardGrid) Destroy(t.gameObject);

            for (int i = 0; i < count; i++)
            {
                //CardSlot 프리펩을 CardGrid 바로 아래 생성
                var slot = Instantiate(cardSlotPrefab, cardGrid, false);

            }

        }

        public void OpenShop()
        {
            shopPanel.SetActive(true); // 상점 패널 활성화
        }
        public void CloseShop()
        {
            shopPanel.SetActive(false); // 상점 패널 비활성화
        }

        public void BuyButton()
        {
            Debug.Log("구매 버튼 클릭됨");
        }

    }
}