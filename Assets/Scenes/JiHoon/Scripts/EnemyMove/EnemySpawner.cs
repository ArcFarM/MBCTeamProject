using UnityEngine;
using System.Collections;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoint;
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
        
    // 1) 먼저 배열 채우기
        if (spawnPoint == null || spawnPoint.Length == 0)
        {
            spawnPoint = GameObject
                .FindGameObjectsWithTag("StartPoint")
                .Select(go => go.transform)
                .ToArray();
        }

        // 2) 배열이 제대로 채워졌나 한 번 더 체크
        if (spawnPoint == null || spawnPoint.Length == 0)
        {
            Debug.LogError("No spawnPoints found! Make sure you have GameObjects tagged 'StartPoint' in the scene.");
            return;
        }

        // 3) autoStart가 true일 때만 코루틴 시작
        if (autoStart)
            StartSpawning();
        
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
                // 1) 랜덤하게 스폰 지점 하나 선택
                Transform sp = spawnPoint[Random.Range(0, spawnPoint.Length)];

                SpawnEnemy(sp);
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
        if (spawnPoint == null || spawnPoint.Length == 0)
        {
            Debug.LogError("No spawnPoints assigned!");
            yield break;
        }

        while (isSpawning)
        {
            Debug.Log($"Wave {currentWave} starting!");

            for (int i = 0; i < enemiesPerWave && totalSpawned < maxEnemies; i++)
            {
                // 올바른 변수명 사용!
                Transform sp = spawnPoint[Random.Range(0, spawnPoint.Length)];
                SpawnEnemy(sp);
                yield return new WaitForSeconds(spawnInterval);
            }

            if (totalSpawned >= maxEnemies)
                break;

            Debug.Log($"Wave {currentWave} completed! Next wave in {timeBetweenWaves} seconds.");
            yield return new WaitForSeconds(timeBetweenWaves);
            currentWave++;
        }

        Debug.Log("All waves completed!");
    }

    [SerializeField]
    public Transform[][] viaPointsGroups; // 2차원 배열처럼, Inspector에서 직접 할당

    void SpawnEnemy(Transform sp)
    {
        GameObject enemy = Instantiate(enemyPrefab, sp.position, sp.rotation);

        var movement = enemy.GetComponent<EnemyMovement>();
        if (movement != null)
        {
            movement.startPoint = sp;
            movement.endPoint = endPoint;

            // 랜덤하게 하나의 웨이포인트 그룹 선택!
            if (viaPointsGroups != null && viaPointsGroups.Length > 0)
            {
                int groupIndex = Random.Range(0, viaPointsGroups.Length);
                movement.viaPoints = viaPointsGroups[groupIndex];
            }
            else
            {
                movement.viaPoints = null; // 혹시 없으면 경유지 없음
            }
        }

        // 3) EnemyController 세팅
        var controller = enemy.GetComponent<EnemyController>()
                         ?? enemy.AddComponent<EnemyController>();
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