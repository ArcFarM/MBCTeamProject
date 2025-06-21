using UnityEngine;
using UnityEngine.EventSystems;

namespace JiHoon
{
    public class UnitPlacementManager : MonoBehaviour
    {
        [Header("References")]
        public UnitSpawner spawner;      // UnitSpawner 오브젝트
        public GridManager gridManager;  // GridManager 오브젝트

        private int selectedPreset = -1; // 선택된 유닛 인덱스

        // UI 버튼에서 호출
        public void OnClickSelectVampire(int presetIndex)
        {
            selectedPreset = presetIndex;
            // 베이지 길 셀만 파란색으로 하이라이트
            gridManager.HighlightAllowedCells();
        }

        void Update()
        {
            // 배치 모드가 아닐 때는 빠져나감
            if (selectedPreset < 0) return;

            // 1) 마우스→월드→셀
            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            wp.z = 0;
            Vector3Int cell = gridManager.WorldToCell(wp);

            // 2) 프리뷰 처리
            gridManager.ClearPreview();
            if (gridManager.roadCells.Contains(cell))
            {
                gridManager.previewTilemap.SetTile(cell, gridManager.placementPreviewTile);
            }

            // 3) 설치 확정 클릭
            if (Input.GetMouseButtonDown(0)
                && gridManager.roadCells.Contains(cell)
                && !EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 spawnPos = gridManager.CellToWorldCenter(cell);
                spawner.SpawnAtPosition(selectedPreset, spawnPos);

                // 배치 모드 종료 및 하이라이트 클리어
                selectedPreset = -1;
                gridManager.ClearAllHighlights();
            }

            // 4) ESC로 취소
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                selectedPreset = -1;
                gridManager.ClearAllHighlights();
            }
        }
    }
}