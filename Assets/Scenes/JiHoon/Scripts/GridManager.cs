using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap baseTilemap;    // ���� �� Ÿ�ϸ� (������ ���� ��� �ִ�)
    public Tilemap rangeTilemap;   // ��ġ ���� ���� ���̶���Ʈ��
    public Tilemap previewTilemap; // ���콺 ���� �������

    [Header("Tiles")]
    public TileBase roadTile;               // ������ �� Ÿ�� (��ġ ��� ����)
    public Tile rangeHighlightTile;     // �Ķ��� ������ ���� Ÿ��
    public Tile placementPreviewTile;   // ������ ������ ������ Ÿ��

    // ���ο����� ���̴� ���� �� ��ǥ ���
    public List<Vector3Int> roadCells;

    void Start()
    {
        // �� baseTilemap ���� roadTile �� ��ġ�ϴ� ��� ���� ����
        roadCells = new List<Vector3Int>();
        var b = baseTilemap.cellBounds;
        foreach (var pos in b.allPositionsWithin)
        {
            if (baseTilemap.GetTile(pos) == roadTile)
                roadCells.Add(pos);
        }
    }

    /// <summary>
    /// �� �̸� �����ص� ����(roadCells)�� �Ķ������� ���̶���Ʈ
    /// </summary>
    public void HighlightAllowedCells()
    {
        rangeTilemap.ClearAllTiles();
        foreach (var cell in roadCells)
            rangeTilemap.SetTile(cell, rangeHighlightTile);
    }

    /// <summary>
    /// �� ������ ���� Ÿ�ϸ� Ŭ����
    /// </summary>
    public void ClearPreview()
    {
        previewTilemap.ClearAllTiles();
    }

    /// <summary>
    /// �� Ư�� ������ ������ ������ ��� (���� ���� ����)
    /// </summary>
    public void PreviewCell(Vector3Int cell)
    {
        previewTilemap.ClearAllTiles();
        if (roadCells.Contains(cell))
            previewTilemap.SetTile(cell, placementPreviewTile);
    }

    /// <summary>
    /// �� ��ü ���̶���Ʈ(����+������) �� ���� �����
    /// </summary>
    public void ClearAllHighlights()
    {
        rangeTilemap.ClearAllTiles();
        previewTilemap.ClearAllTiles();
    }

    // ��ǥ ��ȯ ���۵�
    public Vector3Int WorldToCell(Vector3 worldPos)
        => baseTilemap.WorldToCell(worldPos);

    public Vector3 CellToWorldCenter(Vector3Int cellPos)
    {
        Vector3 bl = baseTilemap.CellToWorld(cellPos);
        return bl + baseTilemap.cellSize * 0.5f;
    }
}