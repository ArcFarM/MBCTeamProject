using UnityEngine;
//유닛에 사용될 열거형 
namespace MainGame.Enum {
    //아군 적군 구분
    public enum UnitFaction { Ally, Enemy, }
    //아군/적군 유형
    public enum AllyClass { Melee, Ranged, Tower, }
    public enum EnemyClass { Normal, Elite, Boss, }
}
