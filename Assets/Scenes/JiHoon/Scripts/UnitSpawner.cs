using UnityEngine;

namespace JiHoon
{
    [System.Serializable]
    public struct UnitPreset
    {
        public UnitCardData cardData;  // ScriptableObject로 모든 데이터 관리
    }

    public class UnitSpawner : MonoBehaviour
    {
        private GameObject lastSpawnedUnit; // 마지막으로 생성된 유닛
        public UnitPreset[] unitPresets;

        [Header("스폰된 아군 유닛을 담을 컨테이너")]
        public Transform unitContainer;

        // 마우스 클릭 위치(worldPos)에 prefab 그대로 인스턴스화
        public void SpawnAtPosition(int presetIndex, Vector3 worldPos)
        {
            if (presetIndex < 0 || presetIndex >= unitPresets.Length) return;
            if (unitPresets[presetIndex].cardData == null) return;

            var cardData = unitPresets[presetIndex].cardData;
            if (cardData.unitPrefab == null) return;

            // 단순 instantiate
            GameObject go = Instantiate(
                cardData.unitPrefab,
                worldPos,
                Quaternion.identity,
                unitContainer
            );

            lastSpawnedUnit = go;  // 마지막 스폰 유닛 저장
        }

        public GameObject GetLastSpawnedUnit()
        {
            return lastSpawnedUnit;
        }
    }
}