using System.Collections.Generic;
using UnityEngine;

namespace JiHoon
{
    public enum SpawnPosition
    {
        Top,        // 상단 (0)
        Middle,     // 중단 (1)
        Bottom      // 하단 (2)
    }

    [System.Serializable]
    public class EnemyGroupConfig
    {
        public string groupName = "Enemy Group";
        public List<GameObject> enemyPrefabs = new List<GameObject>();
        public int enemyCount = 2; // 기본 2마리

        [Header("스폰 설정")]
        public SpawnPosition spawnPosition = SpawnPosition.Top;
        public float enemySpacing = 0.5f; // 적 사이 간격
        public float delayAfterGroup = 2f; // 다음 그룹까지 대기 시간
    }

    [CreateAssetMenu(menuName = "Game/WaveConfig", fileName = "Wave_")]
    public class WaveConfig : ScriptableObject
    {
        public string waveName = "Wave 1";
        public List<EnemyGroupConfig> enemyGroups = new List<EnemyGroupConfig>();
    }
}