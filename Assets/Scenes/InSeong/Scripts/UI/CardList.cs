using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using MainGame.Units;
using MainGame.Enum;

//UI 내에서 유닛 카드를 들고 있는 부분을 담당
namespace MainGame.UI {
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class CardList : MonoBehaviour {
        #region Variables
        //카드가 담길 리스트와 손에 들어갈 수 있는 최대 카드 수
        [SerializeField] List<GameObject> cards;
        [SerializeField] int maxCards;
        //현재 인덱스
        int index;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Method
        private void Start() {
            index = 0;
        }
        #endregion

        #region Custom Method
        //index 값 재설정
        void ResetIndex() {
            index = 0;
        }
        //상점에서 카드를 구매했을 때
        public void AddCard(GameObject go) {
            if(cards.Count < maxCards) {
                cards.Add(go);
            } else {
                //TODO : 손패가 가득 차서 카드 구매 불가
                //상점 스크립트에도 추가될 내용
            }
        }
        public void RemoveCard(GameObject go) {
            if (go.GetComponent<AllyUnitBase>() == null) return;
            if (cards.Count == 0) {
                //TODO : 손패가 없어서 카드를 낼 수 없다는 경고 팝업
            }
            //손에서 카드를 내서 재고가 사라짐
            for(int i = 0; i < cards.Count; i++) {
                if (cards[i].GetComponent<AllyUnitBase>().GetID == go.GetComponent<AllyUnitBase>().GetID) {
                    //TODO : UnitManager를 통해 유닛 배치 모드에 돌입하기
                    cards.Remove(cards[i]);
                    break;
                }
            }
        }
        #endregion
    }

}
