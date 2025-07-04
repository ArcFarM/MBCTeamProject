using UnityEngine;

/* [0] 개요 : EnemyMove
		- 적의 이동과 관련된 내용을 관리하는 클래스.
*/
public class EnemyMove : MonoBehaviour
{
    // [1] Variable.
    #region ▼▼▼▼▼ Variable ▼▼▼▼▼
    // [◆] - ▶▶▶ 이동.
    [SerializeField] private float startMoveSpeed = 10f;        // ) 적의 이동속도.
    [HideInInspector] public float moveSpeed;             // ) 유니티에서 Inspector에서 이동속도를 잘못 기입하는 오류 방지.


    // [◆] - ▶▶▶ ETC.
    private bool isArrive = false;           // ) 적이 목적지까지 도착하였는지 체크함.
    private Transform target;               // ) 현재 적이 향하고 있는 웨이포인트의 위치를 나타내는 변수.
    private int wayPointIndex = 0;         // ) 현재 적이 어느 웨이포인트를 향하여 이동중인지 나타냄.
    #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲





    // [2] Property.
    #region ▼▼▼▼▼ Property ▼▼▼▼▼
    // [◆] - ▶▶▶ ???.
    public bool IsArrive => isArrive;       // ) 적이 웨이포인트 종점에 도착하면 true로 바뀜.
    public float StartMoveSpeed => startMoveSpeed;      // ) 적의 이동속도를 읽기전용으로 외부에 공개함.
    #endregion ▲▲▲▲▲ Property ▲▲▲▲▲





    // [3] Unity Event Method.
    #region ▼▼▼▼▼ Unity Event Method ▼▼▼▼▼
    // [◆] - ▶▶▶ Start.
    private void Start()
    {
        // [◇] - [◆] - ) ???.
        wayPointIndex = 0;                                        // ) wayPointIndex를 0으로 함 → wayPointIndex의 기본값은 1이기에 0으로 해야함.
        target = WayPoints.wayPoints[wayPointIndex];        // ) WayPoints 스크립트에서 .
        moveSpeed = startMoveSpeed;                         // ) .
    }


    // [◆] - ▶▶▶ Update.
    private void Update()
    {
        // [◇] - [◆] - ) 종점에 도착한 적은 더 이상 이동을 실행하지 않도록 막는 역할.
        if (isArrive)
            return;
        // [◇] - [◆] - ) 현재 적위치 - 목표 웨이포인트의 위치.
        Vector3 dir = target.position - this.transform.position;
        // [◇] - [◆] - ) 적이 매 프레임마다 움직이게 함
        transform.Translate(dir.normalized * Time.deltaTime * moveSpeed, Space.World);
        // [◇] - [◆] - ) 적이 목표 웨이포인트에 얼마나가 가까워졌는지 거리를 계산함.
        float distance = Vector3.Distance(target.position, this.transform.position);
        // [◇] - [◆] - ) 적이 목적지에 거의 도착했는지 판단하기 위해 계산.
        if (distance <= 0.1f)
        {
            // [◇] - [◆] - ) ??? - 다음 타겟 셋팅
            GetNextTarget();
        }
        // [◇] - [◆] - ) 디버프로 느려진 속도를 원상복구함.
        moveSpeed = startMoveSpeed;
    }
    #endregion ▲▲▲▲▲ Unity Event Meathod ▲▲▲▲▲





    // [4] Custom Method.
    #region ▼▼▼▼▼ Custom Method ▼▼▼▼▼
    // [◆] - ▶▶▶ GetNextTarget.
    private void GetNextTarget()
    {
        // [◇] - [◆] - ) 적이 종점에 도착하였는지 판정.
        if (wayPointIndex == WayPoints.wayPoints.Length - 1)
        {
            // [◇] - [◇] - [◆] ) 적이 종점에 도착하였는지 체크.
            isArrive = true;
            // [◇] - [◇] - [◆] ) 적이 종점에 도착할 경우 플레이어의 라이프 소모.
            // ) PlayerStats.UseLife(1);
            // [◇] - [◇] - [◆] ) Enemy 카운팅.
            WaveManager.enemyAlive--;
            Debug.Log($"enemyAlive: {WaveManager.enemyAlive}");
            // [◇] - [◇] - [◆] ) 적이 종점에 도착하였을 경우 삭제.
            Destroy(this.gameObject, 1f);
            return;
        }
        // [◇] - [◆] - ) 적이 현재 웨이포인트에 도착하였으니 다음 웨이포인트로 타겟을 설정함.
        wayPointIndex++;
        target = WayPoints.wayPoints[wayPointIndex];
    }
    #endregion ▲▲▲▲▲ Custom Method ▲▲▲▲▲
}
