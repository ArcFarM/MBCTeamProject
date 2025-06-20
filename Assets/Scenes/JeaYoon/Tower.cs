using UnityEngine;

/* [0] 개요 : Tower
		- 아군 유닛 중 타워에 대한 내용임.
		- 타워는 원거리 / 무적
		- 투사체 및 상위티어 타워 필요.
		- 타워의 부모클래스.
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
    protected IDamageable targetEnemy;      // ) IDamageable 스크립트에서 
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





    // [2] Unity Event Method.
    #region ▼▼▼▼▼ Unity Event Method ▼▼▼▼▼
    // [◆] - ▶▶▶ Start.
    private void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.1f);      // ) UpdateTarget를 0.1초마다 반복하여 호출함.
    }


    // [◆] - ▶▶▶ UpdateTarget.
    private void UpdateTarget()
    {
        // [◇] - [◆] - ) Enemy 태그를 가진 모든 오브젝트를 배열 형태로 반환.
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        // [◇] - [◆] - ) 최소거리일 때의 적.
        float minDistance = float.MaxValue;
        GameObject nearEnemy = null;
        // [◇] - [◆] - ) 배열에 포함된 모든 적 오브젝트(GameObject) 를 하나씩 꺼내서 enemy라는 이름으로 반복 실행.
        foreach (var enemy in enemies)
        {
            // [◇] - [◇] - [◆] ) 종점에 도착한 Enemy 탐색에서 제거.
            EnemyBase arriveEnemy = enemy.GetComponent<EnemyBase>();
            // [◇] - [◇] - [◆] ) 이미 목적지에 도착한 적은 무시하고, 더 이상 타겟으로 고려하지 않음.
            if (arriveEnemy != null && arriveEnemy.IsArrive == true)
            {
                continue;
            }
            // [◇] - [◇] - [◆] ) ???. 탐색
            float distance = Vector3.Distance(this.transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearEnemy = enemy;
            }
        }
        // [◇] - [◆] - ) 범위 안에 적이 있는지 확인하고, 가장 가까운 적이 사거리 안에 존재할 경우, 공격함.
        if (nearEnemy != null && minDistance <= attackRange)
        {
            target = nearEnemy.transform;
            targetEnemy = target.GetComponent<IDamageable>();
        }
        else
        {
            target = null;
            targetEnemy = null;
        }
    }


    // [◆] - ▶▶▶ Update.
    private void Update()
    {
        // [◇] - [◆] - ) 타겟이 없을 경우 계속 반복함.
        if (target == null)
            return;
        // [◇] - [◆] - ) 타겟을 찾았을 때 조준.
        LockOn();
        // [◇] - [◆] - ) 타겟팅을 하면 타워가 1초마다 1발씩 쏘기.
        shootCountdown += Time.deltaTime;
        if (shootCountdown >= attackPerSecond)
        {
            // [◇] - [◇] - [◆] ) 타이머 기능 - 1발씩 쏘기.
            Shoot();
            // [◇] - [◇] - [◆] ) 타이머 초기화.
            shootCountdown = 0f;
        }
    }
    #endregion ▲▲▲▲▲ Unity Event Method ▲▲▲▲▲





    // [4] Custom Method.
    #region ▼▼▼▼▼ Custom Method ▼▼▼▼▼
    // [◆] - ▶▶▶ Shoot.
    private void Shoot()
    {
        // [◇] - [◆] - ) 투사체의 정의
        GameObject projectileGo = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        // [◇] - [◆] - ) ProjectileBase 스크립트를 가져옴.
        ProjectileBase projectile = projectileGo.GetComponent<ProjectileBase>();
        // [◇] - [◆] - ) Projectile 스크립트에서 SetTarget을 가져옴.
        if (projectile != null)
        {
            projectile.SetTarget(target);
        }
    }


    // [◆] - ▶▶▶ LockOn.
    private void LockOn()
    {
        // [◇] - [◆] - ) 현재 오브젝트에서 타겟을 향하는 방향 벡터(방향값)를 구하는 것.
        Vector3 dir = target.position - this.transform.position;
        // [◇] - [◆] - ) 타겟을 향해 오브젝트를 회전시키거나 조준할 때.
        // ) Quaternion targetRotation = Quaternion.LookRotation(dir);
    }


    // [◆] - ▶▶▶ OnDrawGizmosSelected.
    private void OnDrawGizmosSelected()
    {
        // [◇] - [◆] - ) 타워의 사거리 색상.
        Gizmos.color = Color.red;
        // [◇] - [◆] - ) 원으로 타워의 사거리 표기.
        Gizmos.DrawWireSphere(this.transform.position, attackRange);
    }
    #endregion ▲▲▲▲▲ Custom Method ▲▲▲▲▲
}