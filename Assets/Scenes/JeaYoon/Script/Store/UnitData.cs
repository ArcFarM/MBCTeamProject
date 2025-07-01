using UnityEngine;

/* [0] 개요 : UnitData
		- UnitData 자체를 저장하고 공유하기 위한 스크립트.
        - Unity에서 유닛 정보를 담는 ScriptableObject를 간편하게 생성할 수 있게 하는 스크립트.
*/


namespace JeaYoon
{
    // [0] Unity에서 Asset - Creat에서 ScriptableObject를 생성할 수 있게하는 항목을 추가함.
    [CreateAssetMenu(menuName = "Shop/UnitData")]
    public class UnitData : ScriptableObject
    {
        // [1] Variable.
        #region ▼▼▼▼▼ Variable ▼▼▼▼▼
        public string unitName;
        public Sprite icon;
        public GameObject prefab;
        public int cost;
        #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲
    }
}
