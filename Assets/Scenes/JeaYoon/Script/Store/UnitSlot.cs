using UnityEngine;
using UnityEngine.UI;
using JiHoon;

/* [0] 개요 : UnitSlot
        - 상점 슬롯을 관리하는 UI 스크립트.
        - 유닛을 보여주고, 버튼을 눌러서 유닛을 구매할 수 있게 함.
*/

namespace JeaYoon
{
    public class UnitSlot : MonoBehaviour
    {
        // [1] Variable.
        #region ▼▼▼▼▼ Variable ▼▼▼▼▼
        // [◆] - ▶▶▶ 정의.
        public Image unitIcon;
        public Button buyButton;
        private UnitData unitData;
        #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲





        // [2] Custom Method.
        #region ▼▼▼▼▼ Custom Method ▼▼▼▼▼
        // [◆] - ▶▶▶ SetUnit → 유닛 슬롯에 유닛을 셋팅.
        public void SetUnit(UnitData data)
        {
            unitData = data;                                               // ) UnitData 스크립트에서 정보를 꺼내서 UnitData unitData;에 저장함.
            unitIcon.sprite = data.icon;                                  // ) UnitData 스크립트에서 icon을 꺼내서 Image unitIcon;에 저장함.
            buyButton.onClick.RemoveAllListeners();                   // ) 버튼에 등록된 모든 클릭 이벤트 제거.
            buyButton.onClick.AddListener(() => BuyUnit());         // ) 버튼을 클릭 했을 때, BuyUnit(가 실행하도록 새로 연결.
            gameObject.SetActive(true);
        }

        // [◆] - ▶▶▶ ClearSlot → 슬롯 비우기.
        public void ClearSlot()
        {
            unitData = null;
            unitIcon.sprite = null;
            buyButton.onClick.RemoveAllListeners();         // ) SetUnit과 ClearSlot에 둘 다 있어야 버튼이 중복되거나 잘못 작동하지 않게 할 수 있음.
            gameObject.SetActive(false);
        }

        // [◆] - ▶▶▶ BuyUnit → 유닛 구매.
        void BuyUnit()
        {
            Debug.Log($"{unitData.unitName}를 구매하였습니다.");
            // TODO : 구매 처리 로직 추가
        }
        #endregion ▲▲▲▲▲ Custom Method ▲▲▲▲▲
    }
}
