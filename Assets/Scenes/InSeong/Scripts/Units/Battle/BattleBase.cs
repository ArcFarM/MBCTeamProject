using MainGame.Enum;
using System.Collections.Generic;
using MainGame.Units.Animation;
using UnityEngine;
using System.Collections;
using JiHoon;

namespace MainGame.Units.Battle {
    [RequireComponent(typeof(Rigidbody2D))]
    public class BattleBase : MonoBehaviour, IBattle {
        #region Variables
        [SerializeField] protected UnitBase ub;
        protected float detectingRange;
        [SerializeField] protected float detectingMultiplier = 1.5f;
        [SerializeField] protected float detectingRangeconstant = 2.5f;
        [SerializeField] protected float engageDistanceMultiplier = 0.75f;

        [Header("IBattle 구현")]
        [SerializeField] protected int maxCombatTarget;
        protected List<GameObject> attackers = new();
        [SerializeField] protected List<GameObject> combatTargetList = new();
        protected GameObject currentTarget = null;

        [Header("StateMachine")]
        [SerializeField] protected CombatState currentState = CombatState.Idle;
        [SerializeField] protected float stateUpdateInterval = 0.1f;
        protected Coroutine stateMachineCoroutine;

        [Header("컴포넌트 참조")]
        [SerializeField] protected UnitAnim unitAnim;
        protected AnimParam animParam = new AnimParam();
        private EnemyMovement enemyMovement;

        private Rigidbody2D rb;
        //공격 대기 시간
        bool isAttackCooldown = false;

        #endregion

        #region Properties
        public int GetCurrentAttackerCount => attackers.Count;
        public int GetMaxCombatTarget => maxCombatTarget;
        public GameObject GetFightingTarget => currentTarget;
        #endregion

