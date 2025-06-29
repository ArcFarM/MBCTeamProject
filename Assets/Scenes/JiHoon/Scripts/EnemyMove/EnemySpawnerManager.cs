using System.Linq;
using UnityEngine;
using System.Collections;
using System;
using static UnityEngine.EventSystems.EventTrigger;
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

        public bool autoStart = false; // 자동 시작 여부
        public event Action OnWaveFinished; // 웨이브 완료 이벤트
        private int _finishedSpawnPoints = 0; // 완료된 스폰 포인트 수

        void Start()
        {
            if(autoStart)
            {
                StartWave();
            }
            _finishedSpawnPoints = 0;
            //foreach (var info in spawnPoints)
            //{
            //    info.currentEnemyCount = 0;
            //    info.totalSpawned = 0;

            //    // 웨이브 방식이면 웨이브 시작
            //    StartCoroutine(SpawnEnemiesFromPoint(info));
            //}
        }

        //외부에서 호출해서 웨이브 시작할 수 있도록
        public void StartWave()
        {
            _finishedSpawnPoints = 0;
            foreach (var info in spawnPoints)
            {
                info.currentEnemyCount = 0;
                info.totalSpawned = 0;
                StartCoroutine(SpawnEnemiesFromPoint(info));
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

            // 모든 적이 생성되면 웨이브 완료 처리
            _finishedSpawnPoints++;
            if (_finishedSpawnPoints >= spawnPoints.Length)
            {
                //남은 모든 적이 씬에서 완전히 사라질 때까지 대기
                yield return new WaitUntil(() => FindObjectsOfType<EnemyMovement>().Length == 0);
                // 모든 스폰 포인트가 완료되면 이벤트 발생
                OnWaveFinished?.Invoke();

            }
        }

        void SpawnEnemy(SpawnPointInfo info)
        {
            var go = Instantiate(enemyPrefab, info.spawnPoint.position, Quaternion.identity);
            var mv = go.GetComponent<EnemyMovement>();
            mv.startPoint = info.spawnPoint;
            mv.endPoint = endPoint;
            // viaPoints 설정 (기존 로직 그대로)
            if (info.viaPoints?.Length > 0)
            {
                var group = info.viaPoints[UnityEngine.Random.Range(0, info.viaPoints.Length)];
                mv.viaPoints = group
                  .GetComponentsInChildren<Transform>()
                  .Where(t => t != group)
                  .ToArray();
            }

            info.currentEnemyCount++;
            info.totalSpawned++;
            // EnemyMovement 쪽에서 OnDeath 시 info.currentEnemyCount-- 해 주시면 완전!
        }
    }
}