using UnityEngine;

namespace JiHoon
{
    [System.Serializable]
    public struct UnitPreset
    {
        public GameObject prefab;
        public Sprite icon;           // 카드 덱에 보여줄 스프라이트
        public Sprite hoverIcon;      // 마우스 오버 시 카드 자체가 바뀌는 스프라이트
        public Sprite tooltipImage;   // 마우스 오버 시 툴팁에 표시될 스프라이트 (새로 추가!)
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

            // 단순 instantiate
            GameObject go = Instantiate(
                unitPresets[presetIndex].prefab,
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