using UnityEngine;
namespace JiHoon
{
    [System.Serializable]
    public struct UnitPreset
    {
        public UnitData data;   //���� ������ ����
        public GameObject prefab;   //���� ���� ������
        
    }

    public class UnitSpawner : MonoBehaviour
    {
        public UnitPreset[] unitPresets;

        // ���콺 Ŭ�� ��ġ�� ���ڷ� �޴� �޼���
        public void SpawnAtPosition(int presetIndex, Vector3 worldPos)
        {
            //��ȿ ���� üũ
            if (presetIndex < 0 || presetIndex >= unitPresets.Length) return;

            // ���� ������ �ν��Ͻ�ȭ
            var preset = unitPresets[presetIndex];
            GameObject go = Instantiate(preset.prefab, worldPos, Quaternion.identity);
            go.name = preset.data.unitName; //������Ʈ �̸� ����

            //���ֺ��̽� ����
            var unit = go.GetComponent<UnitBase>() ?? go.AddComponent<UnitBase>();
            unit.data = preset.data;
        }
    }
}
