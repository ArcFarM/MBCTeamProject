using UnityEngine;
namespace JiHoon
{
    public class Zombie : UnitBase
    {
        

        protected override void DoAttack()
        {
            // 기본 근접 공격 실행
            Collider2D hit = Physics2D.OverlapCircle(transform.position, data.attackRange, LayerMask.GetMask("Enemy"));
            if (hit != null)
            {
                // 대미지 주기
                hit.GetComponent<UnitBase>()?.TakeDamage(data.damage);
            }
        }

        // (선택) 죽을 때 이펙트
        protected override void Die()
        {
            // TODO: 뱀파이어 사망 이펙트
            base.Die();
        }
    }
}