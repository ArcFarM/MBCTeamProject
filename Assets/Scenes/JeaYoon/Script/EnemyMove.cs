using UnityEngine;

/* [0] ���� : EnemyMove
		- ���� �̵��� ���õ� ������ �����ϴ� Ŭ����.
*/
public class EnemyMove : MonoBehaviour
{
    // [1] Variable.
    #region ������ Variable ������
    // [��] - ������ �̵�.
    [SerializeField] private float startMoveSpeed = 10f;        // ) ���� �̵��ӵ�.
    [HideInInspector] public float moveSpeed;             // ) ����Ƽ���� Inspector���� �̵��ӵ��� �߸� �����ϴ� ���� ����.


    // [��] - ������ ETC.
    private bool isArrive = false;           // ) ���� ���������� �����Ͽ����� üũ��.
    private Transform target;               // ) ���� ���� ���ϰ� �ִ� ��������Ʈ�� ��ġ�� ��Ÿ���� ����.
    private int wayPointIndex = 0;         // ) ���� ���� ��� ��������Ʈ�� ���Ͽ� �̵������� ��Ÿ��.
    #endregion ������ Variable ������





    // [2] Property.
    #region ������ Property ������
    // [��] - ������ ???.
    public bool IsArrive => isArrive;       // ) ���� ��������Ʈ ������ �����ϸ� true�� �ٲ�.
    public float StartMoveSpeed => startMoveSpeed;      // ) ���� �̵��ӵ��� �б��������� �ܺο� ������.
    #endregion ������ Property ������





    // [3] Unity Event Method.
    #region ������ Unity Event Method ������
    // [��] - ������ Start.
    private void Start()
    {
        // [��] - [��] - ) ???.
        wayPointIndex = 0;                                        // ) wayPointIndex�� 0���� �� �� wayPointIndex�� �⺻���� 1�̱⿡ 0���� �ؾ���.
        target = WayPoints.wayPoints[wayPointIndex];        // ) WayPoints ��ũ��Ʈ���� .
        moveSpeed = startMoveSpeed;                         // ) .
    }


    // [��] - ������ Update.
    private void Update()
    {
        // [��] - [��] - ) ������ ������ ���� �� �̻� �̵��� �������� �ʵ��� ���� ����.
        if (isArrive)
            return;
        // [��] - [��] - ) ���� ����ġ - ��ǥ ��������Ʈ�� ��ġ.
        Vector3 dir = target.position - this.transform.position;
        // [��] - [��] - ) ���� �� �����Ӹ��� �����̰� ��
        transform.Translate(dir.normalized * Time.deltaTime * moveSpeed, Space.World);
        // [��] - [��] - ) ���� ��ǥ ��������Ʈ�� �󸶳��� ����������� �Ÿ��� �����.
        float distance = Vector3.Distance(target.position, this.transform.position);
        // [��] - [��] - ) ���� �������� ���� �����ߴ��� �Ǵ��ϱ� ���� ���.
        if (distance <= 0.1f)
        {
            // [��] - [��] - ) ??? - ���� Ÿ�� ����
            GetNextTarget();
        }
        // [��] - [��] - ) ������� ������ �ӵ��� ���󺹱���.
        moveSpeed = startMoveSpeed;
    }
    #endregion ������ Unity Event Meathod ������





    // [4] Custom Method.
    #region ������ Custom Method ������
    // [��] - ������ GetNextTarget.
    private void GetNextTarget()
    {
        // [��] - [��] - ) ���� ������ �����Ͽ����� ����.
        if (wayPointIndex == WayPoints.wayPoints.Length - 1)
        {
            // [��] - [��] - [��] ) ���� ������ �����Ͽ����� üũ.
            isArrive = true;
            // [��] - [��] - [��] ) ���� ������ ������ ��� �÷��̾��� ������ �Ҹ�.
            PlayerStats.UseLife(1);
            // [��] - [��] - [��] ) Enemy ī����.
            WaveManager.enemyAlive--;
            Debug.Log($"enemyAlive: {WaveManager.enemyAlive}");
            // [��] - [��] - [��] ) ���� ������ �����Ͽ��� ��� ����.
            Destroy(this.gameObject, 1f);
            return;
        }
        // [��] - [��] - ) ���� ���� ��������Ʈ�� �����Ͽ����� ���� ��������Ʈ�� Ÿ���� ������.
        wayPointIndex++;
        target = WayPoints.wayPoints[wayPointIndex];
    }
    #endregion ������ Custom Method ������
}
