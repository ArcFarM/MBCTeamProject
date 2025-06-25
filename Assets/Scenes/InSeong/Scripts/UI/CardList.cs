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
            //공간이 충분하다면 카드를 자신의 자식으로 놓기
            if(cards.Count < maxCards) {
                GameObject newUnit = Instantiate(go, transform.position, Quaternion.identity);
                cards.Add(newUnit);
                newUnit.transform.SetParent(transform);
            } else {
                //TODO : 손패가 가득 차서 카드 구매 불가
                //상점 스크립트에도 추가될 내용
            }
        }

        //제거 성공 시 true, 실패 시 false 반환
        public bool RemoveCard(GameObject go) {
            string targetID = GetCardID(go);
            if (targetID == null) return false;

            if (cards.Count == 0) {
                //TODO : 손패가 없어서 카드를 낼 수 없다는 경고 팝업
            }
            //손에서 카드를 내거나 이벤트를 통해 들고 있는 카드가 사라지는 경우
            for(int i = 0; i < cards.Count; i++) {
                AllyUnitBase aub = cards[i].GetComponent<AllyUnitBase>();
                if (aub != null && aub.GetID == targetID) {
                    //TODO : UnitManager를 통해 유닛 배치 모드에 돌입하기
                    Destroy(cards[i]);
                    cards.Remove(cards[i]);
                    break;
                }
            }

            return true;
        }

        string GetCardID(GameObject go) {
            //입력받은 유닛의 ID를 추출하는 메서드
            
            //null check
            if (go == null) {
                Debug.LogWarning("GameObject is Null at GetCardID in CardsList.cs");
                return null; 
            }
            AllyUnitBase aub = go.GetComponent<AllyUnitBase>();
            if(aub == null) {
                Debug.LogWarning("GameObject doesn't have AllyUnitBase Component at GetCardID in CardsList.cs");
                return null;
            }
            string result = aub.GetID;
            if (string.IsNullOrEmpty(result)) {
                Debug.LogWarning("GamObject doesn't have unitID at GetCardID in CardsList.cs");
            }

            return result;
        }
        #endregion
    }

}
