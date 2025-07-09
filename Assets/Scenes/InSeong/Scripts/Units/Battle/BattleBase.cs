using MainGame.Enum;
using System.Collections.Generic;
using MainGame.Units.Animation;
using UnityEngine;
using System.Collections;
using NUnit.Framework.Constraints;

namespace MainGame.Units.Battle {
    public class BattleBase : MonoBehaviour, IBattle {
        #region Variables
        [SerializeField] protected UnitBase ub;
        protected float detectingRange; // 탐지 범위 - 현재는 배수와 상수로 계산하여 사용
        [SerializeField] protected float detectingMultiplier = 1.5f; // 탐지 범위 배수
        [SerializeField] protected float detectingRangeconstant = 2.5f; // 탐지 범위 상수
        [SerializeField] protected float engageDistanceMultiplier = 0.75f; // 전투 돌입 거리 배수

        #region IBattle 구현
        [SerializeField] protected int maxCombatTarget;
        [SerializeField] protected List<GameObject> combatTargetList = new();
        #endregion

        #region StateMachine
        [SerializeField] protected CombatState currentState = CombatState.Idle;
        [SerializeField] protected float stateUpdateInterval = 0.1f;
        protected Coroutine stateMachineCoroutine;
        protected Coroutine movementCoroutine;

        #region Animation
        [Header("애니메이션 관련")]
        [SerializeField] protected UnitAnim unitAnim; // 유닛 애니메이션 컴포넌트
        protected AnimParam animParam = new AnimParam(); // 애니메이션 파라미터 설정
        #endregion
        #endregion
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        private void Start() {
            //Debug.Log($"[{gameObject.name}] BattleBase 시작");
            detectingRange = Mathf.Max(ub.GetStat(StatType.CurrRange) * detectingMultiplier, detectingRangeconstant);
            StartStateMachine();
        }

        private void OnDestroy() {
            //Debug.Log($"[{gameObject.name}] BattleBase 종료");
            StopStateMachine();
        }
        #endregion

        #region 상태 머신
        protected void StartStateMachine() {
            if (stateMachineCoroutine == null) {
                //Debug.Log($"[{gameObject.name}] 상태 머신 시작");
                stateMachineCoroutine = StartCoroutine(StateMachineLoop());
            }
        }

        protected void StopStateMachine() {
            if (stateMachineCoroutine != null) {
                //Debug.Log($"[{gameObject.name}] 상태 머신 중지");
                StopCoroutine(stateMachineCoroutine);
                stateMachineCoroutine = null;
            }
        }

        protected IEnumerator StateMachineLoop() {
            WaitForSeconds updateInterval = new WaitForSeconds(stateUpdateInterval);

            while (!ub.IsDead) {
                yield return updateInterval;
                //null check
                RemoveInvalidTargets();

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
                    case CombatState.Dead:
                        HandleDeadState();
                        break;
                }
            }
            //Debug.Log($"[{gameObject.name}] 유닛 사망으로 상태 머신 종료");
        }

        protected void ChangeState(CombatState newState) {
            if (currentState == newState) return;
            //Debug.Log($"[{gameObject.name}] 상태 변경: {currentState} → {newState}");
            currentState = newState;
        }

        // 대기 상태: 전투 가능하면 탐지로 전환
        protected virtual void HandleIdleState() {
            //유닛이 살아있고, 추가 전투가 가능한 경우 전환
            if (!ub.IsDead && combatTargetList.Count < maxCombatTarget) {
                //Debug.Log($"[{gameObject.name}] Idle: 전투 가능, 탐지 상태로 전환");
                ChangeState(CombatState.Detecting);
            }
        }