        #region Unity Event Methods
        private void Awake() {
            TryGetComponent<EnemyMovement>(out enemyMovement);
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start() {
            float range = ub.GetStat(StatType.CurrRange);
            detectingRange = Mathf.Max(range * detectingMultiplier, range + detectingRangeconstant);
            StartStateMachine();
        }

        private void OnDestroy() {
            StopStateMachine();
        }

        private void FixedUpdate() {
            // 전투 상태(Moving)일 때의 이동만 여기서 담당
            if (currentState == CombatState.Moving && currentTarget != null && !ub.IsDead) {
                Vector2 direction = (currentTarget.transform.position - transform.position).normalized;
                Vector2 nextPosition = rb.position + direction * ub.GetStat(StatType.CurrSpd) * Time.fixedDeltaTime;
                rb.MovePosition(nextPosition);
            }
        }
        #endregion

        #region 상태 머신
        protected void StartStateMachine() {
            if (stateMachineCoroutine == null) {
                stateMachineCoroutine = StartCoroutine(StateMachineLoop());
            }
        }

        protected void StopStateMachine() {
            if (stateMachineCoroutine != null) {
                StopCoroutine(stateMachineCoroutine);
                stateMachineCoroutine = null;
            }
        }

        protected IEnumerator StateMachineLoop() {
            var updateInterval = new WaitForSeconds(stateUpdateInterval);
            ChangeState(CombatState.Idle); // 시작 상태를 명확히 설정

            while (!ub.IsDead) {
                UpdateComponentStatus();

                switch (currentState) {
                    case CombatState.Idle: HandleIdleState(); break;
                    case CombatState.Detecting: HandleDetectingState(); break;
                    case CombatState.Moving: HandleMovingState(); break;
                    case CombatState.Fighting: HandleFightingState(); break;
                    case CombatState.Dead: HandleDeadState(); yield break;
                }
                yield return updateInterval;
            }
        }

        private void UpdateComponentStatus() {
            if (enemyMovement == null) return;
            bool shouldDoWaypointMove = (currentState == CombatState.Idle || currentState == CombatState.Detecting) && currentTarget == null;
            if (enemyMovement.enabled != shouldDoWaypointMove) {
                enemyMovement.enabled = shouldDoWaypointMove;
            }
        }

        protected void ChangeState(CombatState newState) {
            if (currentState == newState) return;

            // 상태 변경 전 처리 (공격자 리스트에서 자신을 제거)
            if (currentTarget != null && (newState == CombatState.Detecting || newState == CombatState.Dead)) {
                if (currentTarget.TryGetComponent<BattleBase>(out var targetBattleBase)) {
                    targetBattleBase.RemoveAttacker(gameObject);
                }
            }

            currentState = newState;

            if (unitAnim != null) {
                bool isMoving = (currentState == CombatState.Moving) ||
                                (currentState == CombatState.Detecting && ub.GetFaction == UnitFaction.Enemy);

                // 저기에 해당 안하면 이동이 없는 상태
                unitAnim.SetAnimBool(animParam.Param_bool_move, isMoving);

            }
        }

        protected virtual void HandleIdleState() {
            if(ub.GetFaction == UnitFaction.Ally) transform.localScale = new Vector3(1, 1, 1); // 아군은 기본 스케일로 초기화
            else if (ub.GetFaction == UnitFaction.Enemy) transform.localScale = new Vector3(-1, 1, 1); // 적군은 반전된 스케일로 초기화
            ChangeState(CombatState.Detecting);
        }

        protected virtual void HandleDetectingState() {
            //비활성화 됐거나 죽은 대상 삭제
            if (currentTarget != null && (!currentTarget.activeSelf || currentTarget.GetComponent<UnitBase>().IsDead)) {
                currentTarget = null;
            }
            //이미 전투하기로 결정된 대상이 있으면 전투 돌입
            if (currentTarget != null) {
                ChangeState(CombatState.Moving);
                return;
            }

            FilterValidTargets(Physics2D.OverlapCircleAll(transform.position, detectingRange));
            GameObject potentialTarget = FindClosestTarget();

            if (potentialTarget != null) {
                currentTarget = potentialTarget;
                if (currentTarget.TryGetComponent<BattleBase>(out var targetBattleBase)) {
                    targetBattleBase.AddAttacker(gameObject);
                }
                //combatTargetList.Clear();
                ChangeState(CombatState.Moving);
            }
        }

        protected virtual void HandleMovingState() {
            if (currentTarget == null || !currentTarget.activeSelf || currentTarget.GetComponent<UnitBase>().IsDead) {
                ChangeState(CombatState.Detecting);
                return;
            }

            float currentDistance = Vector2.Distance(transform.position, currentTarget.transform.position);
            FlipSprite(currentTarget.transform.position.x);

            if (currentDistance > detectingRange) {
                ChangeState(CombatState.Detecting);
                return;
            }

            float engageDistance = currentTarget.GetComponent<UnitBase>().GetStat(StatType.CurrRange) * engageDistanceMultiplier;
            if (currentDistance <= engageDistance) {
                ChangeState(CombatState.Fighting);
            }
        }

        protected virtual void HandleFightingState() {
            rb.linearVelocity = Vector2.zero;

            if (currentTarget == null || !currentTarget.activeSelf || currentTarget.GetComponent<UnitBase>().IsDead) {
                ChangeState(CombatState.Detecting);
                return;
            }

            Attack(currentTarget);

            float engageDistance = currentTarget.GetComponent<UnitBase>().GetStat(StatType.CurrRange) * engageDistanceMultiplier;
            if (Vector2.Distance(transform.position, currentTarget.transform.position) > engageDistance) {
                ChangeState(CombatState.Moving);
            }
        }

        protected virtual void HandleDeadState() {
            StopAllCoroutines();
            Die();
        }
        #endregion

        #region Custom Methods
        private void FlipSprite(float xValue) {
            Vector3 scale = transform.localScale;
            //기본 스프라이트가 왼쪽을 보고 있음
            //대상이 나보다 오른쪽이면 오른 쪽 보고, 아니면 왼쪽 보게 하기
            scale.x = (xValue > transform.position.x ? -1 : 1);
            transform.localScale = scale;
        }

        private void FilterValidTargets(Collider2D[] colliders) {
            combatTargetList.Clear();
            foreach (var col in colliders) {
                if (col.gameObject == gameObject) continue;
                if (col.TryGetComponent<UnitBase>(out var targetUnit) && targetUnit.GetFaction != ub.GetFaction && !targetUnit.IsDead) {
                    if (col.TryGetComponent<BattleBase>(out var targetBattleBase) && targetBattleBase.GetCurrentAttackerCount >= targetBattleBase.GetMaxCombatTarget) {
                        continue;
                    }
                    combatTargetList.Add(col.gameObject);
                }
            }
        }

        private GameObject FindClosestTarget() {
            GameObject closest = null;
            float minDistance = float.MaxValue;
            foreach (var target in combatTargetList) {
                float distance = Vector2.Distance(transform.position, target.transform.position);
                if (distance < minDistance) {
                    minDistance = distance;
                    closest = target;
                }
            }
            return closest;
        }

        public void AddAttacker(GameObject attacker) {
            if (!attackers.Contains(attacker)) attackers.Add(attacker);
        }

        public void RemoveAttacker(GameObject attacker) {
            if (attackers.Contains(attacker)) attackers.Remove(attacker);
        }

        public virtual void TakeDamage(float damage) {
            if (ub.IsDead) return;
            float newHealth = ub.GetStat(StatType.CurrHealth) - damage;
            ub.SetStat(StatType.CurrHealth, newHealth);
            if (newHealth <= 0) ChangeState(CombatState.Dead);
        }

        public virtual void HealHealth(float healAmount) {
            if (ub.IsDead) return;
            float newHealth = Mathf.Min(ub.GetStat(StatType.CurrHealth) + healAmount, ub.GetStat(StatType.BaseHealth));
            ub.SetStat(StatType.CurrHealth, newHealth);
        }

        public virtual void Die() {
            if (ub.IsDead) return;
            ub.SetStat(StatType.CurrHealth, 0);
            if (enemyMovement != null) enemyMovement.enabled = false;
            ChangeState(CombatState.Dead);
            if (unitAnim != null) StartCoroutine(unitAnim.PlayDeathAnim());
            else Destroy(gameObject);
        }

        public virtual bool IsInRange(UnitBase target) {
            if (target == null) return false;
            return Vector2.Distance(transform.position, target.transform.position) <= ub.GetStat(StatType.CurrRange);
        }

        public virtual void Attack(GameObject target) {
            if(isAttackCooldown) return; // 공격 대기 시간 체크

            if (ub.IsDead || target == null) return;
            if (unitAnim != null) unitAnim.SetAnimTrigger(animParam.Param_trigger_attack);
            if (target.TryGetComponent<IBattle>(out var targetBattle)) {
                StartCoroutine(DamageCalc(targetBattle, ub.GetStat(StatType.CurrDamage)));
            }
        }

        protected virtual IEnumerator DamageCalc(IBattle target, float damage) {
            isAttackCooldown = true;

            if (unitAnim != null) {
                UnitAnimFrameConfig frameInfo = unitAnim.GetAnimData();
                yield return new WaitForSeconds(frameInfo.attackCompleteFrame / (float)frameInfo.frameRate);
            }
            else {
                yield return new WaitForSeconds(0.1f);
            }
            if (target != null) target.TakeDamage(damage);
            Debug.Log(ub.GetStat(StatType.CurrAtkSpd));
            yield return new WaitForSeconds(ub.GetStat(StatType.CurrAtkSpd));
            isAttackCooldown = false;
        }
        #endregion
    }
}