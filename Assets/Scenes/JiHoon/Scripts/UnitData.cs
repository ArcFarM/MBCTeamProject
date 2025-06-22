using UnityEngine;
namespace JiHoon
{
    public enum Faction { Player, Enemy }
    public enum AttackType { Melee, Ranged, AOE, Support }

    [CreateAssetMenu(menuName = "Game/UnitData")]
    public class UnitData : ScriptableObject
    {
        public string unitID;   // ������ ���� ID
        public string unitName; // ������ �̸�
        public Faction faction; // ������ ���� (Player �Ǵ� Enemy)
        public AttackType attackType; // ������ ���� ���� (Melee, Ranged, AOE, Support)

        [Header("Basic Stats")]
        public int maxHP; // �ִ� ü��
        public int damage; // ���ݷ�
        public float attackRange; // ���� ��Ÿ�
        public float attackInterval; // ���� �ֱ�

        [Header("Footprint")]
        // ������ �����ϴ� �� ũ�� (����)
        public int footprintWidth = 1;
        // ������ �����ϴ� �� ũ�� (����)
        public int footprintHeight = 1;

        //TODO :[Header("Special")] �����̻� ����, �����̻�� üũ ���� ��ȹ����

    }
}