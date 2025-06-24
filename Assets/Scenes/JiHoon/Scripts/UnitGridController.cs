using UnityEngine;
using System.Collections.Generic;
namespace JiHoon
{
    public class UnitGridController : MonoBehaviour
    {
        public GridManager gridManager; // �׸��� �Ŵ��� ����
                                        //�� ��ǥ < > ���� ����
        private Dictionary<Vector3Int, GameObject> unitsOnCell = new Dictionary<Vector3Int, GameObject>();

        //���콺 Ŭ�� ������ �Ʊ� ��ġ
        public void TryPlaceUnit(GameObject unitPrefab, Vector3 worldPos)
        {
            var cell = gridManager.WorldToCell(worldPos); //���� ��ǥ�� �� ��ǥ�� ��ȯ
            if (!gridManager.baseTilemap.HasTile(cell))
            {
                //���� Ÿ���� ������ ��ġ �Ұ�
                Debug.Log("�� ���Դϴ�");
                return;
            }
            if (unitsOnCell.ContainsKey(cell))
            {
                Debug.Log("�̹� ������ �ֽ��ϴ�!");
                return;
            }

            //���� ��ġ
            Vector3 spawnPos = gridManager.CellToWorldCenter(cell); //�� �߽� ��ġ ���
            var unit = Instantiate(unitPrefab, spawnPos, Quaternion.identity); //���� ����
            unitsOnCell.Add(cell, unit); //���� ���� ���� ����
        }

        //�� �������� ���� ������
        public GameObject GetUnitAt(Vector3Int cell)
        {
            unitsOnCell.TryGetValue(cell, out var unit);
            return unit;
        }
    }
}