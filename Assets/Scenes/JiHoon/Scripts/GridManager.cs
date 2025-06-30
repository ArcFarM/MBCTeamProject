using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap baseTilemap;    // 실제 맵 (도로)
    public Tilemap rangeTilemap;   // 설치 가능 범위 하이라이트
    public Tilemap previewTilemap; // 마우스 오버 프리뷰

    [Header("Tiles")]
    public TileBase roadTile;
    public Tile rangeHighlightTile;
    public Tile placementPreviewTile;

    // 도로 셀 목록 (ReadOnly)
    public List<Vector3Int> roadCells { get; private set; }

    // *** 유닛별 점유 셀 기록 ***
    private Dictionary<GameObject, HashSet<Vector3Int>> _unitCells
        = new Dictionary<GameObject, HashSet<Vector3Int>>();

    void Start()
    {
        // roadCells 초기화
        roadCells = new List<Vector3Int>();
        var bounds = baseTilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
            if (baseTilemap.GetTile(pos) == roadTile)
                roadCells.Add(pos);
    }

    // 도로인지 확인
    public bool IsRoadCell(Vector3Int cell)
        => roadCells != null && roadCells.Contains(cell);

    // 전체 하이라이트
    public void HighlightAllowedCells()
    {
        rangeTilemap.ClearAllTiles();
        foreach (var c in roadCells)
            rangeTilemap.SetTile(c, rangeHighlightTile);
    }

    // 미리보기 클리어
    public void ClearPreview() => previewTilemap.ClearAllTiles();

    // (Optional) 한 셀만 프리뷰
    public void PreviewCell(Vector3Int cell)
    {
        previewTilemap.ClearAllTiles();
        if (IsRoadCell(cell))
            previewTilemap.SetTile(cell, placementPreviewTile);
    }

    // 전체 하이라이트 다 지우기
    public void ClearAllHighlights()
    {
        rangeTilemap.ClearAllTiles();
        previewTilemap.ClearAllTiles();
    }

    // 좌표 변환
    public Vector3Int WorldToCell(Vector3 worldPos)
        => baseTilemap.WorldToCell(worldPos);

    public Vector3 CellToWorldCenter(Vector3Int cellPos)
    {
        var bl = baseTilemap.CellToWorld(cellPos);
        return bl + baseTilemap.cellSize * 0.5f;
    }

    //
    // === 여기서부터 유닛 점유 관련 메서드들 ===
    //

    /// <summary>
    /// 특정 유닛이 점유 중인 셀 목록을 반환합니다.
    /// 등록된 정보가 없으면 빈 Set을 돌려줍니다.
    /// </summary>
    public HashSet<Vector3Int> GetOccupiedCellsFor(GameObject unit)
    {
        if (_unitCells.TryGetValue(unit, out var set))
            // 복사본 돌려서 외부 수정 금지
            return new HashSet<Vector3Int>(set);
        return new HashSet<Vector3Int>();
    }

    /// <summary>
    /// 주어진 셀들을 해제합니다.
    /// 해제 대상 셀을 포함하고 있는 유닛 엔트리를 모두 제거합니다.
    /// </summary>
    public void FreeCells(HashSet<Vector3Int> cells)
    {
        // 겹치는 엔트리를 찾아서 모두 제거
        var toRemove = _unitCells
            .Where(kv => kv.Value.Overlaps(cells))
            .Select(kv => kv.Key)
            .ToList();
        foreach (var key in toRemove)
            _unitCells.Remove(key);
    }

    /// <summary>
    /// 특정 유닛이 주어진 셀들을 차지했다고 등록합니다.
    /// 이전에 같은 유닛으로 등록된 셀은 덮어씌워집니다.
    /// </summary>
    public void OccupyCells(HashSet<Vector3Int> cells, GameObject unit)
    {
        if (unit == null) return;
        // 복사본 저장
        _unitCells[unit] = new HashSet<Vector3Int>(cells);
    }

    // 모든 유닛이 점유한 셀의 합집합을 리턴
    public HashSet<Vector3Int> GetAllOccupiedCells()
    {
        var result = new HashSet<Vector3Int>();
        foreach (var kv in _unitCells.Values)
            result.UnionWith(kv);
        return result;
    }
    // 특정 유닛이, baseCell(왼쪽 하단 셀) 기준으로 차지할 footprint 셀 목록을 리턴
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