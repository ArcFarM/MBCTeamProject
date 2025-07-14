using UnityEngine;

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

        void Update()
        {
            if (waypoints == null || waypoints.Length == 0) return;

            if (isLeader)
            {
                MoveAsLeader();
            }
            else
            {
                MoveAsFollower();
            }

            // 2D 게임이므로 회전 제거
        }

        void MoveAsLeader()
        {
            if (currentWaypointIndex >= waypoints.Length)
            {
                OnReachedDestination();
                return;
            }

            Vector3 targetPos = waypoints[currentWaypointIndex].position;
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

            // 이동
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

        void UpdateRotation()
        {
            // 2D 게임에서는 회전 제거 또는 Z축 회전만 사용
            Vector3 moveDirection = Vector3.zero;

            if (isLeader && currentWaypointIndex < waypoints.Length)
            {
                moveDirection = waypoints[currentWaypointIndex].position - transform.position;
            }
            else if (!isLeader && leaderReference != null)
            {
                moveDirection = (leaderReference.transform.position + formationOffset) - transform.position;
            }

            if (moveDirection != Vector3.zero)
            {
                // 2D 게임용 회전 (Z축만 사용)
                float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90); // 스프라이트가 위를 향하도록 -90도 조정
            }
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
                    Debug.Log($"적 파괴! 남은 적: {waveController.enemyCount - 1}");
                }
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
        }
    }
}