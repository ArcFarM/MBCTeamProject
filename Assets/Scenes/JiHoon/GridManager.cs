using UnityEngine;
using UnityEngine.Tilemaps;
namespace JiHoon
{
    public class GridManager : MonoBehaviour
    {
        [Header("TileMaps")]
        public Tilemap baseTilemap; // �⺻ Ÿ�ϸ�
        public Tilemap highlightTilemap; // ���� ǥ�ÿ� Ÿ�ϸ�

        [Header("Highlight")]
        public Tile highlightTile; // ������ ���� Ÿ��

        //���� ��ǥ => �� ��ǥ ��ȯ
        public Vector3Int WorldToCell(Vector3 worldPosition)
        {
            return baseTilemap.WorldToCell(worldPosition);
        }
        //�� ��ǥ => �� �߽� ���� ��ǥ
        public Vector3 CellToWorldCenter(Vector3Int cellPos)
        {
            Vector3 worldBL = baseTilemap.CellToWorld(cellPos); //���� ���� �Ʒ� �𼭸� ���� ��ǥ
            Vector3 halfCell = baseTilemap.cellSize / 2f; //�� ũ���� ����
            return worldBL + halfCell; //�� �߽� ���� ��ǥ                                                      
        }
        //Ư�� ���� �������� ����ư �Ÿ� ���� ǥ��
        public void HighlightRange(Vector3Int centerCell, int range)
        {
            ClearHighlights(); //���� ���̶���Ʈ ����

            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    if (Mathf.Abs(dx) + Mathf.Abs(dy) > range)
                        continue; //����ư �Ÿ� �ʰ��� �ǳʶٱ�
                    var cell = new Vector3Int(centerCell.x + dx, centerCell.y + dy, centerCell.z);
                    if (baseTilemap.HasTile(cell)) //�ش� ���� Ÿ���� �ִ��� Ȯ��
                    {
                        highlightTilemap.SetTile(cell, highlightTile); //���̶���Ʈ Ÿ�� ����
                    }
                }
            }
        }
        // (4) ���̶���Ʈ �ʱ�ȭ
        public void ClearHighlights()
        {
            highlightTilemap.ClearAllTiles();
        }
    }
}