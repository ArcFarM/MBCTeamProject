using MainGame.Enum;
using System.Collections.Generic;
using MainGame.Units.Animation;
using UnityEngine;
using System.Collections;
using JiHoon;

namespace MainGame.Units.Battle
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BattleBase : MonoBehaviour, IBattle
    {
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

        // ★ 추가: 전역 타겟 관리 (이미 공격받고 있는 유닛 추적) ★
        private static Dictionary<GameObject, GameObject> globalTargetedUnits = new Dictionary<GameObject, GameObject>();
        private GameObject lockedTarget = null;

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
        private void Awake()
        {
            TryGetComponent<EnemyMovement>(out enemyMovement);
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            float range = ub.GetStat(StatType.CurrRange);
            detectingRange = Mathf.Max(range * detectingMultiplier, range + detectingRangeconstant);
            StartStateMachine();
        }

        private void OnDestroy()
        {
            StopStateMachine();
            // ★ 파괴 시 타겟 관리에서 제거 ★
            ReleaseAllTargets();
        }

        private void FixedUpdate()
        {
            // 전투 상태(Moving)일 때의 이동만 여기서 담당
            if (currentState == CombatState.Moving && currentTarget != null && !ub.IsDead)
            {
                Vector2 direction = (currentTarget.transform.position - transform.position).normalized;
                Vector2 nextPosition = rb.position + direction * ub.GetStat(StatType.CurrSpd) * Time.fixedDeltaTime;
                rb.MovePosition(nextPosition);
            }
        }
        #endregion

        #region 상태 머신
        protected void StartStateMachine()
        {
            if (stateMachineCoroutine == null)
            {
                stateMachineCoroutine = StartCoroutine(StateMachineLoop());
            }
        }

        protected void StopStateMachine()
        {
            if (stateMachineCoroutine != null)
            {
                StopCoroutine(stateMachineCoroutine);
                stateMachineCoroutine = null;
            }
        }

        protected IEnumerator StateMachineLoop()
        {
            var updateInterval = new WaitForSeconds(stateUpdateInterval);
            ChangeState(CombatState.Idle); // 시작 상태를 명확히 설정

            while (!ub.IsDead)
            {
                UpdateComponentStatus();

                switch (currentState)
                {
                    case CombatState.Idle: HandleIdleState(); break;
                    case CombatState.Detecting: HandleDetectingState(); break;
                    case CombatState.Moving: HandleMovingState(); break;
                    case CombatState.Fighting: HandleFightingState(); break;
                    case CombatState.Dead: HandleDeadState(); yield break;
                }
                yield return updateInterval;
            }
        }

        private void UpdateComponentStatus()
        {
            if (enemyMovement == null) return;
            bool shouldDoWaypointMove = (currentState == CombatState.Idle || currentState == CombatState.Detecting) && currentTarget == null;
            if (enemyMovement.enabled != shouldDoWaypointMove)
            {
                enemyMovement.enabled = shouldDoWaypointMove;
            }
        }

        protected void ChangeState(CombatState newState)
        {
            if (currentState == newState) return;

            // 상태 변경 전 처리 (공격자 리스트에서 자신을 제거)
            if (currentTarget != null && (newState == CombatState.Detecting || newState == CombatState.Dead))
            {
                if (currentTarget.TryGetComponent<BattleBase>(out var targetBattleBase))
                {
                    targetBattleBase.RemoveAttacker(gameObject);
                }
                // ★ 타겟 관리 추가 ★
                if (lockedTarget == null || !lockedTarget.activeSelf || lockedTarget.GetComponent<UnitBase>().IsDead)
                {
                    ReleaseAllTargets();
                    lockedTarget = null;
                }
            }

            currentState = newState;

            if (unitAnim != null)
            {
                bool isMoving = (currentState == CombatState.Moving) ||
                                (currentState == CombatState.Detecting && ub.GetFaction == UnitFaction.Enemy);

                // 저기에 해당 안하면 이동이 없는 상태
                unitAnim.SetAnimBool(animParam.Param_bool_move, isMoving);

            }
        }

        protected virtual void HandleIdleState()
        {
            if (ub.GetFaction == UnitFaction.Ally) transform.localScale = new Vector3(1, 1, 1); // 아군은 기본 스케일로 초기화
            else if (ub.GetFaction == UnitFaction.Enemy) transform.localScale = new Vector3(-1, 1, 1); // 적군은 반전된 스케일로 초기화

            // ★ 적군은 더 자주 탐지 시도 ★
            if (!ub.IsDead && combatTargetList.Count < maxCombatTarget)
            {
                if (ub.GetFaction == UnitFaction.Enemy)
                {
                    // 적군은 항상 탐지 시도
                    ChangeState(CombatState.Detecting);
                }
                else
                {
                    // 아군은 0.5초마다 탐지
                    if (Time.time % 0.5f < stateUpdateInterval)
                    {
                        ChangeState(CombatState.Detecting);
                    }
                }
            }

            // ★ 적군이 Idle 상태에 오래 머물면 강제로 이동 활성화 ★
            if (ub.GetFaction == UnitFaction.Enemy && enemyMovement != null)
            {
                if (!enemyMovement.enabled)
                {
                    enemyMovement.enabled = true;
                    Debug.Log($"[{gameObject.name}] Idle 상태에서 이동 강제 활성화");
                }
            }
        }

        protected virtual void HandleDetectingState()
        {
            //비활성화 됐거나 죽은 대상 삭제
            if (currentTarget != null && (!currentTarget.activeSelf || currentTarget.GetComponent<UnitBase>().IsDead))
            {
                currentTarget = null;
            }

            // ★ 이미 고정된 타겟이 있고 유효하면 바로 Moving으로 ★
            if (lockedTarget != null && lockedTarget.activeSelf && !lockedTarget.GetComponent<UnitBase>().IsDead)
            {
                currentTarget = lockedTarget;
                if (!combatTargetList.Contains(lockedTarget))
                {
                    combatTargetList.Add(lockedTarget);
                }
                ChangeState(CombatState.Moving);
                return;
            }

            //이미 전투하기로 결정된 대상이 있으면 전투 돌입
            if (currentTarget != null)
            {
                ChangeState(CombatState.Moving);
                return;
            }

            // ★ 아군인 경우: 나를 공격하는 적이 있는지 먼저 확인 ★
            if (ub.GetFaction == UnitFaction.Ally)
            {
                GameObject attacker = FindAttackingEnemy();
                if (attacker != null)
                {
                    combatTargetList.Clear();
                    combatTargetList.Add(attacker);
                    currentTarget = attacker;
                    lockedTarget = attacker;
                    RegisterTarget(attacker);
                    Debug.Log($"[{gameObject.name}] 나를 공격하는 {attacker.name}을(를) 즉시 반격!");
                    ChangeState(CombatState.Moving);
                    return;
                }
            }

            FilterValidTargets(Physics2D.OverlapCircleAll(transform.position, detectingRange));
            GameObject potentialTarget = FindClosestTarget();

            if (potentialTarget != null)
            {
                currentTarget = potentialTarget;
                lockedTarget = potentialTarget;
                RegisterTarget(potentialTarget);
                if (currentTarget.TryGetComponent<BattleBase>(out var targetBattleBase))
                {
                    targetBattleBase.AddAttacker(gameObject);
                }
                ChangeState(CombatState.Moving);
            }
            else
            {
                // ★ 적군이 타겟을 못 찾으면 계속 이동하도록 ★
                if (ub.GetFaction == UnitFaction.Enemy)
                {
                    StartCoroutine(RetryDetectionAfterDelay(0.5f));
                }
            }
        }

        protected virtual void HandleMovingState()
        {
            if (currentTarget == null || !currentTarget.activeSelf || currentTarget.GetComponent<UnitBase>().IsDead)
            {
                currentTarget = null;
                lockedTarget = null;
                ChangeState(CombatState.Detecting);
                return;
            }

            // ★ 같은 진영을 추적 중이면 즉시 포기 ★
            var myUnit = GetComponent<UnitBase>();
            var targetUnit = currentTarget.GetComponent<UnitBase>();
            if (myUnit != null && targetUnit != null && myUnit.GetFaction == targetUnit.GetFaction)
            {
                Debug.Log($"[{gameObject.name}] 같은 진영 타겟 감지, 타겟 해제");
                ReleaseTarget(currentTarget);
                currentTarget = null;
                lockedTarget = null;
                combatTargetList.Clear();
                ChangeState(CombatState.Detecting);
                return;
            }

            float currentDistance = Vector2.Distance(transform.position, currentTarget.transform.position);
            FlipSprite(currentTarget.transform.position.x);

            if (currentDistance > detectingRange)
            {
                ChangeState(CombatState.Detecting);
                return;
            }

            float engageDistance = currentTarget.GetComponent<UnitBase>().GetStat(StatType.CurrRange) * engageDistanceMultiplier;
            if (currentDistance <= engageDistance)
            {
                ChangeState(CombatState.Fighting);
            }
        }

        protected virtual void HandleFightingState()
        {
            rb.linearVelocity = Vector2.zero;

            if (currentTarget == null || !currentTarget.activeSelf || currentTarget.GetComponent<UnitBase>().IsDead)
            {
                currentTarget = null;
                lockedTarget = null;
                ChangeState(CombatState.Detecting);
                return;
            }

            Attack(currentTarget);

            // ★ 타겟이 죽었는지 확인 ★
            if (currentTarget.GetComponent<UnitBase>().IsDead)
            {
                ReleaseTarget(currentTarget);
                currentTarget = null;
                lockedTarget = null;
                combatTargetList.Clear();
                ChangeState(CombatState.Detecting);
                return;
            }

            float engageDistance = currentTarget.GetComponent<UnitBase>().GetStat(StatType.CurrRange) * engageDistanceMultiplier;
            if (Vector2.Distance(transform.position, currentTarget.transform.position) > engageDistance)
            {
                ChangeState(CombatState.Moving);
            }
        }

        protected virtual void HandleDeadState()
        {
            ReleaseAllTargets();
            StopAllCoroutines();
            Die();
        }
        #endregion

        #region 타겟 관리 메서드
        // ★ 타겟이 이미 다른 적에게 공격받고 있는지 확인 ★
        private bool IsAlreadyTargeted(GameObject target)
        {
            return globalTargetedUnits.ContainsKey(target) && globalTargetedUnits[target] != gameObject;
        }

        // ★ 타겟을 공격 중으로 등록 ★
        private void RegisterTarget(GameObject target)
        {
            if (target != null)
            {
                globalTargetedUnits[target] = gameObject;
                Debug.Log($"[{gameObject.name}] {target.name}을 공격 타겟으로 등록");
            }
        }

        // ★ 타겟 공격 해제 ★
        private void ReleaseTarget(GameObject target)
        {
            if (target != null && globalTargetedUnits.ContainsKey(target) && globalTargetedUnits[target] == gameObject)
            {
                globalTargetedUnits.Remove(target);
                Debug.Log($"[{gameObject.name}] {target.name}을 타겟에서 해제");
            }
        }

        // ★ 모든 타겟 해제 ★
        private void ReleaseAllTargets()
        {
            foreach (var target in combatTargetList)
            {
                ReleaseTarget(target);
            }
            if (currentTarget != null)
            {
                ReleaseTarget(currentTarget);
            }
            if (lockedTarget != null)
            {
                ReleaseTarget(lockedTarget);
            }
        }

        // ★ 나를 공격하는 적 찾기 ★
        private GameObject FindAttackingEnemy()
        {
            Collider2D[] nearbyUnits = Physics2D.OverlapCircleAll(transform.position, detectingRange * 1.5f);
            List<GameObject> attackingMe = new List<GameObject>();

            foreach (var unit in nearbyUnits)
            {
                if (unit.gameObject == gameObject) continue;

                var otherBattleBase = unit.GetComponent<BattleBase>();
                var otherUnitBase = unit.GetComponent<UnitBase>();

                if (otherBattleBase != null && otherUnitBase != null && otherUnitBase.GetFaction == UnitFaction.Enemy)
                {
                    // 상대의 currentTarget이 나인지 확인
                    if (otherBattleBase.currentTarget == gameObject || otherBattleBase.lockedTarget == gameObject)
                    {
                        attackingMe.Add(unit.gameObject);
                    }
                }
            }

            // 가장 가까운 공격자 반환
            GameObject nearestAttacker = null;
            float nearestDistance = float.MaxValue;
            foreach (var attacker in attackingMe)
            {
                float distance = Vector2.Distance(transform.position, attacker.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestAttacker = attacker;
                }
            }

            return nearestAttacker;
        }

        // ★ 탐지 재시도 코루틴 추가 ★
        private IEnumerator RetryDetectionAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (currentState == CombatState.Idle && combatTargetList.Count == 0)
            {
                ChangeState(CombatState.Detecting);
            }
        }
        #endregion

        #region Custom Methods
        private void FlipSprite(float xValue)
        {
            Vector3 scale = transform.localScale;
            //기본 스프라이트가 왼쪽을 보고 있음
            //대상이 나보다 오른쪽이면 오른 쪽 보고, 아니면 왼쪽 보게 하기
            scale.x = (xValue > transform.position.x ? -1 : 1);
            transform.localScale = scale;
        }

        private void FilterValidTargets(Collider2D[] colliders)
        {
            combatTargetList.Clear();
            foreach (var col in colliders)
            {
                if (col.gameObject == gameObject) continue;
                if (col.TryGetComponent<UnitBase>(out var targetUnit) && targetUnit.GetFaction != ub.GetFaction && !targetUnit.IsDead)
                {
                    if (col.TryGetComponent<BattleBase>(out var targetBattleBase) && targetBattleBase.GetCurrentAttackerCount >= targetBattleBase.GetMaxCombatTarget)
                    {
                        continue;
                    }
                    // ★ 적군은 이미 다른 적이 공격 중인 아군은 무시 ★
                    if (ub.GetFaction == UnitFaction.Enemy && IsAlreadyTargeted(col.gameObject))
                    {
                        Debug.Log($"[{gameObject.name}] {col.gameObject.name}은 이미 다른 적이 공격 중, 무시");
                        continue;
                    }
                    combatTargetList.Add(col.gameObject);
                }
            }
        }

        private GameObject FindClosestTarget()
        {
            GameObject closest = null;
            float minDistance = float.MaxValue;
            foreach (var target in combatTargetList)
            {
                float distance = Vector2.Distance(transform.position, target.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = target;
                }
            }
            return closest;
        }

        public void AddAttacker(GameObject attacker)
        {
            if (!attackers.Contains(attacker)) attackers.Add(attacker);
        }

        public void RemoveAttacker(GameObject attacker)
        {
            if (attackers.Contains(attacker)) attackers.Remove(attacker);
        }

        public virtual void TakeDamage(float damage)
        {
            if (ub.IsDead) return;
            float newHealth = ub.GetStat(StatType.CurrHealth) - damage;
            ub.SetStat(StatType.CurrHealth, newHealth);
            if (newHealth <= 0) ChangeState(CombatState.Dead);
        }

        public virtual void HealHealth(float healAmount)
        {
            if (ub.IsDead) return;
            float newHealth = Mathf.Min(ub.GetStat(StatType.CurrHealth) + healAmount, ub.GetStat(StatType.BaseHealth));
            ub.SetStat(StatType.CurrHealth, newHealth);
        }

        public virtual void Die()
        {
            if (ub.IsDead) return;
            ub.SetStat(StatType.CurrHealth, 0);
            if (enemyMovement != null) enemyMovement.enabled = false;
            ChangeState(CombatState.Dead);
            if (unitAnim != null) StartCoroutine(unitAnim.PlayDeathAnim());
            else Destroy(gameObject);
        }

        public virtual bool IsInRange(UnitBase target)
        {
            if (target == null) return false;
            return Vector2.Distance(transform.position, target.transform.position) <= ub.GetStat(StatType.CurrRange);
        }

        public virtual void Attack(GameObject target)
        {
            if (isAttackCooldown) return; // 공격 대기 시간 체크

            if (ub.IsDead || target == null) return;
            if (unitAnim != null) unitAnim.SetAnimTrigger(animParam.Param_trigger_attack);
            if (target.TryGetComponent<IBattle>(out var targetBattle))
            {
                StartCoroutine(DamageCalc(targetBattle, ub.GetStat(StatType.CurrDamage)));
            }
        }

        protected virtual IEnumerator DamageCalc(IBattle target, float damage)
        {
            isAttackCooldown = true;

            if (unitAnim != null)
            {
                UnitAnimFrameConfig frameInfo = unitAnim.GetAnimData();
                yield return new WaitForSeconds(frameInfo.attackCompleteFrame / (float)frameInfo.frameRate);
            }
            else
            {
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