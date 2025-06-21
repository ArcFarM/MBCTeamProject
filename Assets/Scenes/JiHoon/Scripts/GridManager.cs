using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap baseTilemap;    // 실제 맵 타일맵 (베이지 길이 깔려 있는)
    public Tilemap rangeTilemap;   // 설치 가능 범위 하이라이트용
    public Tilemap previewTilemap; // 마우스 오버 프리뷰용

    [Header("Tiles")]
    public TileBase roadTile;               // 베이지 길 타일 (설치 허용 판정)
    public Tile rangeHighlightTile;     // 파란색 반투명 범위 타일
    public Tile placementPreviewTile;   // 빨간색 반투명 프리뷰 타일

    // 내부에서만 쓰이는 도로 셀 좌표 목록
    public List<Vector3Int> roadCells;

    void Start()
    {
        // ① baseTilemap 에서 roadTile 과 일치하는 모든 셀을 수집
        roadCells = new List<Vector3Int>();
        var b = baseTilemap.cellBounds;
        foreach (var pos in b.allPositionsWithin)
        {
            if (baseTilemap.GetTile(pos) == roadTile)
                roadCells.Add(pos);
        }
    }

    /// <summary>
    /// ② 미리 수집해둔 도로(roadCells)만 파란색으로 하이라이트
    /// </summary>
    public void HighlightAllowedCells()
    {
        rangeTilemap.ClearAllTiles();
        foreach (var cell in roadCells)
            rangeTilemap.SetTile(cell, rangeHighlightTile);
    }

    /// <summary>
    /// ③ 프리뷰 전용 타일맵 클리어
    /// </summary>
    public void ClearPreview()
    {
        previewTilemap.ClearAllTiles();
    }

    /// <summary>
    /// ④ 특정 셀에만 빨간색 프리뷰 찍기 (도로 셀일 때만)
    /// </summary>
    public void PreviewCell(Vector3Int cell)
    {
        previewTilemap.ClearAllTiles();
        if (roadCells.Contains(cell))
            previewTilemap.SetTile(cell, placementPreviewTile);
    }

    /// <summary>
    /// ⑤ 전체 하이라이트(범위+프리뷰) 한 번에 지우기
    /// </summary>
    public void ClearAllHighlights()
    {
        rangeTilemap.ClearAllTiles();
        previewTilemap.ClearAllTiles();
    }

    // 좌표 변환 헬퍼들
    public Vector3Int WorldToCell(Vector3 worldPos)
        => baseTilemap.WorldToCell(worldPos);

    public Vector3 CellToWorldCenter(Vector3Int cellPos)
    {
        Vector3 bl = baseTilemap.CellToWorld(cellPos);
        return bl + baseTilemap.cellSize * 0.5f;
    }
}