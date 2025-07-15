using System.Collections;
using UnityEngine;

public class ArrowAnimationController : MonoBehaviour
{
    [Header("타이밍 설정")]
    [SerializeField] private float delayBeforeStart = 3f;  // 시작 전 대기 시간
    [SerializeField] private float animationDuration = 5f; // 애니메이션 재생 시간

    private Animator animator;

    void Start()
    {
        // Animator 컴포넌트 가져오기
        animator = GetComponent<Animator>();

        // 시작할 때 화살표 숨기기
        gameObject.SetActive(false);

        // 부모나 다른 활성화된 오브젝트에서 코루틴 실행
        GameObject manager = GameObject.Find("MainUI");
        if (manager != null)
        {
            manager.GetComponent<MonoBehaviour>().StartCoroutine(PlayArrowAnimation());
        }
        else
        {
            // MainUI가 없으면 새로 만들어서 실행
            GameObject tempManager = new GameObject("ArrowTempManager");
            tempManager.AddComponent<ArrowHelper>().StartAnimation(this);
        }
    }

    public IEnumerator PlayArrowAnimation()
    {
        // 3초 대기
        yield return new WaitForSeconds(delayBeforeStart);

        // 화살표 활성화
        gameObject.SetActive(true);

        // Animator가 있고 애니메이션이 설정되어 있으면 재생
        if (animator != null)
        {
            animator.Play("Arrow"); // Animator의 기본 상태 이름으로 변경하세요
        }

        // 5초 동안 재생
        yield return new WaitForSeconds(animationDuration);

        // 화살표 비활성화
        gameObject.SetActive(false);
    }
}

// 헬퍼 클래스
public class ArrowHelper : MonoBehaviour
{
    public void StartAnimation(ArrowAnimationController arrow)
    {
        StartCoroutine(arrow.PlayArrowAnimation());
        // 애니메이션 종료 후 자동 삭제
        Destroy(gameObject, 1f);
    }
}