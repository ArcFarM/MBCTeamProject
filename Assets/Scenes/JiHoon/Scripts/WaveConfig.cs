using System.Collections.Generic;
using UnityEngine;

namespace JiHoon
{
    [System.Serializable]
    public class WaveEnemyInfo
    {
        public GameObject prefab;
        public int count;
        public float spawnInterval;
        public int spawnerIndex;
        [Header("=== 파티 스폰 옵션 ===")]
        public int groupCount = 3;
        public float groupSpacing = 1f;
        public float groupInterval = 0.5f;
    }

    [System.Serializable]
    public class GroupConfig
    {
        [Header("그룹 설정")]
        public int groupId;
        public int maxMembersPerGroup = 5;
        public float formationSpacing = 1.5f;
        public EnemyGroup.FormationType formation = EnemyGroup.FormationType.VFormation;

        [Header("이 그룹에 포함될 적들")]
        public List<WaveEnemyInfo> enemyTypes = new List<WaveEnemyInfo>();
    }

    [CreateAssetMenu(menuName = "Game/WaveConfig")]
    public class WaveConfig : ScriptableObject
    {
        [Header("웨이브 기본 설정")]
        public string waveName;
        public bool isBossWave;
        public GameObject bossPrefab;

        [Header("기존 방식 (하위 호환)")]
        public List<WaveEnemyInfo> enemies;

        [Header("새로운 그룹 방식")]
        public List<GroupConfig> groups = new List<GroupConfig>();

        [Header("그룹 사용 여부")]
        public bool useGroupSystem = false; // 체크하면 그룹 시스템 사용
    }
}