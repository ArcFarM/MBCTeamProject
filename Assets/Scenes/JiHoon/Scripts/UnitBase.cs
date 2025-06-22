using System.Collections;
using UnityEngine;
namespace JiHoon
{
    [RequireComponent(typeof(Collider2D))]
    public class UnitBase : MonoBehaviour
    {
        [Header("Config")]
        public UnitData data; // ���� ������ ����

        protected int currentHP; // ���� ü��
        protected bool isDead; // ������ �׾����� ����

        protected virtual void Awake()
        {
            currentHP = data.maxHP; // �ʱ� ü�� ����
        }
        protected virtual void Start()
        {
            StartCoroutine(AttackRoutine());
        }

        //����� ����
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
            //���� �ִϸ��̼�, ����Ʈ ó��
            Destroy(gameObject, 1f);
        }

        //���� ��ƾ(����)
        protected virtual IEnumerator AttackRoutine()
        {
            while (!isDead)
            {
                yield return new WaitForSeconds(data.attackInterval);// ���� �ֱ� ���
                DoAttack();
            }
        }

        //���� ���� ó��(�Ļ� Ŭ������ �� �޼��� ���ο��� �б�)
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
        //���� ���� ó��
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
        //���Ÿ� ���� ó��
        protected void RangedAttack()
        {
            //����ĳ��Ʈ �̿�
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, data.attackRange, LayerMask.GetMask("Enemy"));
            if (hit.collider != null)
            {
                hit.collider.GetComponent<UnitBase>()?.TakeDamage(data.damage);
                //���� ����, �߻� ����Ʈ , Ǯ�� �� �߰�
            }
        }

        //���� ���� : ������ ��� ��
        protected void AOEAttack()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, data.attackRange, LayerMask.GetMask("Enemy"));
            foreach (var h in hits)
            {
                h.GetComponent<UnitBase>()?.TakeDamage(data.damage);
            }
        }

        //��Ÿ� �ð�ȭ�� ���� Gizmos ���
        private void OnDrawGizmosSelected()
        {
            if (data == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, data.attackRange);
        }

    }
}