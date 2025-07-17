using MainGame.Units.Battle;
using MainGame.Enum;
using UnityEngine;
using System.Collections.Generic;

namespace MainGame.Units {
    public abstract class UnitBase : MonoBehaviour{
        #region Variables
        #region 유닛 능력치
        //유닛 명칭과 고유번호
        [Header("===== 유닛 고유 번호 및 이름 =====")]
        [SerializeField, Tooltip("고유 번호")]
        protected string unitID;
        [SerializeField, Tooltip("개체명")]
        protected string unitName;
        [SerializeField, Tooltip("소속")]
        protected UnitFaction faction;
        //각 유닛의 능력치 - 버프,디버프 감안하여 원래 능력치와 현재 능력치로 구분
        //기본 능력치 - 현재는 수정 불가능
        [Header("===== 유닛 기본 능력치 - 현재 수정 불가 =====")]
        [SerializeField, Tooltip("유닛의 행 크기, 기본값 1")]
        protected int baseRawSize = 1;
        [SerializeField, Tooltip("유닛의 열 크기, 기본값 1")]
        protected int baseColSize = 1;
        [SerializeField, Tooltip("방사형 공격 범위, 기본값 0")]
        protected float baseSplash = 0f;
        //수정 가능한 기본 능력치
        [Header("===== 유닛 기본 능력치 - 수정 가능 =====")]
        [SerializeField, Tooltip("체력, 기본값 1")]
        protected float baseHealth = 1;
        [SerializeField, Tooltip("공격력, 기본값 1")]
        protected float baseDamage = 1;
        [SerializeField, Tooltip("공격 대기 시간, 기본값 1")]
        protected float baseAtkSpd = 1;
        [SerializeField, Tooltip("사거리, 기본값 1")]
        protected float baseRange = 1;
        [SerializeField, Tooltip("이동 속도, 기본값 1")]
        protected float baseSpd = 1;
        [Header("===== 유닛 현재 기본 능력치 - 현재 수정 불가 =====")]
        [SerializeField, Tooltip("유닛의 행 크기, 기본값 1")]
        protected float currRawSize = 1;
        [SerializeField, Tooltip("유닛의 열 크기, 기본값 1")]
        protected float currColSize = 1;
        [SerializeField, Tooltip("방사형 공격 범위, 기본값 0")]
        protected float currSplash = 0f;
        [Header("===== 유닛 현재 능력치 - 수정 가능 =====")]
        [SerializeField, Tooltip("체력, 기본값 1")]
        protected float currHealth = 1;
        [SerializeField, Tooltip("공격력, 기본값 1")]
        protected float currDamage = 1;
        [SerializeField, Tooltip("공격 대기 시간, 기본값 1")]
        protected float currAtkSpd = 1;
        [SerializeField, Tooltip("사거리, 기본값 1")]
        protected float currRange = 1;
        [SerializeField, Tooltip("이동 속도, 기본값 1")]
        protected float currSpd = 1;
        //능력치는 배열로 관리
        [SerializeField] protected float[] stats;
        #endregion
        #endregion

        #region Properties
        public string GetID { get { return unitID; } }
        public string GetName { get { return unitName; } }

        //읽기 전용 프로퍼티들
        public UnitFaction GetFaction { get { return faction; } }
        public int GetBaseRawSize { get { return baseRawSize; } }
        public int GetBaseColSize { get { return baseColSize; } }
        public bool IsDead { get { return currHealth <= 0; } }

        //프로퍼티처럼 쓰일 메서드
        public float GetStat(StatType st) {
            int index = (int)st;
            //Debug.Log(stats.Length);
            if (index >= 0 && index < stats.Length) return stats[index];
            else {
                Debug.LogError("올바르지 않은 StatType 입력됨 : "+ st);
                return -1;
            }
        }

        public void SetStat(StatType st, float value) {
            int index = (int)st;
            if (index >= 0 && index < stats.Length) stats[index] = value;
            else {
                Debug.LogError("올바르지 않은 StatType 입력됨 : " + st);
            }
        }

        #endregion

        #region Unity Event Method
        protected virtual void Awake()
        {
            InitStats();
        }

        protected virtual void Start() { }
        #endregion

        #region Custom Method
        //능력치 초기화
        void InitStats() {
            // StatType 열거형이 몇 개인지 계산
            int statCount = System.Enum.GetValues(typeof(StatType)).Length;
            // 경우에 따라 배열도 초기화
            if (stats.Length != statCount) {
                stats = new float[statCount];
            }
            stats[(int)StatType.BaseRawSize] = baseRawSize;
            stats[(int)StatType.BaseColSize] = baseColSize;
            stats[(int)StatType.BaseSplash] = baseSplash;

            stats[(int)StatType.BaseHealth] = baseHealth;
            stats[(int)StatType.BaseDamage] = baseDamage;
            stats[(int)StatType.BaseAtkSpd] = baseAtkSpd;
            stats[(int)StatType.BaseRange] = baseRange;
            stats[(int)StatType.BaseSpd] = baseSpd;

            SetToBase();
        }

        //능력치 기본값으로 설정
        void SetToBase() {
            stats[(int)StatType.CurrHealth] = baseHealth;
            stats[(int)StatType.CurrDamage] = baseDamage;
            stats[(int)StatType.CurrAtkSpd] = baseAtkSpd;
            stats[(int)StatType.CurrRange] = baseRange;
            stats[(int)StatType.CurrSpd] = baseSpd;

            currHealth = baseHealth;
            currDamage = baseDamage;
            currAtkSpd = baseAtkSpd;
            currRange = baseRange;
            currSpd = baseSpd;
        }
        #endregion
    }

}
