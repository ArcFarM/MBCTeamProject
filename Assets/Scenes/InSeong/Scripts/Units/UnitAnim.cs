using UnityEngine;
using MainGame.Units.Animation;
using System.Collections;

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

        //animdata 반환
        public UnitAnimFrameConfig GetAnimData() {
            if (currentConfig != null) {
                return currentConfig;
            }
            else return null;
        }

        //사망 시 모든 애니메이션을 중지하고 사망 애니메이션 재생
        public IEnumerator PlayDeathAnim() {
            if (animator != null) {
                animator.SetTrigger(animParam.Param_trigger_death);
                animator.SetBool(animParam.Param_bool_isDeath, true);
                yield return new WaitForSeconds(1f); // 잠시 대기하여 상태가 적용되도록 함
            }
            else {
                Debug.LogError("Animator is not assigned.");
            }
            gameObject.SetActive(false); // 애니메이션 재생 후 오브젝트 비활성화
            Destroy(gameObject);
        }
        #endregion
    }
}