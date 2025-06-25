using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public Transform endPoint;
    public float spawnInterval = 2f;
    public int maxEnemies = 10;
    public bool autoStart = true;

    [Header("Wave Settings")]
    public bool useWaves = false;
    public int enemiesPerWave = 5;
    public float timeBetweenWaves = 10f;

    private int currentEnemyCount = 0;
    private int totalSpawned = 0;
    private bool isSpawning = false;
    private int currentWave = 1;

    void Start()
    {
        if (autoStart)
        {
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;

            if (useWaves)
            {
                StartCoroutine(SpawnWaves());
            }
            else
            {
                StartCoroutine(SpawnEnemies());
            }
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    IEnumerator SpawnEnemies()
    {
        while (isSpawning && totalSpawned < maxEnemies)
        {
            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        Debug.Log("Spawning completed!");
    }

    IEnumerator SpawnWaves()
    {
        while (isSpawning)
        {
            Debug.Log($"Wave {currentWave} starting!");

            // 웨이브당 적 스폰
            for (int i = 0; i < enemiesPerWave && totalSpawned < maxEnemies; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnInterval);
            }

            if (totalSpawned >= maxEnemies)
            {
                break;
            }

            // 웨이브 간 대기
            Debug.Log($"Wave {currentWave} completed! Next wave in {timeBetweenWaves} seconds.");
            yield return new WaitForSeconds(timeBetweenWaves);
            currentWave++;
        }

        Debug.Log("All waves completed!");
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPoint == null)
        {
            Debug.LogError("Enemy prefab or spawn point is not assigned!");
            return;
        }

        // 적 생성
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // SmoothEnemyMovement 컴포넌트 설정
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if (movement != null)
        {
            movement.startPoint = spawnPoint;
            movement.endPoint = endPoint;
        }

        // 적 파괴 시 카운트 감소를 위한 컴포넌트 추가
        EnemyController controller = enemy.GetComponent<EnemyController>();
        if (controller == null)
        {
            controller = enemy.AddComponent<EnemyController>();
        }
        controller.spawner = this;

        currentEnemyCount++;
        totalSpawned++;

        Debug.Log($"Enemy spawned! Current: {currentEnemyCount}, Total: {totalSpawned}");
    }

    public void OnEnemyDestroyed()
    {
        currentEnemyCount--;
        Debug.Log($"Enemy destroyed! Remaining: {currentEnemyCount}");
    }

    public void OnEnemyReachedEnd()
    {
        currentEnemyCount--;
        Debug.Log($"Enemy reached end! Remaining: {currentEnemyCount}");
        // 여기에 플레이어 생명력 감소 등의 로직 추가
    }
}

// 적 개체 관리용 컴포넌트
public class EnemyController : MonoBehaviour
{
    [HideInInspector]
    public EnemySpawner spawner;

    [Header("Enemy Stats")]
    public float health = 100f;
    public float maxHealth = 100f;

    private EnemyMovement movement;

    void Start()
    {
        movement = GetComponent<EnemyMovement>();

        // 목적지 도달 시 이벤트 연결
        if (movement != null)
        {
            // SmoothEnemyMovement의 OnReachedDestination에 이벤트 연결
            StartCoroutine(CheckDestinationReached());
        }
    }

    IEnumerator CheckDestinationReached()
    {
        while (gameObject != null)
        {
            if (movement != null && !movement.isMoving)
            {
                // 목적지 도달
                if (spawner != null)
                {
                    spawner.OnEnemyReachedEnd();
                }
                Destroy(gameObject);
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Max(0, health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (spawner != null)
        {
            spawner.OnEnemyDestroyed();
        }

        // 죽음 효과 등 추가 가능
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        // 오브젝트가 파괴될 때 스포너에 알림
        if (spawner != null)
        {
            spawner.OnEnemyDestroyed();
        }
    }
}