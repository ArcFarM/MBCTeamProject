using MainGame.Units;
using UnityEngine;
using MainGame.Units.Battle;

namespace JiHoon
{
    public class EnemyMovement : MonoBehaviour
    {
        [Header("이동 설정")]
        public float moveSpeed = 3f;
        public float moveStartDelay = 0.5f; // 이동 시작 전 대기 시간

        // 경로 정보
        private Transform[] waypoints;
        private int currentWaypointIndex = 0;

        // 그룹 정보 (스폰 시에만 사용)
        private EnemyGroup myGroup;
        private float currentDelay = 0f;
        private bool hasStartedMoving = false;

        // 중복 처리 방지용 플래그
        private bool hasReachedDestination = false;

        // 전투 시스템 참조
        private BattleBase battleBase;
        private bool isInCombat = false;

        void Start()
        {
            // BattleBase 컴포넌트 참조 가져오기
            battleBase = GetComponent<BattleBase>();

            // 각 유닛마다 약간 다른 딜레이를 주어 자연스러운 움직임
            currentDelay = moveStartDelay + Random.Range(0f, 0.2f);
        }

        void Update()
        {
            if (waypoints == null || waypoints.Length == 0) return;

            // 이동 시작 전 대기
            if (!hasStartedMoving)
            {
                currentDelay -= Time.deltaTime;
                if (currentDelay <= 0)
                {
                    hasStartedMoving = true;
                }
                return;
            }

            // 전투 중인지 체크
            CheckCombatStatus();

            // 전투 중이면 이동 중지 - 완전히 멈춤
            if (isInCombat)
            {
                // 전투 중에는 아무것도 하지 않음
                return;
            }

            // 전투가 끝났을 때만 독립적으로 이동
            MoveIndependently();
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

                    // 전투 타겟이 있고, 그 타겟이 살아있으면 계속 전투 중
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

                // currentState도 체크
                var stateField = battleBase.GetType().GetField("currentState",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                if (stateField != null)
                {
                    var currentState = stateField.GetValue(battleBase).ToString();
                    // Idle, Detecting, Dead가 아닌 모든 상태를 전투 중으로 간주
                    if (currentState == "Engaging" || currentState == "Moving" || currentState == "Fighting")
                    {
                        isInCombat = true;
                    }
                }
            }
        }

        void MoveIndependently()
        {
            if (currentWaypointIndex >= waypoints.Length)
            {
                OnReachedDestination();
                return;
            }

            Vector3 targetPos = waypoints[currentWaypointIndex].position;
            Vector3 moveDirection = (targetPos - transform.position).normalized;

            // ★ 전방에 장애물(전투 중인 유닛) 체크 ★
            RaycastHit2D hit = Physics2D.CircleCast(
                transform.position,
                0.5f, // 감지 반경
                moveDirection,
                1f, // 감지 거리
                LayerMask.GetMask("Unit") // Unit 레이어만 체크
            );

            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                // 장애물이 전투 중인지 확인
                var otherBattleBase = hit.collider.GetComponent<MainGame.Units.Battle.BattleBase>();
                if (otherBattleBase != null)
                {
                    var stateField = otherBattleBase.GetType().GetField("currentState",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

                    if (stateField != null)
                    {
                        var state = stateField.GetValue(otherBattleBase).ToString();

                        // 전투 중인 유닛이면 우회
                        if (state == "Fighting" || state == "Moving" || state == "Engaging")
                        {
                            // 왼쪽 또는 오른쪽으로 우회
                            Vector3 avoidDirection = Vector3.Cross(Vector3.forward, moveDirection);

                            // 왼쪽이 막혔으면 오른쪽으로
                            RaycastHit2D leftCheck = Physics2D.Raycast(transform.position, -avoidDirection, 1f);
                            if (leftCheck.collider != null)
                            {
                                avoidDirection = -avoidDirection;
                            }

                            // 우회 이동
                            transform.position += avoidDirection * moveSpeed * 0.7f * Time.deltaTime;
                        }
                    }
                }
            }

            // 목표 지점으로 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // 웨이포인트 도달 체크
            if (Vector3.Distance(transform.position, targetPos) < 0.5f)
            {
                currentWaypointIndex++;
            }
        }

        public void SetPath(Transform[] path)
        {
            waypoints = path;
            currentWaypointIndex = 0;
        }

        // 기존 메서드들은 호환성을 위해 유지하되, 실제로는 사용하지 않음
        public void SetAsLeader()
        {
            // 리더 설정은 무시하고 딜레이만 약간 짧게
            currentDelay = moveStartDelay * 1f;
        }

        public void SetAsFollower(Vector3 offset)
        {
            // 팔로워도 독립적으로 이동, 딜레이만 약간 길게
            currentDelay = moveStartDelay * 1.2f;
        }

        public void SetGroup(EnemyGroup group, EnemyMovement leader = null)
        {
            myGroup = group;
            // 리더 참조는 무시
        }

        void OnReachedDestination()
        {
            // 중복 호출 방지
            if (hasReachedDestination) return;
            hasReachedDestination = true;

            // WaveController에 적 제거 알림
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
            if (myGroup != null)
            {
                myGroup.RemoveMember(this);
            }

            // 도착이 아닌 다른 이유로 파괴될 때 (예: 플레이어가 처치)
            if (!hasReachedDestination)
            {
                var waveController = WaveController.Instance;
                if (waveController != null)
                {
                    waveController.OnEnemyDeath();
                }
                return;
            }

            // 적이 죽지않고 목표에 도달했을 때
            if (TryGetComponent<EnemyUnitBase>(out EnemyUnitBase ub))
            {
                Debug.Log("유닛 패널티 전달");
                ub.GivePenalty();
            }
        }

        // 적이 공격받아 죽을 때 호출할 메서드
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

        // 전투 상태 설정을 위한 public 메서드
        public void SetCombatStatus(bool inCombat)
        {
            isInCombat = inCombat;
        }

        void OnDrawGizmos()
        {
            // 이동 시작 전
            if (!hasStartedMoving)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawWireSphere(transform.position, 0.2f);
            }
            // 전투 중일 때
            else if (isInCombat)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, 0.4f);
                // 전투 중임을 표시하는 X 표시
                Vector3 pos = transform.position;
                Gizmos.DrawLine(pos + Vector3.left * 0.3f + Vector3.up * 0.3f,
                               pos + Vector3.right * 0.3f + Vector3.down * 0.3f);
                Gizmos.DrawLine(pos + Vector3.left * 0.3f + Vector3.down * 0.3f,
                               pos + Vector3.right * 0.3f + Vector3.up * 0.3f);
            }
            // 이동 중
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, 0.25f);
            }

            // 현재 목표 웨이포인트로의 선 표시 (전투 중이 아닐 때만)
            if (hasStartedMoving && !isInCombat && waypoints != null && currentWaypointIndex < waypoints.Length)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, waypoints[currentWaypointIndex].position);
            }
        }
    }
}