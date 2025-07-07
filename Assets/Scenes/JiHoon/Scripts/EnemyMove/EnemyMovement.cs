using UnityEngine;
namespace JiHoon
{
    public class EnemyMovement : MonoBehaviour
    {
        [Header("이동 세팅")]
        public float moveSpeed = 3f;
        public float nodeReachDistance = 0.1f;

        [Header("그룹 이동 설정")]
        public float followDistance = 2f;       // 리더 따라가기 거리
        public float separationRadius = 1f;     // 충돌 방지 반경
        public float maxFollowSpeed = 5f;       // 팔로워 최대 속도

        [Header("이동경로 세팅")]
        public Transform startPoint;
        public Transform endPoint;
        public Transform[] viaPoints;

        private Transform[] fullPath;
        public int currentTargetIndex = 0;
        public bool isMoving = false;

        // 그룹 관련 변수
        private EnemyGroup enemyGroup;
        private bool isLeader = false;
        private Vector3 formationOffset = Vector3.zero;
        private Vector3 targetPosition;

        void Start()
        {
            SetupPath();
            isMoving = true;
        }

        void SetupPath()
        {
            int totalLen = 2 + (viaPoints != null ? viaPoints.Length : 0);
            fullPath = new Transform[totalLen];
            fullPath[0] = startPoint;
            if (viaPoints != null && viaPoints.Length > 0)
                viaPoints.CopyTo(fullPath, 1);
            fullPath[totalLen - 1] = endPoint;

            if (fullPath[0] != null)
                transform.position = fullPath[0].position;
            currentTargetIndex = 1;
        }

        void Update()
        {
            if (!isMoving || fullPath == null) return;

            if (isLeader)
            {
                UpdateLeaderMovement();
            }
            else
            {
                UpdateFollowerMovement();
            }
        }

        void UpdateLeaderMovement()
        {
            if (currentTargetIndex >= fullPath.Length) return;

            Transform target = fullPath[currentTargetIndex];
            if (target == null) return;

            // 리더는 기존 웨이포인트 시스템 그대로
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, target.position) < nodeReachDistance)
            {
                currentTargetIndex++;
                if (currentTargetIndex >= fullPath.Length)
                {
                    isMoving = false;
                    OnReachedDestination();
                }
            }
        }

        void UpdateFollowerMovement()
        {
            if (enemyGroup == null || enemyGroup.leader == null) return;

            Vector3 leaderPos = enemyGroup.GetLeaderPosition();
            Vector3 desiredPos = leaderPos + formationOffset;

            // 분리 행동 (다른 적들과 겹치지 않게)
            Vector3 separationForce = CalculateSeparation();
            desiredPos += separationForce;

            // 리더와 너무 멀어지면 빠르게 따라잡기
            float distanceToLeader = Vector3.Distance(transform.position, leaderPos);
            float currentSpeed = moveSpeed;

            if (distanceToLeader > followDistance)
            {
                currentSpeed = Mathf.Lerp(moveSpeed, maxFollowSpeed,
                    (distanceToLeader - followDistance) / followDistance);
            }

            // 목표 위치로 이동
            transform.position = Vector3.MoveTowards(
                transform.position,
                desiredPos,
                currentSpeed * Time.deltaTime
            );

            // 리더의 진행도 동기화
            currentTargetIndex = enemyGroup.leader.currentTargetIndex;

            // 리더가 도착했으면 팔로워도 도착
            if (!enemyGroup.leader.isMoving)
            {
                isMoving = false;
                OnReachedDestination();
            }
        }

        Vector3 CalculateSeparation()
        {
            Vector3 separationForce = Vector3.zero;
            int count = 0;

            if (enemyGroup != null)
            {
                foreach (var member in enemyGroup.members)
                {
                    if (member != this && member != null)
                    {
                        float distance = Vector3.Distance(transform.position, member.transform.position);
                        if (distance < separationRadius && distance > 0)
                        {
                            Vector3 diff = (transform.position - member.transform.position).normalized;
                            diff /= distance; // 거리에 반비례
                            separationForce += diff;
                            count++;
                        }
                    }
                }
            }

            if (count > 0)
            {
                separationForce /= count;
                separationForce = separationForce.normalized * 0.5f; // 강도 조절
            }

            return separationForce;
        }

        public void SetGroup(EnemyGroup group)
        {
            enemyGroup = group;
        }

        public void SetAsLeader()
        {
            isLeader = true;
            formationOffset = Vector3.zero;
        }

        public void SetAsFollower()
        {
            isLeader = false;
        }

        public void SetFormationOffset(Vector3 offset)
        {
            formationOffset = offset;
        }

        void OnReachedDestination()
        {
            Debug.Log($"{gameObject.name} 목표 도착");

            // 그룹에서 제거
            if (enemyGroup != null)
            {
                enemyGroup.RemoveMember(this);
            }

            Destroy(gameObject);
        }

        void OnDestroy()
        {
            // 그룹에서 제거
            if (enemyGroup != null)
            {
                enemyGroup.RemoveMember(this);
            }
        }

        void OnDrawGizmos()
        {
            // 기존 웨이포인트 그리기
            if (startPoint == null || endPoint == null)
                return;

            Gizmos.color = Color.green;
            Vector3 prev = startPoint.position;

            if (viaPoints != null && viaPoints.Length > 0)
            {
                foreach (var v in viaPoints)
                {
                    if (v != null)
                    {
                        Gizmos.DrawLine(prev, v.position);
                        prev = v.position;
                    }
                }
            }

            Gizmos.DrawLine(prev, endPoint.position);

            // 그룹 관련 기즈모
            if (isLeader)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, 0.3f);
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, 0.2f);

                // 대형 위치 표시
                if (enemyGroup != null && enemyGroup.leader != null)
                {
                    Vector3 formationPos = enemyGroup.GetLeaderPosition() + formationOffset;
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireCube(formationPos, Vector3.one * 0.2f);
                    Gizmos.DrawLine(transform.position, formationPos);
                }
            }
        }
    }
}