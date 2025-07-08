using MainGame.Enum;
using UnityEngine;

namespace MainGame.Units.Battle {
    public class TowerBattle : BattleBase {
        #region Variables
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        #endregion

        #region Custom Methods

        // 타워는 이동하지 않으므로 Moving 상태를 건너뛰고 바로 Fighting으로 전환
        protected override void HandleEngagingState() {
            Debug.Log($"[{gameObject.name}] Tower Engaging: 타겟 검증 시작, 현재 타겟 수: {combatTargetList.Count}");

            // 교전 불가능한 적 제거 - 람다식을 쓸 수 밖에 없어서 람다식 사용
            combatTargetList.RemoveAll(target => !EngageConditionCheck(target, out UnitBase ub, out BattleBase bb)
                                    || !target.activeSelf);

            // 교전 상태에서 교전할 대상이 없다면 다시 탐지 상태로 전환
            if (combatTargetList.Count == 0) {
                Debug.Log($"[{gameObject.name}] Tower Engaging: 유효한 타겟 없음, Detecting 상태로 전환");
                ChangeState(CombatState.Detecting);
                return;
            }
            else {
                Debug.Log($"[{gameObject.name}] Tower Engaging: 유효한 타겟 {combatTargetList.Count}개, Fighting 상태로 바로 전환");
                Debug.Log($"[{gameObject.name}] Tower Engaging: 선택된 타겟 - {combatTargetList[0].name}");
                ChangeState(CombatState.Fighting); // Moving 상태를 건너뛰고 바로 Fighting으로
            }
        }

        // 타워는 이동하지 않으므로 Moving 상태 처리를 무시하고 바로 Fighting으로 전환
        protected override void HandleMovingState() {
            Debug.Log($"[{gameObject.name}] Tower Moving: 타워는 이동하지 않음, Fighting 상태로 즉시 전환");
            ChangeState(CombatState.Fighting);
        }

        #endregion
    }
}