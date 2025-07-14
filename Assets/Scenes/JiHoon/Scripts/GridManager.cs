using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap placementZoneTilemap;  // 에디터에서 설치 가능 영역만 칠해둔 Tilemap
    public Tilemap rangeTilemap;          // 설치 가능 범위 하이라이트
    public Tilemap previewTilemap;        // 마우스 오버 설치 프리뷰

    [Header("Tiles")]
    public TileBase rangeHighlightTile;
    public TileBase placementPreviewTile;

    // 설치 가능 셀 목록
    public HashSet<Vector3Int> placementCells { get; private set; }

    // 유닛별 점유 셀 기록
    private Dictionary<GameObject, HashSet<Vector3Int>> _unitCells
        = new Dictionary<GameObject, HashSet<Vector3Int>>();

    // 죽은 유닛 정리용 타이머
    private float cleanupTimer = 0f;

    void Start()
    {
        // placementCells 초기화 (placementZoneTilemap에 칠한 타일만)
        placementCells = new HashSet<Vector3Int>();
        var bounds = placementZoneTilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
            if (placementZoneTilemap.HasTile(pos))
                placementCells.Add(pos);
    }

    // Update 제거 - 카드 클릭 시에만 정리

    /// <summary>
    /// 해당 셀이 설치 가능 영역인지 확인
    /// </summary>
    public bool IsRoadCell(Vector3Int cell)
        => placementCells.Contains(cell);

    /// <summary>
    /// 설치 가능 영역 전체를 rangeTilemap에 하이라이트
    /// </summary>
    public void HighlightAllowedCells()
    {
        rangeTilemap.ClearAllTiles();
        foreach (var c in placementCells)
            rangeTilemap.SetTile(c, rangeHighlightTile);
    }

    /// <summary>
    /// 미리보기 클리어
    /// </summary>
    public void ClearPreview()
        => previewTilemap.ClearAllTiles();

    /// <summary>
    /// 단일 셀 미리보기
    /// </summary>
    public void PreviewCell(Vector3Int cell)
    {
        previewTilemap.ClearAllTiles();
        if (IsRoadCell(cell))
            previewTilemap.SetTile(cell, placementPreviewTile);
    }

    /// <summary>
    /// 모든 하이라이트/프리뷰 제거
    /// </summary>
    public void ClearAllHighlights()
    {
        rangeTilemap.ClearAllTiles();
        previewTilemap.ClearAllTiles();
    }

    /// <summary>
    /// 월드 좌표 → 셀 좌표
    /// </summary>
    public Vector3Int WorldToCell(Vector3 worldPos)
        => placementZoneTilemap.WorldToCell(worldPos);

    /// <summary>
    /// 셀 좌표 → 셀 중앙 월드 좌표
    /// </summary>
    public Vector3 CellToWorldCenter(Vector3Int cellPos)
        => placementZoneTilemap.GetCellCenterWorld(cellPos);

    // === 이하 유닛 점유(Cell Occupation) 관리 ===

    public HashSet<Vector3Int> GetOccupiedCellsFor(GameObject unit)
    {
        if (_unitCells.TryGetValue(unit, out var set))
            return new HashSet<Vector3Int>(set);
        return new HashSet<Vector3Int>();
    }

    public void FreeCells(HashSet<Vector3Int> cells)
    {
        var keysToUpdate = new List<GameObject>();
        foreach (var kv in _unitCells)
        {
            if (kv.Value.Overlaps(cells))
                keysToUpdate.Add(kv.Key);
        }
        foreach (var unit in keysToUpdate)
        {
            var set = _unitCells[unit];
            foreach (var cell in cells)
                set.Remove(cell);
            if (set.Count == 0)
                _unitCells.Remove(unit);
        }
    }

    public void OccupyCells(HashSet<Vector3Int> cells, GameObject unit)
    {
        if (unit == null) return;
        _unitCells[unit] = new HashSet<Vector3Int>(cells);
    }

    public HashSet<Vector3Int> GetAllOccupiedCells()
    {
        var result = new HashSet<Vector3Int>();
        foreach (var set in _unitCells.Values)
            result.UnionWith(set);
        return result;
    }

    public HashSet<Vector3Int> GetCellsFor(GameObject unit, Vector3Int baseCell)
    {
        var cells = new HashSet<Vector3Int>();
        var ub = unit.GetComponent<MainGame.Units.UnitBase>();
        if (ub == null) return cells;

        int w = ub.GetBaseRawSize;
        int h = ub.GetBaseColSize;
        for (int dx = 0; dx < w; dx++)
            for (int dy = 0; dy < h; dy++)
                cells.Add(new Vector3Int(baseCell.x + dx,
                                         baseCell.y + dy,
                                         baseCell.z));
        return cells;
    }

    // === 추가: 죽은 유닛 자동 정리 ===

    /// <summary>
    /// 죽은 유닛의 점유 정보를 정리 (카드 클릭 시 호출)
    /// </summary>
    public void CleanupDeadUnits()
    {
        var keysToRemove = new List<GameObject>();
        foreach (var kvp in _unitCells)
        {
            // 유닛이 null이거나 비활성화된 경우
            if (kvp.Key == null || !kvp.Key.activeInHierarchy)
            {
                keysToRemove.Add(kvp.Key);
            }
        }

        // 제거
        int removedCount = 0;
        foreach (var key in keysToRemove)
        {
            _unitCells.Remove(key);
            removedCount++;
        }

        if (removedCount > 0)
        {
            Debug.Log($"[GridManager] 죽은 유닛 {removedCount}개의 점유 정보 제거됨");
        }
    }

    /// <summary>
    /// 특정 유닛의 점유 정보를 즉시 해제 (외부에서 호출용)
    /// </summary>
    public void FreeUnit(GameObject unit)
    {
        if (unit == null) return;

        if (_unitCells.TryGetValue(unit, out var cells))
        {
            Debug.Log($"[GridManager] {unit.name} 점유 해제: {cells.Count}개 셀");
            _unitCells.Remove(unit);
        }
    }

    /// <summary>
    /// 유닛의 실제 위치와 점유 정보가 일치하는지 검증하고 정리
    /// </summary>
    public void ValidateUnitPositions()
    {
        var toUpdate = new Dictionary<GameObject, HashSet<Vector3Int>>();
        var toRemove = new List<GameObject>();

        foreach (var kvp in _unitCells)
        {
            var unit = kvp.Key;
            var occupiedCells = kvp.Value;

            if (unit == null)
            {
                toRemove.Add(unit);
                continue;
            }

            // 유닛의 현재 위치 확인
            var currentCell = WorldToCell(unit.transform.position);
            var expectedCells = GetCellsFor(unit, currentCell);

            // 점유 셀이 현재 위치와 다른 경우
            if (!occupiedCells.SetEquals(expectedCells))
            {
                Debug.Log($"[GridManager] {unit.name}의 위치 불일치 감지 - 점유 정보 업데이트");
                toUpdate[unit] = expectedCells;
            }
        }

        // 업데이트 적용
        foreach (var kvp in toUpdate)
        {
            _unitCells[kvp.Key] = kvp.Value;
        }

        // 제거
        foreach (var unit in toRemove)
        {
            _unitCells.Remove(unit);
        }
    }

    /// <summary>
    /// 디버그용 - 모든 점유 정보 초기화
    /// </summary>
    [ContextMenu("Clear All Occupied Cells")]
    public void ClearAllOccupiedCells()
    {
        _unitCells.Clear();
        Debug.Log("[GridManager] 모든 점유 정보가 초기화되었습니다.");
    }

    /// <summary>
    /// 디버그용 - 유닛 위치 검증
    /// </summary>
    [ContextMenu("Validate Unit Positions")]
    public void DebugValidatePositions()
    {
        ValidateUnitPositions();
    }
}