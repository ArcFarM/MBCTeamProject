using UnityEngine;

/* [0] 개요 : AAATowerItemData
		- 
*/

namespace JeaYoon.Roulette
{
    [CreateAssetMenu(fileName = "New Tower Item", menuName = "Shop/Tower Item")]
    public class AAATowerItemData : MonoBehaviour
    {

        // [1] Variable.
        #region ▼▼▼▼▼ Variable ▼▼▼▼▼
        // [◆] - ▶▶▶ Header.
        [Header("기본 아이템 설정")]
        public ItemType itemType;


        // [◆] - ▶▶▶ Header.
        [Header("포탑 전용 설정")]
        public int towerLevel = 2;          // 포탑 레벨 (2단계 이상)
        public TowerType towerType;         // 포탑 타입
        public float attackDamage = 50f;    // 공격력
        public float attackSpeed = 1f;      // 공격 속도
        public float attackRange = 5f;      // 공격 범위
        public GameObject towerPrefab;      // 포탑 프리팹 (unitPrefab 대신 사용)
        #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲





        // [2] Custom Method.
        #region ▼▼▼▼▼ Custom Method ▼▼▼▼▼
        // [◆] - ▶▶▶ OnValidate.
        private void OnValidate()
        {
            // 포탑 아이템은 항상 Tower 타입으로 설정
            itemType = ItemType.Tower;

            // 포탑 레벨이 2 미만이면 자동으로 2로 설정
            if (towerLevel < 2)
                towerLevel = 2;
        }

        // [◆] - ▶▶▶ 456.
        [System.Serializable]
        public enum TowerType
        {
            Basic,      // 기본 포탑
            Magic      // ) 마법 포탑.

            /*
            Cannon,     // 대포 포탑
            Laser,      // 레이저 포탑
            Ice,        // 빙결 포탑
            Poison,     // 독 포탑
            Lightning,  // 번개 포탑
            Flame,      // 화염 포탑
            Sniper      // 저격 포탑
            */
        }

        // ItemType enum에 Tower 추가가 필요할 경우
        public enum ItemType
        {
            Unit,
            Tower,      // 포탑 타입 추가
            Upgrade,
            Consumable
        }
        #endregion ▲▲▲▲▲ Custom Method ▲▲▲▲▲
    }
}