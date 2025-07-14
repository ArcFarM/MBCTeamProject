using UnityEngine;
using UnityEngine.UI;
using MainGame.Manager;
using MainGame.Enum;
using MainGame.Units;

namespace MainGame.UI {
    public class PolicyCard : MonoBehaviour {
        #region Variables
        //정책 카드 선택 버튼
        [SerializeField] Button acceptButton;
        //카드의 등급과 이 카드가 건드리는 요소
        [SerializeField] CardGrade cardGrade;
        [SerializeField] CardEffect cardEffect;
        //이 카드가 건드릴 능력치들과 유닛들
        [SerializeField] int[] stats;
        [SerializeField] GameObject[] units;
        [SerializeField] bool isAdd;
        #endregion

        #region Properties
        public CardGrade GetSetCardGrade {
            get { return cardGrade; }
            set { cardGrade = value; }
        }
        public CardEffect GetSetCardEffect {
            get { return cardEffect; }
            set { cardEffect = value; }
        }
        public int[] GetSetStatsArr {
            get { return stats; }
            set { stats = value; }
        }

        public GameObject[] GetSetUnitsArr {
            get { return units; }
            set { units = value; }
        }

        public bool GetAddFlag {
            get { return isAdd; }
        }
        #endregion

        #region Unity Event Method
        private void Start() {
            acceptButton.onClick.AddListener(OnPolicySelected);
        }
        #endregion

        #region Custom Method
        //카드 효과 설정하기

        void OnPolicySelected() {
            //카드 클릭하면 싱글톤이 사용
           // CardManager.Instance.ApplyEffect(this);
        }
        #endregion
    }

}
