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

        // UI ��ư���� ȣ��
        public void OnClickSelectVampire(int presetIndex)
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
            Vector3Int cell = gridManager.WorldToCell(wp);

            // 2) ������ ó��
            gridManager.ClearPreview();
            if (gridManager.roadCells.Contains(cell))
            {
                gridManager.previewTilemap.SetTile(cell, gridManager.placementPreviewTile);
            }

            // 3) ��ġ Ȯ�� Ŭ��
            if (Input.GetMouseButtonDown(0)
                && gridManager.roadCells.Contains(cell)
                && !EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 spawnPos = gridManager.CellToWorldCenter(cell);
                spawner.SpawnAtPosition(selectedPreset, spawnPos);

                // ��ġ ��� ���� �� ���̶���Ʈ Ŭ����
                selectedPreset = -1;
                gridManager.ClearAllHighlights();
            }

            // 4) ESC�� ���
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                selectedPreset = -1;
                gridManager.ClearAllHighlights();
            }
        }
    }
}