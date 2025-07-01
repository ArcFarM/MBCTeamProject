using UnityEngine;

namespace JiHoon
{
    [System.Serializable]
    public struct UnitPreset
    {
        public GameObject prefab;
        public Sprite icon;    // 카드 덱에 보여줄 스프라이트
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

            // 추가 세팅이 필요 없으면 여기서 끝!
            // UnitBase 스크립트가 Awake/Start에서 stats 초기화 담당
        }
        public GameObject GetLastSpawnedUnit()
        {
            return lastSpawnedUnit;
        }
    }

}