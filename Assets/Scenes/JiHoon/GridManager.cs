using UnityEngine;
using UnityEngine.Tilemaps;
namespace JiHoon
{
    public class GridManager : MonoBehaviour
    {
        [Header("TileMaps")]
        public Tilemap baseTilemap; // 기본 타일맵
        public Tilemap highlightTilemap; // 범위 표시용 타일맵

        [Header("Highlight")]
        public Tile highlightTile; // 범위에 찍을 타일

        //월드 좌표 => 셀 좌표 변환
        public Vector3Int WorldToCell(Vector3 worldPosition)
        {
            return baseTilemap.WorldToCell(worldPosition);
        }
        //셀 좌표 => 셀 중심 월드 좌표
        public Vector3 CellToWorldCenter(Vector3Int cellPos)
        {
            Vector3 worldBL = baseTilemap.CellToWorld(cellPos); //셀의 왼쪽 아래 모서리 월드 좌표
            Vector3 halfCell = baseTilemap.cellSize / 2f; //셀 크기의 절반
            return worldBL + halfCell; //셀 중심 월드 좌표                                                      
        }
        //특정 셀을 기준으로 맨해튼 거리 범위 표시
        public void HighlightRange(Vector3Int centerCell, int range)
        {
            ClearHighlights(); //기존 하이라이트 제거

            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    if (Mathf.Abs(dx) + Mathf.Abs(dy) > range)
                        continue; //맨해튼 거리 초과시 건너뛰기
                    var cell = new Vector3Int(centerCell.x + dx, centerCell.y + dy, centerCell.z);
                    if (baseTilemap.HasTile(cell)) //해당 셀에 타일이 있는지 확인
                    {
                        highlightTilemap.SetTile(cell, highlightTile); //하이라이트 타일 설정
                    }
                }
            }
        }
        // (4) 하이라이트 초기화
        public void ClearHighlights()
        {
            highlightTilemap.ClearAllTiles();
        }
    }
}