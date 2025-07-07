using UnityEngine;

namespace MainGame.Units.Animation {
    [CreateAssetMenu(fileName = "UnitsAnimationData", menuName = "Animation/UnitsAnimationData")]
    public class UnitsAnimationData : ScriptableObject {
        [Header("모든 유닛 데이터")]
        public UnitAnimFrameConfig[] unitConfigs;
    }

    [System.Serializable]
    public class UnitAnimFrameConfig {
        [Header("유닛 정보")]
        public string unitID = "UN_001";
        public string unitName = "기사";

        [Header("공격 설정")]
        public int attackDamageFrame = 15;
        public int attackCompleteFrame = 30;
        public int frameRate = 60;

        [Header("루프 설정")]
        public float idleLoopInterval = 2f;
    }
}
