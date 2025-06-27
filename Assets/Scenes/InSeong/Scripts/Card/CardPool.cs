using MainGame.UI;
using UnityEngine;
using MainGame.Enum;

namespace MainGame.Card {
    public class CardPool : MonoBehaviour {
        //Policy Card 들을 저장할 Pool 오브젝트이며, 확률에 따라 정해진 등급의 카드들을 뽑아서 반환
        #region Variables
        int crisislevel = -1; //crisis 등급은 특수한 경우에만 등장
        public bool crisisEvent = false; //위기 이벤트가 발생했는지 여부
        int notBadLevel = 30; //Not Bad 등급은 30% 확률로 등장 (변동 가능)
        int goodLevel = 90; //Good 등급은 60% 확률로 등장 (변동 가능)
        int awesomeLevel = 100; //Awesome 등급은 10% 확률로 등장 (변동 가능)
        [SerializeField] GameObject[] cards;
        /* cards[(int)CardGrade.NotBad]; */
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        #endregion

        #region Custom Methods
        PolicyCard GetCard() {
            PolicyCard card = new();
            int cardGrade = Random.Range(0, 100);
            //TODO : 수치에 따라 카드 등급을 정해주고, 그에 맞는 무작위 카드를 반환
            if (crisisEvent) {
                //위기 이벤트가 발생했을 때는 위기 등급 카드만 반환
            } else {
                if(cardGrade <= notBadLevel) {
                    //Not Bad 등급 카드 반환
                    int rnd = Random.Range(0, cards[(int)CardGrade.NotBad].transform.childCount);
                    card = cards[(int)CardGrade.NotBad].transform.GetChild(rnd).GetComponent<PolicyCard>();
                } else if(cardGrade <= goodLevel) {
                    //Good 등급 카드 반환
                    int rnd = Random.Range(0, cards[(int)CardGrade.Good].transform.childCount);
                    card = cards[(int)CardGrade.Good].transform.GetChild(rnd).GetComponent<PolicyCard>();
                }
                else if (cardGrade <= awesomeLevel) {
                    //Awesome 등급 카드 반환
                    int rnd = Random.Range(0, cards[(int)CardGrade.Awesome].transform.childCount);
                    card = cards[(int)CardGrade.Awesome].transform.GetChild(rnd).GetComponent<PolicyCard>();
                }
            }
            return card;
        }
        #endregion
    }

}
