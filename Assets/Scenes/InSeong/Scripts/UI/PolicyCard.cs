using UnityEngine;
using UnityEngine.UI;
using MainGame.Manager;

namespace MainGame.UI {
    public class PolicyCard : MonoBehaviour {
        #region Variables
        //정책 카드 선택 버튼
        [SerializeField] Button acceptButton;
        //카드의 효과
        
        #endregion

        #region Properties
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

        }
        #endregion
    }

}
