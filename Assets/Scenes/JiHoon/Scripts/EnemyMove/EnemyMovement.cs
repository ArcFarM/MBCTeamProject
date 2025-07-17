using UnityEngine;
using System.Collections.Generic;
using MainGame.Units;

namespace JiHoon {
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))] // 충돌 처리를 위해 Collider는 필수
    public class EnemyMovement : MonoBehaviour {
        [Header("이동 및 경로 설정")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float waypointArrivalRadius = 0.5f;

        [Header("뭉침 방지 설정")]
        [SerializeField] private float separationRadius = 1.2f;
        [SerializeField] private float separationForce = 1.5f;

        private Transform[] waypoints;
        private int currentWaypointIndex = 0;
        private bool hasReachedDestination = false;
        private EnemyGroup myGroup;

        private Rigidbody2D rb;

        private void Awake() {
            rb = GetComponent<Rigidbody2D>();
            // 이 방식에서는 Rigidbody의 물리 속성(Gravity, Drag 등)이 거의 영향을 주지 않습니다.
            // Body Type은 Dynamic 또는 Kinematic 둘 다 괜찮지만, 충돌 감지를 위해 Dynamic을 권장합니다.
            rb.gravityScale = 0;
        }

        private void OnDisable() {
            // 이동 로직이 멈추므로, 속도를 0으로 만들어 관성을 제거합니다.
            rb.linearVelocity = Vector2.zero;
        }

        private void FixedUpdate() {
            if (waypoints == null || waypoints.Length == 0 || hasReachedDestination) {
                return;
            }

            MoveAlongPath();
        }

        private void MoveAlongPath() {
            if (currentWaypointIndex >= waypoints.Length) {
                OnReachedDestination();
                return;
            }

            // 1. 목표(웨이포인트) 방향 계산
            Vector3 targetPos = waypoints[currentWaypointIndex].position;
            Vector2 directionToWaypoint = ((Vector2)targetPos - rb.position).normalized;

            // 2. 분리(Separation) 방향 계산
            Vector2 separationVector = CalculateSeparation();

            // 3. 두 방향을 합쳐 최종 이동 방향 결정
            // 목표 방향에 더 큰 가중치를 주어, 분리 때문에 목표를 잃지 않도록 합니다.
            Vector2 finalMoveDirection = (directionToWaypoint * 1.0f + separationVector * separationForce).normalized;

            // 4. 다음 프레임의 목표 위치를 계산
            // Time.fixedDeltaTime을 곱해 프레임 속도에 관계없이 일정한 속도로 이동하게 합니다.
            Vector2 nextPosition = rb.position + finalMoveDirection * moveSpeed * Time.fixedDeltaTime;

            // 5. Rigidbody.MovePosition으로 위치를 직접 업데이트
            // 이 함수는 물리 엔진의 충돌 감지를 존중하므로, 다른 유닛을 뚫고 가지 않습니다.
            rb.MovePosition(nextPosition);

            // 7. 도착 확인
            if (Vector2.Distance(rb.position, targetPos) < waypointArrivalRadius) {
                currentWaypointIndex++;
            }
        }

        private Vector2 CalculateSeparation() {
            if (myGroup == null) return Vector2.zero;

            Vector2 separationVector = Vector2.zero;
            List<EnemyMovement> members = myGroup.GetMembers();

            foreach (var member in members) {
                if (member == this || member == null) continue;

                float distance = Vector2.Distance(transform.position, member.transform.position);
                if (distance > 0 && distance < separationRadius) {
                    separationVector += (rb.position - (Vector2)member.transform.position).normalized;
                }
            }

            return separationVector;
        }

        /*private void FlipSprite(float directionX) {
            if (Mathf.Abs(directionX) > 0.05f) {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * (directionX > 0 ? 1 : -1);
                transform.localScale = scale;
            }
        }*/

        public void SetPath(Transform[] path) {
            waypoints = path;
            currentWaypointIndex = 0;
            hasReachedDestination = false;
        }

        public void SetGroup(EnemyGroup group) {
            myGroup = group;
        }

        private void OnReachedDestination() {
            if (hasReachedDestination) return;
            hasReachedDestination = true;

            var waveController = WaveController.Instance;
            if (waveController != null) waveController.OnEnemyDeath();
            if (TryGetComponent<EnemyUnitBase>(out var enemyUnit)) enemyUnit.GivePenalty();
            Destroy(gameObject);
        }

        private void OnDestroy() {
            if (myGroup != null) myGroup.RemoveMember(this);
            if (!hasReachedDestination) {
                var waveController = WaveController.Instance;
                if (waveController != null) waveController.OnEnemyDeath();
            }
        }

        // (OnDrawGizmosSelected 코드는 이전과 동일하게 사용하셔도 좋습니다)
    }
}