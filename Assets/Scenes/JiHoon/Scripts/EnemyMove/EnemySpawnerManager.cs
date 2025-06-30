using System;
using System.Linq;
using UnityEngine;

namespace JiHoon
{
    [Serializable]
    public class SpawnPointInfo
    {
        public Transform spawnPoint;    // 출발 위치
        public Transform[] viaPoints;   // 경유할 포인트 그룹
    }

    public class EnemySpawnerManager : MonoBehaviour
    {
        [Header("도착 지점 (EndPoint)")]
        public Transform endPoint;

        [Header("스폰 지점들 (SpawnPointInfo 배열)")]
        public SpawnPointInfo[] spawnPoints;

        /// <summary>
        /// 지정한 프리팹을 spawnPoints[index] 위치에 인스턴스화하고,
        /// EnemyMovement 컴포넌트의 start/end/via 세팅을 전부 처리합니다.
        /// </summary>
        /// <param name="spawnPointIndex">spawnPoints 배열 인덱스</param>
        /// <param name="prefab">인스턴스화할 프리팹</param>
        public void SpawnPrefabAt(int spawnPointIndex, GameObject prefab)
        {
            // 안전 처리
            if (spawnPoints == null ||
                spawnPointIndex < 0 ||
                spawnPointIndex >= spawnPoints.Length)
            {
                Debug.LogError($"[{name}] Invalid spawnPointIndex {spawnPointIndex}");
                return;
            }

            var info = spawnPoints[spawnPointIndex];
            // 1) 인스턴스화
            var go = Instantiate(prefab, info.spawnPoint.position, Quaternion.identity);

            // 2) EnemyMovement 컴포넌트 세팅
            var mv = go.GetComponent<EnemyMovement>();
            if (mv == null)
            {
                Debug.LogWarning($"[{name}] Spawned prefab '{prefab.name}' has no EnemyMovement");
                return;
            }

            mv.startPoint = info.spawnPoint;
            mv.endPoint = endPoint;

            // 3) 경유 포인트(viaPoints) 설정
            if (info.viaPoints != null && info.viaPoints.Length > 0)
            {
                var group = info.viaPoints[
                    UnityEngine.Random.Range(0, info.viaPoints.Length)
                ];
                mv.viaPoints = group
                  .GetComponentsInChildren<Transform>()
                  .Where(t => t != group)
                  .ToArray();
            }
        }
    }
}