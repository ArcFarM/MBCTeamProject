using UnityEngine;
using UnityEngine.UI;

namespace MainGame.Units
{

}

/* [0] ���� : EnemyBase
		- ���� ���õ� ������ �����ϴ� Ŭ����.
*/

public class EnemyBase : UnitBase
{
    // [1] Variable.
    #region ������ Variable ������
    // [��] - ������ �̵�.
    private EnemyMove enemyMove;        // ) �̵�.


    // [��] - ������ ü��.
    private float health;                                      // ) ü��.
    [SerializeField] private float startHealth = 100f;      // ) ������ ���� ü���ʱⰪ.
    public Image healthBarImage;                         // ) ü�¹� UI.


    // [��] - [��] - ) ���.
    private bool isDeath = false;                   // ) ���� üũ.
    public GameObject deathEffectPrefab;        // ) �׾��� ���� ����Ʈ ������.


    // [��] - [��] - ) ����.
    [SerializeField] private int rewardGold = 50;       // ) ���� ����� ��� Gold ����.
    #endregion ������ Variable ������





    // [2] Property.
    #region ������ Property ������
    // [��] - ������ enemyMove�� IsArrive�� �̿��Ͽ����� ��ǥ������ �����Ͽ����� �˷���.
    public bool IsArrive => enemyMove.IsArrive;
    #endregion ������ Property ������





    // [3] Unity Event Method.
    #region ������ Unity Event Method ������
    // [��] - ������ Start.
    private void Start()
    {
        // [��] - [��] - ) EnemyMove�� ������.
        enemyMove = this.GetComponent<EnemyMove>();

        // [��] - [��] - ) ���� ���� ü���� ü���ʱⰪ���� ������.
        health = startHealth;
    }
    #endregion ������ Unity Event Method ������





    // [4] Custom Method.
    #region ������ Custom Method ������
    // [��] - ������ TakeDamage.
    public void TakeDamage(float damage)
    {
        // [��] - [��] - ) �������� ������ ���� �������� ��������.
        if (enemyMove.IsArrive)
            return;
        // [��] - [��] - ) ������ ���� ��ŭ health�� ������.
        health -= damage;
        // [��] - [��] - ) ü�¹� UI(�����) = ����ü��(health) / ü���ʱⰪ(startHealth).
        healthBarImage.fillAmount = health / startHealth;
        // [��] - [��] - ) ���� ��� Ȯ�� �� ü���� 0���ϰ� �Ǹ� �������� ó��.
        if (health <= 0f && isDeath == false)
        {
            Die();
        }
    }


    // [��] - ������ Die.
    private void Die()
    {
        // [��] - [��] - ) ���.
        isDeath = true;
        // [��] - [��] - ) ���� ����� ��� ������ ����.
        PlayerStats.AddMoney(rewardGold);
        // [��] - [��] - ) ���� �׾��� ��, ����Ʈ ����.
        GameObject effectGo = Instantiate(deathEffectPrefab, this.transform.position, Quaternion.identity);
        Destroy(effectGo, 2f);
        // [��] - [��] - ) ���� ������ ���� ���� ī����.
        WaveManager.enemyAlive--;
        Debug.Log($"enemyAlive: {WaveManager.enemyAlive}");
        // [��] - [��] - ) �׾��� ��� ������Ʈ ����.
        Destroy(this.gameObject, 0f);
    }


    // [��] - ������ Slow.
    public void Slow(float rate)
    {
        // [��] - [��] - ) ���ӷ���ŭ ���� �ӵ��� ����.
        enemyMove.moveSpeed = enemyMove.StartMoveSpeed * (1 - rate);
    }
    #endregion ������ Custom Method ������
}
