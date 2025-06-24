using UnityEngine;
using UnityEngine.UI;

/* [0] 개요 : UnitBase
		- 아군, 적군 모두 공통으로 사용되는 능력치.
        - 웨이 포인트를 따라서 이동함
*/

public class UnitBase : MonoBehaviour
{
    // [1] Variable.
    #region ▼▼▼▼▼ Variable ▼▼▼▼▼
    // [◆] - ▶▶▶ 코드 및 이름.
    [SerializeField] private string id;                           // ) 유닛 코드.
    [SerializeField] private string name;                      // ) 유닛 명.


    // [◆] - ▶▶▶ 체력.
    [SerializeField] private float health = 100f;             // ) 체력.
    [SerializeField] private float startHealth = 100f;      // ) 시작할 때의 체력초기값.
    public Image healthBarImage;                         // ) HP 바.


    // [◆] - ▶▶▶ 공격.
    [SerializeField] private float damage = 10f;            // ) 데미지.
    [SerializeField] private int[] rangeOfAttack;             // ) 공격범위.
    [SerializeField] private float attackSpeed = 10f;        // ) 초당 공격속도.


    // [◆] - ▶▶▶ 이동.
    private EnemyMove Move;                        // ) 이동.
    [SerializeField] private float moveSpeed = 10f;         // ) 이동속도.


    // [◆] - ▶▶▶ 사망.
    private bool isDeath = false;       // ) 죽음 체크.
    [SerializeField] public GameObject deathEffectPrefab;        // ) 죽음 프리팹.


    // [◆] - ▶▶▶ ETC.
    [SerializeField] private int[] size;                          // ) 유닛 크기.
    #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲





    // [2] Property.
    #region ▼▼▼▼▼ Property ▼▼▼▼▼
    // [◆] - ▶▶▶ Move의 IsArrive를 이용하여적이 목표지점에 도착하였는지 알려줌.
    public bool IsArrive => Move.IsArrive;
    #endregion ▲▲▲▲▲ Property ▲▲▲▲▲





    // [3] Unity Event Method.
    #region ▼▼▼▼▼ Unity Event Method ▼▼▼▼▼
    // [◆] - ▶▶▶ Start.
    private void Start()
    {
        // [◇] - [◆] - ) EnemyMove를 참조함.
        Move = this.GetComponent<EnemyMove>();

        // [◇] - [◆] - ) 적의 현재 체력을 체력초기값으로 설정함.
        health = startHealth;
    }
    #endregion ▲▲▲▲▲ Unity Event Method ▲▲▲▲▲





    // [4] Custom Method.
    #region ▼▼▼▼▼ Custom Method ▼▼▼▼▼
    // [◆] - ▶▶▶ TakeDamage.
    public void TakeDamage(float damage)
    {
        // [◇] - [◆] - ) 목적지에 도착한 적은 데미지를 받지않음.
        if (Move.IsArrive)
            return;
        // [◇] - [◆] - ) 데미지 받은 만큼 health가 감소함.
        health -= damage;
        // [◇] - [◆] - ) 체력바 UI(백분율) = 현재체력(health) / 체력초기값(startHealth).
        healthBarImage.fillAmount = health / startHealth;
        // [◇] - [◆] - ) 적의 사망 확인 및 체력이 0이하가 되면 죽음으로 처리.
        if (health <= 0f && isDeath == false)
        {
            Die();
        }
    }


    // [◆] - ▶▶▶ Die.
    private void Die()
    {
        // [◇] - [◆] - ) 사망.
        isDeath = true;
        // [◇] - [◆] - ) 적이 죽었을 때, 이팩트 실행.
        GameObject effectGo = Instantiate(deathEffectPrefab, this.transform.position, Quaternion.identity);
        Destroy(effectGo, 2f);
        // [◇] - [◆] - ) 현재 생존한 적의 숫자 카운팅.
        WaveManager.enemyAlive--;
        Debug.Log($"enemyAlive: {WaveManager.enemyAlive}");
        // [◇] - [◆] - ) 죽었을 경우 오브젝트 제거.
        Destroy(this.gameObject, 0f);
    }


    // [◆] - ▶▶▶ Slow.
    public void Slow(float rate)
    {
        // [◇] - [◆] - ) 감속률만큼 적의 속도를 감속.
        Move.moveSpeed = Move.StartMoveSpeed * (1 - rate);
    }
    #endregion ▲▲▲▲▲ Custom Method ▲▲▲▲▲

}
