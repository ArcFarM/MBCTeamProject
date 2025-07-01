using UnityEngine;
using System.Collections.Generic;
namespace JiHoon
{
    public class UnitGridController : MonoBehaviour
    {
        public GridManager gridManager; // 그리드 매니저 참조
                                        //셀 좌표 < > 유닛 매핑
        private Dictionary<Vector3Int, GameObject> unitsOnCell = new Dictionary<Vector3Int, GameObject>();

        //마우스 클릭 등으로 아군 배치
        public void TryPlaceUnit(GameObject unitPrefab, Vector3 worldPos)
        {
            var cell = gridManager.WorldToCell(worldPos); //월드 좌표를 셀 좌표로 변환
            if (!gridManager.placementZoneTilemap.HasTile(cell))
            {
                //셀에 타일이 없으면 배치 불가
                Debug.Log("맵 밖입니다");
                return;
            }
            if (unitsOnCell.ContainsKey(cell))
            {
                Debug.Log("이미 유닛이 있습니다!");
                return;
            }

            //유닛 배치
            Vector3 spawnPos = gridManager.CellToWorldCenter(cell); //셀 중심 위치 계산
            var unit = Instantiate(unitPrefab, spawnPos, Quaternion.identity); //유닛 생성
            unitsOnCell.Add(cell, unit); //셀과 유닛 매핑 저장
        }

        //셀 기준으로 유닛 꺼내기
        public GameObject GetUnitAt(Vector3Int cell)
        {
            unitsOnCell.TryGetValue(cell, out var unit);
            return unit;
        }
    }
}