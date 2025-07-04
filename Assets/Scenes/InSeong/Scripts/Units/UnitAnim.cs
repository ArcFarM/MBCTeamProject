using UnityEngine;
using MainGame.Units.Animation;

namespace MainGame.Units {
    public class UnitAnim : MonoBehaviour {
        #region Variables
        [Header("애니메이터")]
        [SerializeField] private Animator animator;

        [Header("애니메이션 데이터")]
        [SerializeField] private UnitsAnimationData animationData;

        [Header("현재 유닛 ID")]
        [SerializeField] private string unitID;

        private UnitAnimFrameConfig currentConfig;
        private AnimParam animParam = new AnimParam();
        #endregion

        #region Unity Event Methods
        private void Start() {
            LoadAnimData(unitID);
        }
        #endregion

        #region Custom Methods
        //해당 유닛 id의 애니메이션 관련 정보를 가져옴
        void LoadAnimData(string id) {
            if(animationData == null) {
                Debug.LogError("Animation data is not assigned.");
                return;
            }
            currentConfig = System.Array.Find(animationData.unitConfigs, config => config.unitID == id);
            if(currentConfig == null) {
                Debug.LogError($"No animation config found for unit ID: {id}");
                return;
            }
        }

        //bool형과 trigger형 파라미터를 설정
        public void SetAnimBool(string paramName, bool value) {
            if (animator != null) {
                animator.SetBool(paramName, value);
            }
            else {
                Debug.LogError("Animator is not assigned.");
            }
        }
        public void SetAnimTrigger(string paramName) {
            if (animator != null) {
                animator.SetTrigger(paramName);
            }
            else {
                Debug.LogError("Animator is not assigned.");
            }
        }
        #endregion
    }
}