using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform leader;         // PathFollower가 붙은 리더
    public Vector3 localOffset;      // 리더 로컬 좌표계에서의 오프셋
    public float followSpeed = 3f;   // 이동 속도

    void Update()
    {
        // 리더의 현재 로컬→월드 포메이션 위치 계산
        Vector3 target = leader.TransformPoint(localOffset);
        // 부드럽게 이동 (원하시면 Lerp나 SmoothDamp으로 바꿔도 OK)
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            followSpeed * Time.deltaTime
        );

        // 옵션: 원본 방향(리더 바라보기)
        // transform.rotation = Quaternion.LookRotation(leader.position - transform.position);
    }
}