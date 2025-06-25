using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MainGame.Units;
namespace JiHoon
{
    public class UnitPlacementManager : MonoBehaviour
    {
        [Header("References")]
        public UnitSpawner spawner;      // UnitSpawner 오브젝트
        public GridManager gridManager;  // GridManager 오브젝트

        private int selectedPreset = -1; // 선택된 유닛 인덱스

        private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>(); // 이미 배치된 셀 목록
        // UI 버튼에서 호출
        public void OnClickSelectUmit(int presetIndex)
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
            Vector3Int baseCell = gridManager.WorldToCell(wp);

            // 2) 선택된 유닛의 footprint 크기 가져오기
            GameObject prefabObj = spawner.unitPresets[selectedPreset].prefab;
            MainGame.Units.UnitBase prefab = prefabObj.GetComponent<MainGame.Units.UnitBase>();

            int w = prefab.baseRawSize;  // 프로퍼티 사용
            int h = prefab.baseColSize; // 프로퍼티 사용

            // 3) footprint에 포함될 모든 셀 리스트 생성
            List<Vector3Int> cells = new List<Vector3Int>();
            for (int dx = 0; dx < w; dx++)
                for (int dy = 0; dy < h; dy++)
                    cells.Add(new Vector3Int(baseCell.x + dx, baseCell.y + dy, baseCell.z));

            // 4) 프리뷰 처리
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


            // 5) 설치 확정 클릭
            if (canPlace
                && Input.GetMouseButtonDown(0)
                && !EventSystem.current.IsPointerOverGameObject())
            {
                // footprint 셀들의 중앙 계산
                Vector3 worldSum = Vector3.zero;
                foreach (var c in cells)
                    worldSum += gridManager.CellToWorldCenter(c);
                Vector3 spawnPos = worldSum / cells.Count;

                spawner.SpawnAtPosition(selectedPreset, spawnPos);

                // 점유 기록
                foreach (var c in cells)
                    occupiedCells.Add(c);

                // 모드 종료
                selectedPreset = -1;
                gridManager.ClearAllHighlights();
            }

            // 6) ESC로 취소
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                selectedPreset = -1;
                gridManager.ClearAllHighlights();
            }
        }
    }
}