/*using UnityEngine;
namespace JiHoon
{
    [System.Serializable]
    public struct UnitPreset
    {
        public UnitData data;   //유닛 데이터 참조
        public GameObject prefab;   //실제 유닛 프리펩
        
    }

    public class UnitSpawner : MonoBehaviour
    {
        public UnitPreset[] unitPresets;

        // 마우스 클릭 위치를 인자로 받는 메서드
        public void SpawnAtPosition(int presetIndex, Vector3 worldPos)
        {
            //유효 범위 체크
            if (presetIndex < 0 || presetIndex >= unitPresets.Length) return;

            // 유닛 프리펩 인스턴스화
            var preset = unitPresets[presetIndex];
            GameObject go = Instantiate(preset.prefab, worldPos, Quaternion.identity);
            go.name = preset.data.unitName; //오브젝트 이름 설정

            //유닛베이스 세팅
            var unit = go.GetComponent<UnitBase>() ?? go.AddComponent<UnitBase>();
            unit.data = preset.data;
        }
    }
}
*/