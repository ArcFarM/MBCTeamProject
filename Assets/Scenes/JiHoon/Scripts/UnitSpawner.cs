using UnityEngine;

namespace JiHoon
{
    [System.Serializable]
    public struct UnitPreset
    {
        public GameObject prefab;   
    }

    public class UnitSpawner : MonoBehaviour
    {
        public UnitPreset[] unitPresets;

        // 마우스 클릭 위치(worldPos)에 prefab 그대로 인스턴스화
        public void SpawnAtPosition(int presetIndex, Vector3 worldPos)
        {
            if (presetIndex < 0 || presetIndex >= unitPresets.Length) return;

            // 단순 instantiate
            GameObject go = Instantiate(
                unitPresets[presetIndex].prefab,
                worldPos,
                Quaternion.identity
            );

            // 추가 세팅이 필요 없으면 여기서 끝!
            // UnitBase 스크립트가 Awake/Start에서 stats 초기화 담당
        }
    }
}