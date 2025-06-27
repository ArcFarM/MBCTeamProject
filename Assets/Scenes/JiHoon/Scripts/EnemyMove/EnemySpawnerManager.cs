using System.Linq;
using UnityEngine;
using System.Collections;
namespace JiHoon
{
    [System.Serializable]
    public class SpawnPointInfo
    {
        public Transform spawnPoint;    // 출발 위치
        public Transform[] viaPoints; //중간에 경유하는 Transform 그룹
        public int maxEnemies = 10; // 최대 생성 적 수
        public float spawnInterval = 2f; // 생성 간격
        public int enemiesPerWave = 5;  // 한 웨이브당 생성 적 수
        public bool useWaves = false;   // 웨이브 방식 사용 여부
        // ... 필요하면 추가 ...
        [HideInInspector] public int currentEnemyCount = 0; // 현재 생성된 적 수
        [HideInInspector] public int totalSpawned = 0;  // 총 생성된 적 수
    }

    public class EnemySpawnerManager : MonoBehaviour
    {
        public GameObject enemyPrefab;  //생성할 Enemy 프리팹
        public Transform endPoint;  //생성된 Enemy의 도착 위치
        public SpawnPointInfo[] spawnPoints;    // 출발 위치 정보 배열

        public bool autoStart = true;   // 자동 시작 여부

        void Start()
        {
            if (autoStart)
            {
                foreach (var info in spawnPoints)
                {
                    StartCoroutine(SpawnEnemiesFromPoint(info));
                }
            }
        }

        IEnumerator SpawnEnemiesFromPoint(SpawnPointInfo info)
        {
            while (info.totalSpawned < info.maxEnemies)
            {
                if (info.currentEnemyCount < info.maxEnemies)
                {
                    SpawnEnemy(info);
                    yield return new WaitForSeconds(info.spawnInterval);
                }
                else
                {
                    yield return new WaitForSeconds(0.5f); //적이 남아있다면 대기 시간
                }
            }
        }

        void SpawnEnemy(SpawnPointInfo info)
        {
            GameObject enemy = Instantiate(enemyPrefab, info.spawnPoint.position, Quaternion.identity);

            var movement = enemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.startPoint = info.spawnPoint;
                movement.endPoint = endPoint;

                // "경유지 그룹" 중 하나 랜덤 선택
                if (info.viaPoints != null && info.viaPoints.Length > 0)
                {
                    // 랜덤 선택
                    var groupRoot = info.viaPoints[Random.Range(0, info.viaPoints.Length)];
                    // 이 그룹의 자식들만 가져오기
                    var viaArray = groupRoot.GetComponentsInChildren<Transform>()
                                            .Where(t => t != groupRoot)
                                            .ToArray();
                    movement.viaPoints = viaArray;
                }
                else
                {
                    movement.viaPoints = null;
                }
            }
        }
    }
}
