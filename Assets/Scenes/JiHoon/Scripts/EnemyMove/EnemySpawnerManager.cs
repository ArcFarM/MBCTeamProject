using UnityEngine;
using System.Linq;

namespace JiHoon {
    [System.Serializable]
    public class SpawnPointData {
        public string name;
        public Transform spawnPoint;

        [Header("경로 설정 (2개의 경로 중 랜덤 선택)")]
        public Transform waypointsParentA; // 첫 번째 경로
        public Transform waypointsParentB; // 두 번째 경로

        [HideInInspector]
        public Transform[] waypointsA; // A 경로의 웨이포인트들
        [HideInInspector]
        public Transform[] waypointsB; // B 경로의 웨이포인트들

        // 랜덤하게 경로 선택
        public Transform[] GetRandomPath() {
            return Random.value < 0.5f ? waypointsA : waypointsB;
        }
    }

    public class EnemySpawnerManager : MonoBehaviour {
        [Header("스폰 포인트 데이터")]
        public SpawnPointData[] spawnPointsData;

        // ★★★ 추가된 부분 ★★★
        [Header("스폰 타이머 설정")]
        [Tooltip("그룹 내 각 유닛이 소환될 때의 시간 간격입니다.")]
        [Range(0f, 2f)]
        public float spawnInterval = 0.2f;
        // ★★★★★★★★★★★★★★★

        void Awake() {
            // 게임 시작 시 자동으로 자식 웨이포인트들을 수집
            CollectChildWaypoints();
        }

        void OnValidate() {
            // Inspector에서 값이 변경될 때마다 자동 수집
            CollectChildWaypoints();
        }

        void CollectChildWaypoints() {
            if (spawnPointsData == null) return;

            foreach (var data in spawnPointsData) {
                // A 경로 수집
                if (data.waypointsParentA != null) {
                    data.waypointsA = new Transform[data.waypointsParentA.childCount];
                    for (int i = 0; i < data.waypointsParentA.childCount; i++) {
                        data.waypointsA[i] = data.waypointsParentA.GetChild(i);
                    }
                }

                // B 경로 수집
                if (data.waypointsParentB != null) {
                    data.waypointsB = new Transform[data.waypointsParentB.childCount];
                    for (int i = 0; i < data.waypointsParentB.childCount; i++) {
                        data.waypointsB[i] = data.waypointsParentB.GetChild(i);
                    }
                }
            }
        }

        // 특정 인덱스의 스폰 데이터 가져오기
        public SpawnPointData GetSpawnData(int index) {
            if (spawnPointsData != null && index >= 0 && index < spawnPointsData.Length)
                return spawnPointsData[index];

            return spawnPointsData[0];
        }

        // 랜덤 스폰 데이터 가져오기
        public SpawnPointData GetRandomSpawnData() {
            if (spawnPointsData == null || spawnPointsData.Length == 0)
                return null;

            return spawnPointsData[Random.Range(0, spawnPointsData.Length)];
        }

        // 모든 스폰 데이터 가져오기
        public SpawnPointData[] GetAllSpawnData() {
            return spawnPointsData;
        }

        void OnDrawGizmos() {
            if (spawnPointsData == null) return;

            // 먼저 웨이포인트 수집
            CollectChildWaypoints();

            foreach (var data in spawnPointsData) {
                if (data.spawnPoint == null) continue;

                // 각 경로마다 다른 색상 사용
                Color baseColor;
                switch (data.name) {
                    case "Up":
                    case "Left":
                        baseColor = Color.red;
                        break;
                    case "Middle":
                    case "Right":
                        baseColor = Color.blue;
                        break;
                    case "Down":
                    case "Straight":
                        baseColor = Color.green;
                        break;
                    default:
                        baseColor = Color.yellow;
                        break;
                }

                // 스폰 포인트 표시
                Gizmos.color = baseColor;
                Gizmos.DrawWireSphere(data.spawnPoint.position, 0.5f);

                // A 경로 그리기 (기본 색상)
                if (data.waypointsA != null && data.waypointsA.Length > 0) {
                    Gizmos.color = baseColor;
                    DrawPath(data.spawnPoint.position, data.waypointsA);
                }

                // B 경로 그리기 (살짝 어두운 색상)
                if (data.waypointsB != null && data.waypointsB.Length > 0) {
                    Gizmos.color = new Color(baseColor.r * 0.6f, baseColor.g * 0.6f, baseColor.b * 0.6f);
                    DrawPath(data.spawnPoint.position, data.waypointsB);
                }
            }
        }

        void DrawPath(Vector3 startPos, Transform[] waypoints) {
            if (waypoints.Length > 0 && waypoints[0] != null) {
                Gizmos.DrawLine(startPos, waypoints[0].position);
            }

            // 웨이포인트 경로 그리기
            for (int i = 0; i < waypoints.Length - 1; i++) {
                if (waypoints[i] != null && waypoints[i + 1] != null) {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                    Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);
                }
            }

            // 마지막 웨이포인트
            if (waypoints.Length > 0 && waypoints[waypoints.Length - 1] != null) {
                Gizmos.DrawWireSphere(waypoints[waypoints.Length - 1].position, 0.5f);
            }
        }
    }
}