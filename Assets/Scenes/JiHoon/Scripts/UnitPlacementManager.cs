using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JiHoon
{
    public class UnitPlacementManager : MonoBehaviour
    {
        [Header("References")]
        public UnitSpawner spawner;      // UnitSpawner ������Ʈ
        public GridManager gridManager;  // GridManager ������Ʈ

        private int selectedPreset = -1; // ���õ� ���� �ε���

        private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>(); // �̹� ��ġ�� �� ���
        // UI ��ư���� ȣ��
        public void OnClickSelectUmit(int presetIndex)
        {
            selectedPreset = presetIndex;
            // ������ �� ���� �Ķ������� ���̶���Ʈ
            gridManager.HighlightAllowedCells();
        }

        void Update()
        {
            // ��ġ ��尡 �ƴ� ���� ��������
            if (selectedPreset < 0) return;

            // 1) ���콺�����漿
            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            wp.z = 0;
            Vector3Int baseCell = gridManager.WorldToCell(wp);

            // 2) footprint ũ�� �б�
            UnitData data = spawner.unitPresets[selectedPreset].data;
            int w = data.footprintWidth;
            int h = data.footprintHeight;

            // 3) footprint�� ���Ե� ��� �� ����Ʈ ����
            List<Vector3Int> cells = new List<Vector3Int>();
            for (int dx = 0; dx < w; dx++)
                for (int dy = 0; dy < h; dy++)
                    cells.Add(new Vector3Int(baseCell.x + dx, baseCell.y + dy, baseCell.z));

            // 4) ������ ó��
            gridManager.ClearPreview();
            bool canPlace = true;
            foreach (var c in cells)
            {
                if (!gridManager.IsRoadCell(c) || occupiedCells.Contains(c))
                {
                    canPlace = false;
                    break;
                }
            }
            if (canPlace)
            {
                foreach (var c in cells)
                    gridManager.previewTilemap.SetTile(c, gridManager.placementPreviewTile);
            }


            // 5) ��ġ Ȯ�� Ŭ��
            if (canPlace
                && Input.GetMouseButtonDown(0)
                && !EventSystem.current.IsPointerOverGameObject())
            {
                // footprint ������ �߾� ���
                Vector3 worldSum = Vector3.zero;
                foreach (var c in cells)
                    worldSum += gridManager.CellToWorldCenter(c);
                Vector3 spawnPos = worldSum / cells.Count;

                spawner.SpawnAtPosition(selectedPreset, spawnPos);

                // ���� ���
                foreach (var c in cells)
                    occupiedCells.Add(c);

                // ��� ����
                selectedPreset = -1;
                gridManager.ClearAllHighlights();
            }

            // 6) ESC�� ���
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                selectedPreset = -1;
                gridManager.ClearAllHighlights();
            }
        }
    }
}