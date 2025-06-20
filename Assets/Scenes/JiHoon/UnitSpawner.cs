using UnityEngine;
namespace JiHoon
{
    public class UnitSpawner : MonoBehaviour
    {
        public UnitData[] unitPresets; // ���� ������ �迭
        public Transform[] spawnPoint; // ���� ���� ��ġ

        public void Spawn(int presetIndex)
        {
            var go = new GameObject(unitPresets[presetIndex].unitName);
            var unit = go.AddComponent<UnitBase>();
            unit.data = unitPresets[presetIndex]; // ���� ������ ����
            go.transform.position = spawnPoint[presetIndex].position; // ���� ��ġ ����
        }

    }
}