        // 탐지 상태: 주변 적 스캔
        protected virtual void HandleDetectingState() {
            //Debug.Log($"[{gameObject.name}] Detecting: 주변 적 탐지 시작");
            unitAnim.SetAnimBool(animParam.Param_bool_move, false); // 이동 애니메이션 중지

            Collider2D[] detectedTargets = Physics2D.OverlapCircleAll(transform.position, detectingRange);
            //Debug.Log($"[{gameObject.name}] Detecting: 탐지 범위 {detectingRange:F2}, 발견된 객체 수: {detectedTargets.Length}");

            // 자기 자신을 제외한 모든 Unit Tag 오브젝트들을 리스트에 추가
            int addedCount = 0;
            foreach (var target in detectedTargets) {
                if(target.gameObject.tag != "Unit") continue; // Unit 태그가 아닌 오브젝트는 무시
                if (target.gameObject == gameObject || combatTargetList.Contains(target.gameObject)) continue; // 자기 자신 및 중복 대상 제외
                combatTargetList.Add(target.gameObject);
                addedCount++;
                //Debug.Log($"[{gameObject.name}] Detecting: 타겟 추가 - {target.gameObject.name}");
            }

            //감지된 유닛이 자기 말고 더 있다면 전투 돌입
            if (combatTargetList.Count > 0) {
                Debug.Log($"[{gameObject.name}] Detecting: {addedCount}개 타겟 발견, Engaging 상태로 전환");
                ChangeState(CombatState.Engaging);
            }
            else {
                Debug.Log($"[{gameObject.name}] Detecting: 타겟 없음, Idle 상태로 전환");
                ChangeState(CombatState.Idle);
            }
        }

        // 조건 확인 상태: 최적 타겟 선택
        protected virtual void HandleEngagingState() {
            Debug.Log($"[{gameObject.name}] Engaging: 타겟 검증 시작, 현재 타겟 수: {combatTargetList.Count}");

            //교전 불가능한 적 제거 - 람다식을 쓸 수 밖에 없어서 람다식 사용
            combatTargetList.RemoveAll(target => !EngageConditionCheck(target, out UnitBase ub, out BattleBase bb)
                                    || !target.activeSelf);

            // 교전 상태에서 교전할 대상이 없다면 다시 탐지 상태로 전환
            if (combatTargetList.Count == 0) {
                Debug.Log($"[{gameObject.name}] Engaging: 유효한 타겟 없음, Detecting 상태로 전환");
                ChangeState(CombatState.Detecting);
                return;
            }
            else {
                Debug.Log($"[{gameObject.name}] Engaging: 유효한 타겟 {combatTargetList.Count}개, Moving 상태로 전환");
                Debug.Log($"[{gameObject.name}] Engaging: 선택된 타겟 - {combatTargetList[0].name}");
                ChangeState(CombatState.Moving);
            }
        }

        // 이동 상태: 전투 위치로 이동
        protected virtual void HandleMovingState() {
            if (combatTargetList.Count == 0) {
                Debug.Log($"[{gameObject.name}] Moving: 타겟 없음, Detecting 상태로 전환");
                ChangeState(CombatState.Detecting);
                return;
            }

            //null reference error 발생
            GameObject target = combatTargetList[0]; // 첫 번째 타겟으로 이동
            float targetDistance = ub.GetStat(StatType.CurrRange) * engageDistanceMultiplier;
            float currentDistance = Vector2.Distance(transform.position, target.transform.position);

            if (movementCoroutine == null) {
                Debug.Log($"[{gameObject.name}] Moving: {target.name}로 이동 시작, 목표 거리: {targetDistance:F2}, 현재 거리: {currentDistance:F2}");
                movementCoroutine = StartCoroutine(MoveToTarget(targetDistance, target));
            }

            // 이동이 완료되면 Fighting 상태로 전환
            if (Vector2.Distance(transform.position, target.transform.position) <= targetDistance + 0.01f) {
                Debug.Log($"[{gameObject.name}] Moving: 목표 거리 도달, Fighting 상태로 전환");
                if (movementCoroutine != null) {
                    StopCoroutine(movementCoroutine);
                    movementCoroutine = null;
                }
                ChangeState(CombatState.Fighting);
            }
        }

        // 전투 상태: 실제 공격 수행
        protected virtual void HandleFightingState() {
            if (combatTargetList.Count == 0) {
                Debug.Log($"[{gameObject.name}] Fighting: 타겟 없음, Detecting 상태로 전환");
                ChangeState(CombatState.Detecting);
                return;
            }

            GameObject target = combatTargetList[0]; // 첫 번째 타겟으로 공격
            if (target != null) {
                Debug.Log($"[{gameObject.name}] Fighting: {target.name} 공격 실행");
                Attack(target);
                ChangeState(CombatState.Detecting); // 공격 후 다시 이동 상태로 전환
            }
            else {
                Debug.Log($"[{gameObject.name}] Fighting: 타겟이 null, 제거 후 Detecting 상태로 전환");
                combatTargetList.RemoveAt(0); // 타겟이 null이면 제거
                ChangeState(CombatState.Detecting); // 타겟이 없으면 다시 탐지 상태로 전환
            }
        }

