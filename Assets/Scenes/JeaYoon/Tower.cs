using UnityEngine;

/* [0] ���� : Tower
		- �Ʊ� ���� �� Ÿ���� ���� ������.
		- Ÿ���� ���Ÿ� / ����
		- ����ü �� ����Ƽ�� Ÿ�� �ʿ�.
		- Ÿ���� �θ�Ŭ����.
		- 
*/

public class Tower : MonoBehaviour
{
    // [1] Variable.
    #region ������ Variable ������
    // [��] - ������ ����.
    [SerializeField] private float attackPerSecond = 1.0f;      // ) ���ݼӵ�.
    private float shootCountdown = 0;                         // ) ���� ���� ���� ���ݱ����� ī��Ʈ.
    [SerializeField] private float attackRange = 10f;           // ) ���ݻ�Ÿ�.


    // [��] - ������ ����.
    protected Transform target;                 // ) ���� ����� ���� ã��.
    protected IDamageable targetEnemy;      // ) ???.
    public float searchTimer = 0.5f;             // ) ???.
    private float countdown = 0f;               // ) ???.


    // [��] - ������ ����ü.
    public GameObject projectilePrefab;         // ) ����ü ������.
    public Transform firePoint;                // ) ����ü�� �߻� ��ġ.


    // [��] - ������ ���(�Ʊ�����).
    [SerializeField] private int cost;      // ) �Ʊ� ���ź��.


    // [��] - ������ ETC.
    public string enemyTag = "Enemy";       // ) Enemy �±�.
    #endregion ������ Variable ������





    // [3] Unity Event Method.
    #region ������ Unity Event Method ������
    // [��] - ������ 123.


    // [��] - ������ 456.


    // [��] - [��] - ) 789.
    // [��] - [��] - [��] ) 147.
    // [��] - [��] - [��] - [��] ) 258.
    // [��] - [��] - [��] - [��] - [��] ) 369.
    #endregion ������ Unity Event Method ������





    // [4] Custom Method.
    #region ������ Custom Method ������
    // [��] - ������ 123.


    // [��] - ������ 456.


    // [��] - [��] - ) 789.
    // [��] - [��] - [��] ) 147.
    // [��] - [��] - [��] - [��] ) 258.
    // [��] - [��] - [��] - [��] - [��] ) 369.
    #endregion ������ Custom Method ������
}
