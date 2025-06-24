using UnityEngine;
namespace JiHoon
{
    // UnitBase의 기본 공격 로직만으로 충분하다면 따로 오버라이드 불필요
    public class VampireUnit : UnitBase
    {
        [Header("Vampire 특수")]
        public float lifestealPercent = 0.3f;  // 공격 시 흡혈 비율

        protected override void DoAttack()
        {
            // 기본 근접 공격 실행
            Collider2D hit = Physics2D.OverlapCircle(transform.position, data.attackRange, LayerMask.GetMask("Enemy"));
            if (hit != null)
            {
                // 대미지 주기
                hit.GetComponent<UnitBase>()?.TakeDamage(data.damage);

                // 흡혈: 데미지의 일정 비율만큼 체력 회복
                int heal = Mathf.RoundToInt(data.damage * lifestealPercent);
                currentHP = Mathf.Min(currentHP + heal, data.maxHP);
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