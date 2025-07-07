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
        /// 지정한 프리팹을 spawnPoints[index] 위치 + offset 에 인스턴스화하고,
        /// EnemyMovement 컴포넌트 세팅까지 책임집니다.
        /// </summary>
        public void SpawnPrefabAt(int spawnPointIndex, GameObject prefab, Vector3 offset)
        {

            // (안전 검사 코드는 생략하되, 기존 SpawnPrefabAt과 동일하게 처리)
            var info = spawnPoints[spawnPointIndex];
            var pos = info.spawnPoint.position + offset;
            var go = Instantiate(prefab, pos, Quaternion.identity, spawnContainer);
            

            var mv = go.GetComponent<EnemyMovement>();
            if (mv == null) return;

            mv.startPoint = info.spawnPoint;
            mv.endPoint = endPoint;

            if (info.viaPoints != null && info.viaPoints.Length > 0)
            {
                var group = info.viaPoints[UnityEngine.Random.Range(0, info.viaPoints.Length)];
                mv.viaPoints = group
                  .GetComponentsInChildren<Transform>()
                  .Where(t => t != group)
                  .ToArray();
            }
        }
    
    }
}