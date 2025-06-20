using UnityEngine;
namespace JiHoon
{
    public class UnitSpawner : MonoBehaviour
    {
        public UnitData[] unitPresets; // 유닛 프리팹 배열
        public Transform[] spawnPoint; // 유닛 생성 위치

        public void Spawn(int presetIndex)
        {
            var go = new GameObject(unitPresets[presetIndex].unitName);
            var unit = go.AddComponent<UnitBase>();
            unit.data = unitPresets[presetIndex]; // 유닛 데이터 설정
            go.transform.position = spawnPoint[presetIndex].position; // 생성 위치 설정
        }

    }
}