        protected virtual void HandleDeadState() {
            StopAllCoroutines();
            StopStateMachine();
            Die();
        }
        #endregion

        #region Custom Methods
        #region IBattle Methods
        public virtual void TakeDamage(float damage) {
            if (ub.IsDead) return;
            float newHealth = ub.GetStat(StatType.CurrHealth) - damage;
            Debug.Log($"[{gameObject.name}] {damage} 데미지 받음, 체력: {ub.GetStat(StatType.CurrHealth):F1} → {newHealth:F1}");
            ub.SetStat(StatType.CurrHealth, newHealth);
            if (newHealth <= 0 && !ub.IsDead) {
                ChangeState(CombatState.Dead);
            }
        }

        public virtual void HealHealth(float healAmount) {
            if (ub.IsDead) return;
            float oldHealth = ub.GetStat(StatType.CurrHealth);
            float newHealth = ub.GetStat(StatType.CurrHealth) + healAmount;
            newHealth = Mathf.Min(newHealth, ub.GetStat(StatType.BaseHealth));
            Debug.Log($"[{gameObject.name}] {healAmount} 회복, 체력: {oldHealth:F1} → {newHealth:F1}");
            ub.SetStat(StatType.CurrHealth, newHealth);
        }

        public virtual void Die() {
            if (ub.IsDead) return;
            Debug.Log($"[{gameObject.name}] 사망");
            ub.SetStat(StatType.CurrHealth, 0);
            // 사망 애니메이션 재생
            if (unitAnim != null) {
                StartCoroutine(unitAnim.PlayDeathAnim());
            }
        }

        public virtual bool IsInRange(UnitBase target) {
            if (ub.IsDead) return false;
            float distance = Vector3.Distance(transform.position, target.transform.position);
            bool inRange = (distance <= ub.GetStat(StatType.CurrRange)) && (combatTargetList.Count <= maxCombatTarget);
            Debug.Log($"{gameObject.name}의 {target.gameObject.name}까지의 거리: {distance:F2}, 사거리: {ub.GetStat(StatType.CurrRange):F2}, 범위 내: {inRange}");
            return inRange;
        }

        // 전투 조건 검사
        protected virtual bool EngageConditionCheck(GameObject target, out UnitBase targetUnit, out BattleBase targetBattleBase) {
            targetUnit = null; targetBattleBase = null;

            //null check
            if (target == null) {
                Debug.Log($"[{gameObject.name}] EngageCheck: 타겟이 null");
                return false;
            }
            if (!target.TryGetComponent<UnitBase>(out targetUnit)) {
                Debug.Log($"[{gameObject.name}] EngageCheck: {target.name}에 UnitBase 없음");
                return false;
            }
            if (!target.TryGetComponent<BattleBase>(out targetBattleBase)) {
                Debug.Log($"[{gameObject.name}] EngageCheck: {target.name}에 BattleBase 없음");
                return false;
            }
            //아군 유닛이거나 이미 죽은 유닛인 경우 전투할 수 없으므로 false
            if (targetUnit.GetFaction == ub.GetFaction) {
                Debug.Log($"[{gameObject.name}] EngageCheck: {target.name}은 아군");
                return false;
            }
            if (targetUnit.IsDead) {
                Debug.Log($"[{gameObject.name}] EngageCheck: {target.name}은 이미 사망");
                return false;
            }
            //대상이 이미 나와 전투 중이거나, 전투 대상이 최대치인 경우 전투할 수 없으므로 false
            /*if (combatTargetList.Contains(target)) {
                Debug.Log($"[{gameObject.name}] EngageCheck: {target.name}은 이미 전투 중");
                return false;
            }*/

            int targetIndex = combatTargetList.IndexOf(target);
            if (targetIndex > maxCombatTarget) {
                Debug.Log($"[{gameObject.name}] EngageCheck: 전투 대상 최대치 도달 ({maxCombatTarget})");
                return false;
            }

            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance > detectingRange) {
                Debug.Log($"[{gameObject.name}] EngageCheck: {target.name} 거리 초과, 거리: {distance:F2}, 탐지범위: {detectingRange:F2}");
                return false;
            }

