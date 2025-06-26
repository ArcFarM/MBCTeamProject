using System.Linq;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpawnPointInfo
{
    public Transform spawnPoint;
    public Transform[] viaPoints; // <<--- 웨이포인트 배열 추가!
    public int maxEnemies = 10;
    public float spawnInterval = 2f;
    public int enemiesPerWave = 5;
    public bool useWaves = false;
    // ... 필요하면 추가 ...
    [HideInInspector] public int currentEnemyCount = 0;
    [HideInInspector] public int totalSpawned = 0;
}

public class EnemySpawnerManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform endPoint;
    public SpawnPointInfo[] spawnPoints;

    public bool autoStart = true;

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
                yield return new WaitForSeconds(0.5f);
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