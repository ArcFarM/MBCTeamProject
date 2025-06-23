using UnityEngine;

namespace MainGame.Units {
    public abstract class UnitBase : MonoBehaviour {
        #region Variables
        //유닛 명칭과 고유번호
        [Header("===== 유닛 고유 번호 및 이름 =====")]
        [SerializeField, Tooltip("고유 번호")]
        protected string unitID;
        [SerializeField, Tooltip("개체명")]
        protected string unitName;
        //각 유닛의 능력치 - 버프,디버프 감안하여 원래 능력치와 현재 능력치로 구분
        //기본 능력치 - 현재는 수정 불가능
        [Header("===== 유닛 기본 능력치 - 현재 수정 불가 =====")]
        [SerializeField, Tooltip("유닛의 크기, 기본값 1/1")]
        protected float baseRawSize = 1, baseColSize = 1;
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
        [SerializeField, Tooltip("유닛의 크기, 기본값 1/1")]
        protected float currRawSize = 1, currColSize = 1;
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
        //참조
        //애니메이션 사용에 필요한 Animator
        Animator animator;
        
        #endregion

        #region Properties
        #endregion

        #region Unity Event Method
        #endregion

        #region Custom Method
        #endregion
    }

}
