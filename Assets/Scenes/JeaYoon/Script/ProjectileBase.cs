using UnityEngine;

/* [0] 개요 : TowerProjectile
		- 타워와 원거리 유닛(아군, 적군 포함) 모두 공용으로 사용함.
*/

public class ProjectileBase : MonoBehaviour
{
    // [1] Variable.
    #region ▼▼▼▼▼ Variable ▼▼▼▼▼
    // [◆] - ▶▶▶ 공격.
    [SerializeField] private float attackDamage = 10f;             // ) 공격력.
    [SerializeField] private float projectileSpeed = 10f;            // ) 투사체의 날라가는 속도.

    // [◆] - ▶▶▶ ETC.
    public Transform target;                              // ) 타겟 오브젝트.
    public GameObject projectileImpactPrefab;        // ) 투사체 타격 이펙트 프리팹.
    #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲





    // [2] Unity Event Method.
    #region ▼▼▼▼▼ Unity Event Method ▼▼▼▼▼
    // [◆] - ▶▶▶ SetTarget.
    public void SetTarget(Transform _target)
    {
        // [◇] - [◆] - ) 외부에서 전달받은 '_target'값을 클래스 안에 있는 'target'에 저장.
        this.target = _target;
    }


    // [◆] - ▶▶▶ Update.
    private void Update()
    {
        // [◇] - [◆] - ) 타겟이 죽었을 경우 투사체를 제거함.
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }
        // [◇] - [◆] - ) 타겟과 투사체간의 거리.
        Vector3 dir = target.position - this.transform.position;          // ) 타겟위치 - 투사체위치.
        float ditanceThisFrame = Time.deltaTime * projectileSpeed;    // ) 이번 프레임에 투사체가 이동하는 거리.
        // [◇] - [◆] - ) 투사체가 타겟에게 충돌했는지 확인.
        if (dir.magnitude <= ditanceThisFrame)
        {
            HitTarget();
            return;
        }
        // [◇] - [◆] - ) 투사체를 매 프레임마다 타겟방향으로 이동
        transform.Translate(dir.normalized * Time.deltaTime * projectileSpeed, Space.World);
        // [◇] - [◆] - ) 투사체가 타겟을 바라보며 날라가기
        transform.LookAt(target);
    }
    #endregion ▲▲▲▲▲ Unity Event Method ▲▲▲▲▲





    // [3] Custom Method.
    #region ▼▼▼▼▼ Custom Method ▼▼▼▼▼
    // [◆] - ▶▶▶ HitTarget.
    protected virtual void HitTarget()
    {
        // [◇] - [◆] - ) 타격시 이펙트 효과.
        GameObject effectGo = Instantiate(projectileImpactPrefab, this.transform.position, Quaternion.identity);        // ) 투사체가 적에게 명중했을 때 이펙트 효과가 발생.
        Destroy(effectGo, 2f);                                                                                                          // ) 2초 뒤 이펙트가 사라짐.
        // [◇] - [◆] - ) 타겟에게 데미지 주기.
        Damage(target);
        // [◇] - [◆] - ) 투사체가 맞은 다음 오브젝트 제거.
        Destroy(this.gameObject);
    }


    // [◆] - ▶▶▶ Damage → 타겟에게 데미지 주기.
    protected void Damage(Transform _target)
    {
        // [◇] - [◆] - ) 공격력(attackDamgae)만큼 타겟의 체력(maxHealth)을 계산.
        IDamageable enemy = _target.GetComponent<IDamageable>();
        if (enemy != null)
        {
            // [◇] - [◆] - ) 적에게 'attackDamage'만큼 데미지를 입힘.
            enemy.TakeDamage(attackDamage);
        }
    }
        #endregion ▲▲▲▲▲ Custom Method ▲▲▲▲▲
}
