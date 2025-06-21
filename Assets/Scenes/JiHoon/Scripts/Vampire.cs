using UnityEngine;
namespace JiHoon
{
    // UnitBase�� �⺻ ���� ���������� ����ϴٸ� ���� �������̵� ���ʿ�
    public class VampireUnit : UnitBase
    {
        [Header("Vampire Ư��")]
        public float lifestealPercent = 0.3f;  // ���� �� ���� ����

        protected override void DoAttack()
        {
            // �⺻ ���� ���� ����
            Collider2D hit = Physics2D.OverlapCircle(transform.position, data.attackRange, LayerMask.GetMask("Enemy"));
            if (hit != null)
            {
                // ����� �ֱ�
                hit.GetComponent<UnitBase>()?.TakeDamage(data.damage);

                // ����: �������� ���� ������ŭ ü�� ȸ��
                int heal = Mathf.RoundToInt(data.damage * lifestealPercent);
                currentHP = Mathf.Min(currentHP + heal, data.maxHP);
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