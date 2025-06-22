using UnityEngine;
namespace JiHoon
{
    public class Zombie : UnitBase
    {
        

        protected override void DoAttack()
        {
            // �⺻ ���� ���� ����
            Collider2D hit = Physics2D.OverlapCircle(transform.position, data.attackRange, LayerMask.GetMask("Enemy"));
            if (hit != null)
            {
                // ����� �ֱ�
                hit.GetComponent<UnitBase>()?.TakeDamage(data.damage);
            }
        }

        // (����) ���� �� ����Ʈ
        protected override void Die()
        {
            // TODO: �����̾� ��� ����Ʈ
            base.Die();
        }
    }
}