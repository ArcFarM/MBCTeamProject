/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


    /*
     ㅁ 유닛 (우선순위 1)
    - 아군 : 근접 / 원거리 / 타워
    - 적군 : 노말 / 엘리트 / 보스
    - 능력치 : 공격력, 체력, 공격속도, 공격범위, 이동속도, 비용(아군 한정), 패널티(적군 한정) ....
     

    // [0] 유닛 타입 구분.
    public enum UnitFaction { Ally, Enemy }
    public enum AllyUnitClass { Melee, Ranged, Tower }
    public enum EnemyUnitClass { Boss, Elite, Normal }

    public class UnitStat : MonoBehaviour
    {
        // [1] Variable.
        #region ▼▼▼▼▼ Variable ▼▼▼▼▼
        // [◆] - ▶▶▶ 체력.
        [SerializeField] private UnitFaction faction;            // ) 각 유닛별 소속.
        [SerializeField] private float maxHealth = 100f;       // ) 체력 초기값.
        public Image healthBarImage;                         // ) HP 바.
                                                             // ㄴ HP는 아군과 적군을 따로 나눠야 함.

        // [◆] - ▶▶▶ 공격.
        [SerializeField] private float attackDamgae = 10f;         // ) 공격력.
        [SerializeField] private float attackPerSecond = 1.0f;      // ) 공격속도.
        [SerializeField] private float attackRange = 10f;           // ) 공격사거리.

        // [◆] - ▶▶▶ 이동속도.
        [SerializeField] private float moveSpeed = 10f;

        // [◆] - ▶▶▶ 죽음.
        private bool isDeath = false;       // ) 죽음 체크.
        [SerializeField] public GameObject deathEffectPrefab;        // ) 죽음 프리팹.

        // [◆] - ▶▶▶ 비용(아군한정).
        [SerializeField] private int cost;      // ) 아군 구매비용.

        // [◆] - ▶▶▶ 패널티(적군한정).

        // [◆] - ▶▶▶ ETC.
        [SerializeField] private int rewardGold = 100;       // ) 적을 잡았을 때 얻는 골드 보상.

        #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲



        // 5. 애니메이션 제어용 변수.
        private Animator animator;


    }

    public class StatusEffect
    {
        public string effectName;
        public float duration;
        public float value;

        public StatusEffect(string name, float dur, float val)
        {
            effectName = name;
            duration = dur;
            value = val;
        }
    }
*/
