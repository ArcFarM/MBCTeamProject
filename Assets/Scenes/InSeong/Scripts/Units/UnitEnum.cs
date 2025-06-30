using UnityEngine;
//유닛에 사용될 열거형 
namespace MainGame.Enum {
    //아군 적군 구분
    public enum UnitFaction { Ally, Enemy, }
    //아군/적군 유형
    public enum AllyClass { Melee, Ranged, Tower, }
    public enum EnemyClass { Normal, Elite, Boss, }
    //능력치 인덱스
    public enum StatType {
        //수정 불가 기본 능력치
        //유닛 크기와 방사 피해 범위
        BaseRawSize = 0,
        BaseColSize, BaseSplash,
        //수정 가능한 기본 능력치
        //체력, 공격력, 공격 속도, 사거리, 이동 속도
        BaseHealth, BaseDamage, BaseAtkSpd, BaseRange, BaseSpd,

        //현재 능력치
        CurrRawSize, CurrColSize, CurrSplash,
        CurrHealth, CurrDamage, CurrAtkSpd, CurrRange, CurrSpd,
    }
    //전투에 필요할 상태 머신
    public enum CombatState {
        Idle, //대기 상태
        Detecting, //적 탐지 상태
        Engaging, //전투 돌입
        Moving, //이동 상태
        Fighting, //공격 상태
        Dead, //죽음 상태
    }
}
