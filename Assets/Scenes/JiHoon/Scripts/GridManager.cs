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

    void Start()
    {
        // placementCells 초기화 (placementZoneTilemap에 칠한 타일만)
        placementCells = new HashSet<Vector3Int>();
        var bounds = placementZoneTilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
            if (placementZoneTilemap.HasTile(pos))
                placementCells.Add(pos);
    }

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
}