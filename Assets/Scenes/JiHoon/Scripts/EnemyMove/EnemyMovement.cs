using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Move Settings")]
    public float moveSpeed = 3f;
    public float nodeReachDistance = 0.1f;

    [Header("Path Settings")]
    public Transform startPoint;
    public Transform endPoint;
    public Transform[] viaPoints; // 중간 경유지들(웨이포인트)

    private Transform[] fullPath; // 실제 이동 경로 (시작+경유+도착 합쳐서)

    private int currentTargetIndex = 0;
    public bool isMoving = false;

    void Start()
    {
        // 경로 합치기: Start → (Via...) → End
        int totalLen = 2 + (viaPoints != null ? viaPoints.Length : 0);
        fullPath = new Transform[totalLen];
        fullPath[0] = startPoint;
        if (viaPoints != null && viaPoints.Length > 0)
            viaPoints.CopyTo(fullPath, 1);
        fullPath[totalLen - 1] = endPoint;

        // 시작점으로 위치 이동, 첫 타겟은 두 번째 포인트부터!
        if (fullPath[0] != null)
            transform.position = fullPath[0].position;
        currentTargetIndex = 1;
        isMoving = true;
    }

    void Update()
    {
        if (!isMoving || fullPath == null || currentTargetIndex >= fullPath.Length)
            return;

        Transform target = fullPath[currentTargetIndex];
        if (target == null) return;

        // 이동
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
        Debug.Log("Enemy reached destination!");
        Destroy(gameObject);
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
