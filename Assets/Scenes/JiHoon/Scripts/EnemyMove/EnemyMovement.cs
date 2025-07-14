using MainGame.Units;
using UnityEngine;
using MainGame.Units.Battle;

namespace JiHoon
{
    public class EnemyMovement : MonoBehaviour
    {
        [Header("이동 설정")]
        public float moveSpeed = 3f;

        [Header("그룹 설정")]
        public float separationRadius = 1f;
        public float followDistance = 2f;

        // 경로 정보
        private Transform[] waypoints;
        private int currentWaypointIndex = 0;

        // 그룹 정보
        private bool isLeader = false;
        private EnemyGroup myGroup;
        private Vector3 formationOffset;
        private EnemyMovement leaderReference;

        // ★ 추가: 중복 처리 방지용 플래그 ★
        private bool hasReachedDestination = false;

        // ★ 추가: 전투 시스템 참조 ★
        private BattleBase battleBase;
        private bool isInCombat = false;

        void Start()
        {
            // BattleBase 컴포넌트 참조 가져오기
            battleBase = GetComponent<BattleBase>();
        }

        void Update()
        {
            if (waypoints == null || waypoints.Length == 0) return;

            // ★ 전투 중인지 체크 ★
            CheckCombatStatus();

            // 전투 중이면 이동 중지
            if (isInCombat)
            {
                return;
            }

            // 전투 중이 아닐 때만 이동
            if (isLeader)
            {
                MoveAsLeader();
            }
            else
            {
                MoveAsFollower();
            }
        }

