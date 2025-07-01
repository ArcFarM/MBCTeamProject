using System.Collections.Generic;
using UnityEngine;
namespace JiHoon
{
    [System.Serializable]
    public class  WaveEnemyInfo
    {
        public GameObject prefab;   //스폰할 적 프리팹
        public int count;   //스폰할 적의 수
        public float spawnInterval;   //스폰 간격
        public int spawnerIndex; //적을 스폰할 스포너 인덱스
        
    }
    [CreateAssetMenu(menuName = "Game/WaveConfig")]
    public class WaveConfig : ScriptableObject
    {
        public string waveName;  //웨이브 이름
        public bool isBossWave;     //보스 웨이브 여부
        public GameObject bossPrefab;  //보스 프리팹 (보스 웨이브일 때만 사용)
        public List<WaveEnemyInfo> enemies; //웨이브에 포함된 적 정보 리스트
    }
}