            Debug.Log($"[{gameObject.name}] EngageCheck: {target.name} 전투 조건 만족");
            return true;
        }

        // 전투 돌입
        public virtual void Engage(GameObject target) {
            Debug.Log($"[{gameObject.name}] Engage 호출: {target?.name}");
            if (EngageConditionCheck(target, out UnitBase ub, out BattleBase bb)) {
                //대상과 전투하기 위해 이동
                Vector2 targetPosition = target.transform.position;
                float distance = Vector2.Distance(transform.position, targetPosition);
                float engageDistance = ub.GetStat(StatType.CurrRange) * engageDistanceMultiplier;
                Debug.Log($"[{gameObject.name}] Engage: {target.name}로 이동 시작, 거리: {distance:F2}, 목표: {engageDistance:F2}");
                //이동 코루틴 시작
                StartCoroutine(MoveToTarget(engageDistance, target));
            }
        }

        protected virtual IEnumerator MoveToTarget(float targetDistance, GameObject target) {
            //이동 애니메이션 재생
            if (unitAnim != null) {
                unitAnim.SetAnimBool(animParam.Param_bool_move, true);
            }
            Debug.Log($"[{gameObject.name}] MoveToTarget 코루틴 시작: {target.name}");
            while (target != null) {
                float distance = Vector2.Distance(transform.position, target.transform.position);
                if (distance <= targetDistance + 0.01f) {
                    Debug.Log($"[{gameObject.name}] MoveToTarget 완료: 목표 거리 {targetDistance:F2} 도달");
                    break;
                }

                //목표 거리까지 이동
                Vector2 direction = (target.transform.position - transform.position).normalized;
                transform.position += (Vector3)(direction * ub.GetStat(StatType.CurrSpd) * Time.deltaTime);
                yield return null;
            }
            Debug.Log($"[{gameObject.name}] MoveToTarget 코루틴 종료");
            //이동 애니메이션 중지
            if (unitAnim != null) {
                unitAnim.SetAnimBool(animParam.Param_bool_move, false);
            }
        }

        public virtual void Attack(GameObject target) {
            if (ub.IsDead) return;
            if (target == null) {
                Debug.LogError($"[{gameObject.name}] Attack: 공격 대상이 null입니다.");
                return;
            }
            //공격 애니메이션 재생
            if (unitAnim != null) {
                unitAnim.SetAnimTrigger(animParam.Param_trigger_attack);
            }
            if (target.TryGetComponent<IBattle>(out IBattle ib) && target.TryGetComponent<UnitBase>(out UnitBase ub_Method)) {
                Debug.Log($"[{gameObject.name}] Attack: {target.name} 공격 시도");
                // TODO: 공격 애니메이션 재생, 애니메이션이 끝나고 - 코루틴으로 처리 - 공격 효과 적용
                if (IsInRange(ub_Method)) {
                    float damage = ub.GetStat(StatType.CurrDamage);
                    //대미지 적용을 코루틴으로 변경
                    StartCoroutine(DamageCalc(target, damage));
                }
                else {
                    Debug.Log($"[{gameObject.name}] Attack: {target.name}이 사거리 밖");
                }
            }
            else {
                Debug.LogError($"[{gameObject.name}] Attack: {target.name}은 올바른 공격 대상이 아닙니다.");
            }
        }

        protected virtual IEnumerator DamageCalc(GameObject target, float damage) {
            //yield return으로 애니메이션의 특정 프레임까지 재생 대기
            UnitAnimFrameConfig currFrameInfo = unitAnim.GetAnimData();
            float waitTime = currFrameInfo.attackCompleteFrame / (float)currFrameInfo.frameRate;
            yield return new WaitForSeconds(waitTime);
            //null check는 상위 메서드에서 이미 했음
            if (target != null) target.GetComponent<IBattle>().TakeDamage(damage);
            //Debug.Log($"[{gameObject.name}] Attack: {target.name}에게 {damage} 데미지 가함");
        }

        //조건에 안맞는 적 삭제
        public virtual void RemoveInvalidTargets() {
            combatTargetList.RemoveAll(target => target == null 
                                              || !target.activeSelf 
                                              || !EngageConditionCheck(target, out UnitBase ub, out BattleBase bb));
        }
        #endregion
        #endregion
    }
}