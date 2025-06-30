using MainGame.Enum;
using MainGame.Units.Battle;
using System.Collections.Generic;
using MainGame.Units;
using UnityEngine;
using System.Collections;

namespace MainGame.Units.Battle {
    public class BattleBase : MonoBehaviour {
        #region Variables
        [SerializeField] UnitBase ub;

        #region IBattle 구현
        [SerializeField] int maxCombatTarget;
        List<GameObject> combatTargetList = new();
        #endregion

        #region StateMachine
        [SerializeField] CombatState currentState = CombatState.Idle;
        [SerializeField] float stateUpdateInterval = 0.1f;
        List<GameObject> detectedEnemies = new();
        GameObject currentTarget;
        private Coroutine stateMachineCoroutine;
        private Coroutine movementCoroutine;
        #endregion
        #endregion

        #region Properties
        public bool isFighting {
            get { return combatTargetList.Count > 0; }
        }
        #endregion

        #region Unity Event Methods
        private void Start() {
            StartStateMachine();
        }

        private void OnDestroy() {
            StopStateMachine();
        }
        #endregion

        #region 상태 머신
        void StartStateMachine() {
            if (stateMachineCoroutine == null) {
                stateMachineCoroutine = StartCoroutine(StateMachineLoop());
            }
        }

        void StopStateMachine() {
            if (stateMachineCoroutine != null) {
                StopCoroutine(stateMachineCoroutine);
                stateMachineCoroutine = null;
            }
        }

        IEnumerator StateMachineLoop() {
            WaitForSeconds updateInterval = new WaitForSeconds(stateUpdateInterval);

            while (!ub.IsDead) {
                yield return updateInterval;

                switch (currentState) {
                    case CombatState.Idle:
                        HandleIdleState();
                        break;
                    case CombatState.Detecting:
                        HandleDetectingState();
                        break;
                    case CombatState.Engaging:
                        HandleEngagingState();
                        break;
                    case CombatState.Moving:
                        HandleMovingState();
                        break;
                    case CombatState.Fighting:
                        HandleFightingState();
                        break;
                }
            }
        }

        void ChangeState(CombatState newState) {
            if (currentState == newState) return;
            currentState = newState;
        }

        // 대기 상태: 전투 가능하면 탐지로 전환
        void HandleIdleState() {
            // TODO: 전투 가능 조건 확인 후 Detecting으로 상태 변경
        }

        // 탐지 상태: 주변 적 스캔
        void HandleDetectingState() {
            // TODO: 주변 적 탐지, 발견되면 Engaging으로 상태 변경
        }

        // 조건 확인 상태: 최적 타겟 선택
        void HandleEngagingState() {
            // TODO: 탐지된 적들 중 최적 타겟 선택, Moving으로 상태 변경
        }

        // 이동 상태: 전투 위치로 이동
        void HandleMovingState() {
            // TODO: 최적 거리 도달하면 Fighting으로 상태 변경
        }

        // 전투 상태: 실제 공격 수행
        void HandleFightingState() {
            // TODO: 전투 대상들에게 공격, 대상 없으면 Detecting으로 상태 변경
        }
        #endregion

        #region Custom Methods
        #region IBattle Methods
        public void TakeDamage(float damage) {
            if (ub.IsDead) return;
            float newHealth = ub.GetStat(StatType.CurrHealth) - damage;
            ub.SetStat(StatType.CurrHealth, newHealth);
            if (newHealth <= 0 && !ub.IsDead) {
                Die();
            }
        }

        public void HealHealth(float healAmount) {
            if (ub.IsDead) return;
            float newHealth = ub.GetStat(StatType.CurrHealth) + healAmount;
            newHealth = Mathf.Min(newHealth, ub.GetStat(StatType.BaseHealth));
            ub.SetStat(StatType.CurrHealth, newHealth);
        }

        public void Die() {
            if (ub.IsDead) return;
            ub.SetStat(StatType.CurrHealth, 0);
            // TODO: 사망 시 처리할 내용
        }

        public virtual bool IsInRange(UnitBase target) {
            if (ub.IsDead) return false;
            float distance = Vector3.Distance(transform.position, target.transform.position);
            return (distance <= ub.GetStat(StatType.CurrRange) && combatTargetList.Count < maxCombatTarget);
        }

        // 전투 조건 검사
        private bool EngageConditionCheck(GameObject target, out UnitBase targetUnit, out BattleBase targetBattleBase) {
            // TODO: 기존에 만든 전투 조건 검사 로직
            targetUnit = null;
            targetBattleBase = null;
            return false;
        }

        // 전투 돌입
        public void Engage(GameObject target) {
            // TODO: 기존에 만든 Engage 로직을 상태 머신에 맞게 수정
        }

        public virtual void Attack(GameObject target) {
            if (ub.IsDead) return;
            if (target == null) {
                Debug.LogError("공격 대상이 null입니다.");
                return;
            }

            if (target.TryGetComponent<IBattle>(out IBattle ib) && target.TryGetComponent<UnitBase>(out UnitBase ub_Method)) {
                // TODO: 공격 애니메이션 재생, 애니메이션이 끝나고 - 코루틴으로 처리 - 공격 효과 적용
                if (IsInRange(ub_Method)) ib.TakeDamage(ub.GetStat(StatType.CurrDamage));
            }
            else {
                Debug.LogError("올바른 공격 대상이 아닙니다.");
            }
        }
        #endregion
        #endregion
    }
}