        void CheckCombatStatus()
        {
            if (battleBase != null)
            {
                // BattleBase의 전투 타겟 리스트를 체크
                var combatTargets = battleBase.GetType().GetField("combatTargetList",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                if (combatTargets != null)
                {
                    var targetList = combatTargets.GetValue(battleBase) as System.Collections.Generic.List<GameObject>;

                    // ★ 전투 타겟이 있고, 그 타겟이 살아있으면 계속 전투 중 ★
                    if (targetList != null && targetList.Count > 0)
                    {
                        // 살아있는 적이 있는지 확인
                        bool hasLivingTarget = false;
                        foreach (var target in targetList)
                        {
                            if (target != null && target.activeSelf)
                            {
                                var targetUnitBase = target.GetComponent<MainGame.Units.UnitBase>();
                                if (targetUnitBase != null && !targetUnitBase.IsDead)
                                {
                                    hasLivingTarget = true;
                                    break;
                                }
                            }
                        }
                        isInCombat = hasLivingTarget;
                    }
                    else
                    {
                        isInCombat = false;
                    }
                }

                // currentState도 체크하되, Idle이나 Detecting이 아닌 경우만
                var stateField = battleBase.GetType().GetField("currentState",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                if (stateField != null)
                {
                    var currentState = stateField.GetValue(battleBase).ToString();
                    // Idle이나 Detecting 상태가 아니면 전투 중
                    if (currentState != "Idle" && currentState != "Detecting" && currentState != "Dead")
                    {
                        isInCombat = true;
                    }
                }
            }
        }

        void MoveAsLeader()
        {
            if (currentWaypointIndex >= waypoints.Length)
            {
                OnReachedDestination();
                return;
            }

            Vector3 targetPos = waypoints[currentWaypointIndex].position;

            // ★ 전투 중이 아닐 때만 실제 이동 ★
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // 웨이포인트 도달 체크
            if (Vector3.Distance(transform.position, targetPos) < 0.5f)
            {
                currentWaypointIndex++;
            }
        }

        void MoveAsFollower()
        {
            if (leaderReference == null) return;

            // 리더 기준 목표 위치
            Vector3 targetPos = leaderReference.transform.position + formationOffset;

            // 다른 유닛과의 분리
            Vector3 separation = CalculateSeparation();
            targetPos += separation;

            // ★ 전투 중이 아닐 때만 실제 이동 ★
            float speed = moveSpeed;
            float distToTarget = Vector3.Distance(transform.position, targetPos);

            // 너무 멀면 빠르게 따라잡기
            if (distToTarget > followDistance * 2)
            {
                speed = moveSpeed * 1.5f;
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            // 리더가 도착했으면 팔로워도 도착 처리
            if (leaderReference.currentWaypointIndex >= leaderReference.waypoints.Length)
            {
                OnReachedDestination();
            }
        }

        Vector3 CalculateSeparation()
        {
            Vector3 separationForce = Vector3.zero;
            int neighborCount = 0;

            if (myGroup != null)
            {
                foreach (var member in myGroup.GetMembers())
                {
                    if (member != this && member != null)
                    {
                        float distance = Vector3.Distance(transform.position, member.transform.position);
                        if (distance < separationRadius && distance > 0)
                        {
                            Vector3 diff = (transform.position - member.transform.position) / distance;
                            separationForce += diff;
                            neighborCount++;
                        }
                    }
                }
            }

            if (neighborCount > 0)
            {
                separationForce /= neighborCount;
                separationForce = separationForce.normalized * 0.5f;
            }

            return separationForce;
        }

        public void SetPath(Transform[] path)
        {
            waypoints = path;
            currentWaypointIndex = 0;
        }

        public void SetAsLeader()
        {
            isLeader = true;
            formationOffset = Vector3.zero;
        }

        public void SetAsFollower(Vector3 offset)
        {
            isLeader = false;
            formationOffset = offset;
        }

        public void SetGroup(EnemyGroup group, EnemyMovement leader = null)
        {
            myGroup = group;
            leaderReference = leader;
        }

        void OnReachedDestination()
        {
            // ★ 중복 호출 방지 ★
            if (hasReachedDestination) return;
            hasReachedDestination = true;

            // ★ WaveController에 적 제거 알림 ★
            var waveController = WaveController.Instance;
            if (waveController != null)
            {
                waveController.OnEnemyDeath();
                Debug.Log($"적 도착! 남은 적: {waveController.enemyCount - 1}");
            }

            if (myGroup != null)
            {
                myGroup.RemoveMember(this);
            }
            Destroy(gameObject);
        }

        void OnDestroy()
        {
            // ★ 도착이 아닌 다른 이유로 파괴될 때 (예: 플레이어가 처치) ★
            if (!hasReachedDestination)
            {
                var waveController = WaveController.Instance;
                if (waveController != null)
                {
                    waveController.OnEnemyDeath();
                    //Debug.Log($"적 파괴! 남은 적: {waveController.enemyCount - 1}");
                }
            }

            //적이 죽지않고 목표에 도달했을 때
            if (TryGetComponent<EnemyUnitBase>(out EnemyUnitBase ub)) {
                Debug.Log("유닛 패널티 전달");
                ub.GivePenalty();
            }

            if (myGroup != null)
            {
                myGroup.RemoveMember(this);
            }
        }

        // ★ 추가: 적이 공격받아 죽을 때 호출할 메서드 ★
        public void Die()
        {
            hasReachedDestination = true; // 중복 처리 방지

            var waveController = WaveController.Instance;
            if (waveController != null)
            {
                waveController.OnEnemyDeath();
                Debug.Log($"적 사망! 남은 적: {waveController.enemyCount - 1}");
            }

            if (myGroup != null)
            {
                myGroup.RemoveMember(this);
            }

            // 사망 이펙트나 애니메이션 재생 후 파괴
            Destroy(gameObject);
        }

        // ★ 추가: 전투 상태 설정을 위한 public 메서드 ★
        public void SetCombatStatus(bool inCombat)
        {
            isInCombat = inCombat;
        }

        void OnDrawGizmos()
        {
            if (isLeader)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, 0.3f);
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, 0.2f);
            }

            // 전투 중일 때 빨간색으로 표시
            if (isInCombat)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, 0.4f);
            }
        }
    }
}