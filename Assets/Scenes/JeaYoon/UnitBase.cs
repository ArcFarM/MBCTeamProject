using UnityEngine;

/* [0] ���� : UnitBase
		- �Ʊ�, ���� ��� �������� ���Ǵ� �ɷ�ġ.
*/

public class UnitBase : MonoBehaviour
{
    [SerializeField] private string id;                           // ) ���� �ڵ�.
    [SerializeField] private string name;                      // ) ���� ��.
    [SerializeField] private int[] size;                          // ) ���� ũ��.
    [SerializeField] private float health = 100f;             // ) ü��.
    [SerializeField] private float damage = 10f;            // ) ������.
    [SerializeField] private int[] rangeOfAttack;             // ) ���ݹ���.
    [SerializeField] private float attackSpeed = 10f;        // ) �ʴ� ���ݼӵ�.
}
