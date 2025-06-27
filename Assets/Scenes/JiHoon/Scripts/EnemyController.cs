using UnityEngine;
using System.Collections;
namespace JiHoon
{
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
}