using UnityEngine;

/* [0] 개요 : UnitBase
		- 아군, 적군 모두 공통으로 사용되는 능력치.
*/

public class UnitBase : MonoBehaviour
{
    [SerializeField] private string id;                           // ) 유닛 코드.
    [SerializeField] private string name;                      // ) 유닛 명.
    [SerializeField] private int[] size;                          // ) 유닛 크기.
    [SerializeField] private float health = 100f;             // ) 체력.
    [SerializeField] private float damage = 10f;            // ) 데미지.
    [SerializeField] private int[] rangeOfAttack;             // ) 공격범위.
    [SerializeField] private float attackSpeed = 10f;        // ) 초당 공격속도.
}
