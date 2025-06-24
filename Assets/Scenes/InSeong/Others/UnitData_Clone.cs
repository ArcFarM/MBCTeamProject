/*using UnityEngine;
namespace JiHoon
{
    public enum Faction { Player, Enemy }
    public enum AttackType { Melee, Ranged, AOE, Support }

    [CreateAssetMenu(menuName = "Game/UnitData")]
    public class UnitData : ScriptableObject
    {
        public string unitID;   // 유닛의 고유 ID
        public string unitName; // 유닛의 이름
        public Faction faction; // 유닛의 진영 (Player 또는 Enemy)
        public AttackType attackType; // 유닛의 공격 유형 (Melee, Ranged, AOE, Support)

        [Header("Basic Stats")]
        public int maxHP; // 최대 체력
        public int damage; // 공격력
        public float attackRange; // 공격 사거리
        public float attackInterval; // 공격 주기

        [Header("Footprint")]
        // 유닛이 차지하는 셀 크기 (가로)
        public int footprintWidth = 1;
        // 유닛이 차지하는 셀 크기 (세로)
        public int footprintHeight = 1;

        //TODO :[Header("Special")] 상태이상 공격, 상태이상등 체크 아직 기획안함

    }
}*/