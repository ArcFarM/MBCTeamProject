using UnityEngine;
using System.Collections.Generic;

//UI 내에서 유닛 카드를 들고 있는 부분을 담당
namespace MainGame.UI {
    public class CardList : MonoBehaviour {
        #region Variables
        //카드가 담길 리스트와 현재 index
        [SerializeField] List<GameObject> cards;
        int index;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Method

        #endregion

        #region Custom Method
        //index 값 재설정
        void ResetIndex() {
            index = cards.Count - 1;
        }
        //상점에서 카드를 구매했을 때
        public void AddCard(GameObject go) {
            cards.Add(go);
            ResetIndex();
        }
        //카드를 재배치 해야 할 때
        public void AlignCard() {

        }
        #endregion
    }

}
