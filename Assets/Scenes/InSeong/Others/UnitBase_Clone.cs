/*using System.Collections;
using UnityEngine;
namespace JiHoon
{
    [RequireComponent(typeof(Collider2D))]
    public class UnitBase : MonoBehaviour
    {
        [Header("Config")]
        public UnitData data; // 유닛 데이터 참조

        protected int currentHP; // 현재 체력
        protected bool isDead; // 유닛이 죽었는지 여부

        protected virtual void Awake()
        {
            currentHP = data.maxHP; // 초기 체력 설정
        }
        protected virtual void Start()
        {
            StartCoroutine(AttackRoutine());
        }

        //생명력 감소
        public virtual void TakeDamage(int damage)
        {
            if (isDead) return;
            currentHP -= damage;
            if (currentHP <= 0)
            {
                Die();
            }
        }
        protected virtual void Die()
        {
            isDead = true;
            //죽음 애니메이션, 이펙트 처리
            Destroy(gameObject, 1f);
        }

        //공격 루틴(공통)
        protected virtual IEnumerator AttackRoutine()
        {
            while (!isDead)
            {
                yield return new WaitForSeconds(data.attackInterval);// 공격 주기 대기
                DoAttack();
            }
        }

        //실제 공격 처리(파생 클래스나 이 메서드 내부에서 분기)
        protected virtual void DoAttack()
        {

            switch (data.attackType)
            {
                case AttackType.Melee:
                    MeleeAttack();
                    break;
                case AttackType.Ranged:
                    break;
                case AttackType.AOE:
                    break;
                case AttackType.Support:
                    break;
            }
        }
        //근접 공격 처리
        protected void MeleeAttack()
        {
            if (isDead) return;

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position,
                data.attackRange,
                LayerMask.GetMask("Enemy")
            );

            Collider2D closest = null;
            float minDistSq = float.MaxValue;

            foreach (var h in hits)
            {
                float distSq = (h.transform.position - transform.position).sqrMagnitude;
                if (distSq < minDistSq)
                {
                    minDistSq = distSq;
                    closest = h;
                }
            }

            if (closest != null)
            {
                closest.GetComponent<UnitBase>()?.TakeDamage(data.damage);
            }
        }
        //원거리 공격 처리
        protected void RangedAttack()
        {
            //레이캐스트 이용
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, data.attackRange, LayerMask.GetMask("Enemy"));
            if (hit.collider != null)
            {
                hit.collider.GetComponent<UnitBase>()?.TakeDamage(data.damage);
                //실제 방향, 발사 이펙트 , 풀링 등 추가
            }
        }

        //광역 범위 : 범위내 모든 적
        protected void AOEAttack()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, data.attackRange, LayerMask.GetMask("Enemy"));
            foreach (var h in hits)
            {
                h.GetComponent<UnitBase>()?.TakeDamage(data.damage);
            }
        }

        //사거리 시각화를 위해 Gizmos 사용
        private void OnDrawGizmosSelected()
        {
            if (data == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, data.attackRange);
        }

    }
}*/