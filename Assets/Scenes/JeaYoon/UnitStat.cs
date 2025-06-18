using UnityEngine;
using UnityEngine.UI;

/*
 �� ���� (�켱���� 1)
- �Ʊ� : ���� / ���Ÿ� / Ÿ��
- ���� : �븻 / ����Ʈ / ����
- �ɷ�ġ : ���ݷ�, ü��, ���ݼӵ�, ���ݹ���, �̵��ӵ�, ���(�Ʊ� ����), �г�Ƽ(���� ����) ....
 */

public class UnitStat : MonoBehaviour
{
    // [1] Variable.
    #region ������ Variable ������
    // [��] - ������ ü��.
    [SerializeField] private float health = 100f;       // ) ü�� �ʱⰪ.
    public Image healthBarImage;                     // ) HP ��.


    // �� HP�� �Ʊ��� ������ ���� ������ ��.

    // [��] - ������ ����.
    [SerializeField] private float attackDamgae = 10f;      // ) ���ݷ�.
    [SerializeField] private float attackSpeed = 10f;      // ) ���ݼӵ�.
    [SerializeField] private float attackRange = 10f;      // ) ���ݻ�Ÿ�.

    // [��] - ������ �̵��ӵ�.
    [SerializeField] private float moveSpeed = 10f;

    // [��] - ������ ����.
    private bool isDeath = false;       // ) ���� üũ.
    public GameObject deathEffectPrefab;        // ) ���� ������.

    // [��] - ������ ���(�Ʊ�����).
    [SerializeField] private int cost;      // ) �Ʊ� ���ź��.

    // [��] - ������ �г�Ƽ(��������).


    // [��] - ������ ETC.
    [SerializeField] private int rewardGold = 50;       // ) ���� ����� �� ��� ��� ����.
    #endregion ������ Variable ������
}
