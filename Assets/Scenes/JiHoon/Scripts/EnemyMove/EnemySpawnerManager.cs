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
        [Header("스폰된 적들을 담을 컨테이너")]
        public Transform spawnContainer; // 빈 오브젝트를 드래그

        [Header("도착 지점 (EndPoint)")]
        public Transform endPoint;

        [Header("스폰 지점들 (SpawnPointInfo 배열)")]
        public SpawnPointInfo[] spawnPoints;

        /// <summary>
        /// 지정한 프리팹을 spawnPoints[index] 위치에 인스턴스화하고,
        /// EnemyMovement 컴포넌트의 start/end/via 세팅까지 책임집니다.
        /// </summary>
        /// <param name="spawnPointIndex">spawnPoints 배열 인덱스</param>
        /// <param name="prefab">인스턴스화할 프리팹</param>
        public void SpawnPrefabAt(int spawnPointIndex, GameObject prefab)
        {
            // 1) 사전 안전 검사
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogError($"[{name}] spawnPoints 배열이 비어 있습니다.");
                return;
            }
            if (spawnPointIndex < 0 || spawnPointIndex >= spawnPoints.Length)
            {
                Debug.LogError($"[{name}] 잘못된 spawnPointIndex: {spawnPointIndex}");
                return;
            }
            if (spawnContainer == null)
            {
                Debug.LogError($"[{name}] spawnContainer가 할당되지 않았습니다.");
                return;
            }
            if (prefab == null)
            {
                Debug.LogError($"[{name}] SpawnPrefabAt에 전달된 prefab이 null입니다.");
                return;
            }

            var info = spawnPoints[spawnPointIndex];

            // 2) 인스턴스화 (부모 지정)
            var go = Instantiate(
                prefab,
                info.spawnPoint.position,
                Quaternion.identity,
                spawnContainer
            );

            // 3) EnemyMovement 컴포넌트 세팅
            var mv = go.GetComponent<EnemyMovement>();
            if (mv == null)
            {
                Debug.LogWarning($"[{name}] 생성된 '{prefab.name}'에 EnemyMovement가 없습니다.");
                return;
            }

            mv.startPoint = info.spawnPoint;
            mv.endPoint = endPoint;

            // 4) 경유 포인트(viaPoints) 설정
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