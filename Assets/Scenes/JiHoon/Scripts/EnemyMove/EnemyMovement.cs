using UnityEngine;
namespace JiHoon
{
    public class EnemyMovement : MonoBehaviour
    {
        [Header("이동 세팅")]
        public float moveSpeed = 3f;    //이동속도
        public float nodeReachDistance = 0.1f;  // 노드 도달 거리

        [Header("이동경로 세팅")]
        public Transform startPoint;    // 시작점
        public Transform endPoint;  //도착점
        public Transform[] viaPoints; // 중간 경유지들(웨이포인트)

        private Transform[] fullPath; // 실제 이동 경로 (시작+경유+도착 합쳐서)

        private int currentTargetIndex = 0; //현재 도착해야하는 웨이포인트 인덱스
        public bool isMoving = false;   //이동 중인지 여부

        void Start()
        {
            // 경로 합치기: Start → (Via...) → End
            int totalLen = 2 + (viaPoints != null ? viaPoints.Length : 0);
            fullPath = new Transform[totalLen];
            fullPath[0] = startPoint;
            if (viaPoints != null && viaPoints.Length > 0)
                viaPoints.CopyTo(fullPath, 1);  //경유지 복사
            fullPath[totalLen - 1] = endPoint;

            // 시작점으로 위치 이동
            if (fullPath[0] != null)
                transform.position = fullPath[0].position;
            currentTargetIndex = 1; //첫 목표 출발지 다음 포인트로 설정
            isMoving = true;    //이동 시작
        }

        void Update()
        {
            // 이동 중이 아니거나 경로가 없으면 업데이트 중지
            if (!isMoving || fullPath == null || currentTargetIndex >= fullPath.Length)
                return;

            Transform target = fullPath[currentTargetIndex];    // 현재 목표 웨이포인트
            if (target == null) return;

            // 한 프레임마다 목표 웨이포인트로 이동
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                moveSpeed * Time.deltaTime
            );

            // 도착하면 다음 포인트로
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

        void OnReachedDestination()
        {
            // 목적지 도착 시 할 일
            Debug.Log("목표 도착");
            Destroy(gameObject);    // 도착시 제거
            
        }
        void OnDrawGizmos()
        {
            // 경로(웨이포인트) 연결선 그리기
            if (startPoint == null || endPoint == null)
                return;

            Gizmos.color = Color.green;
            Vector3 prev = startPoint.position;

            // ViaPoints가 있으면
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

            // 마지막에 EndPoint까지 연결
            Gizmos.DrawLine(prev, endPoint.position);

            // 웨이포인트 구체(원) 표시
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(startPoint.position, 0.15f);
            if (viaPoints != null)
            {
                foreach (var v in viaPoints)
                {
                    if (v != null)
                        Gizmos.DrawWireSphere(v.position, 0.13f);
                }
            }
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(endPoint.position, 0.18f);
        }
    }
}