using UnityEngine;

/* [0] 개요 : Tower
		- 아군 유닛 중 타워에 대한 내용임.
		- 타워는 원거리 / 무적
		- 투사체 및 상위티어 타워 필요.
		- 타워의 부모클래스.
		- 
*/

public class Tower : MonoBehaviour
{
    // [1] Variable.
    #region ▼▼▼▼▼ Variable ▼▼▼▼▼
    // [◆] - ▶▶▶ 공격.
    [SerializeField] private float attackPerSecond = 1.0f;      // ) 공격속도.
    private float shootCountdown = 0;                         // ) 공격 이후 다음 공격까지의 카운트.
    [SerializeField] private float attackRange = 10f;           // ) 공격사거리.


    // [◆] - ▶▶▶ 색적.
    protected Transform target;                 // ) 가장 가까운 적을 찾음.
    protected IDamageable targetEnemy;      // ) ???.
    public float searchTimer = 0.5f;             // ) ???.
    private float countdown = 0f;               // ) ???.


    // [◆] - ▶▶▶ 투사체.
    public GameObject projectilePrefab;         // ) 투사체 프리팹.
    public Transform firePoint;                // ) 투사체의 발사 위치.


    // [◆] - ▶▶▶ 비용(아군한정).
    [SerializeField] private int cost;      // ) 아군 구매비용.


    // [◆] - ▶▶▶ ETC.
    public string enemyTag = "Enemy";       // ) Enemy 태그.
    #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲





    // [3] Unity Event Method.
    #region ▼▼▼▼▼ Unity Event Method ▼▼▼▼▼
    // [◆] - ▶▶▶ 123.


    // [◆] - ▶▶▶ 456.


    // [◇] - [◆] - ) 789.
    // [◇] - [◇] - [◆] ) 147.
    // [◇] - [◇] - [◇] - [◆] ) 258.
    // [◇] - [◇] - [◇] - [◇] - [◆] ) 369.
    #endregion ▲▲▲▲▲ Unity Event Method ▲▲▲▲▲





    // [4] Custom Method.
    #region ▼▼▼▼▼ Custom Method ▼▼▼▼▼
    // [◆] - ▶▶▶ 123.


    // [◆] - ▶▶▶ 456.


    // [◇] - [◆] - ) 789.
    // [◇] - [◇] - [◆] ) 147.
    // [◇] - [◇] - [◇] - [◆] ) 258.
    // [◇] - [◇] - [◇] - [◇] - [◆] ) 369.
    #endregion ▲▲▲▲▲ Custom Method ▲▲▲▲▲
}
