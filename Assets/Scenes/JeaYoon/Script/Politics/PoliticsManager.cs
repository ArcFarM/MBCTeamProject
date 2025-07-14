using UnityEngine;

namespace JeaYoon.Politics
{
    public class PoliticsManager : MonoBehaviour
    {

        // [1] Variable.
        #region ▼▼▼▼▼ Variable ▼▼▼▼▼
        private float dominanceRatio = 0.5f;        // ) 현재 지배 비율 (0 ~ 1 범위로 가정).
        private float dissatisfaction = 0f;         // ) 현재 불만 수치.
        #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲





        // [X] RouletteEffectManager.
        #region ▼▼▼▼▼ RouletteEffectManager ▼▼▼▼▼
        // [◆] - ▶▶▶ GetDominanceRatio → HD_004: 부정 부패.
        public float GetDominanceRatio()
        {
            return Mathf.Clamp01(dominanceRatio); // 0~1 사이로 제한
            // 설명 : 부정 부패로인해 재정과 불만이 끊임 없이 늘어납니다. 지배의 비율에 비례해 불만이 감소 합니다.
        }
        public void AddDissatisfaction(float amount)
        {
            dissatisfaction += amount;
            dissatisfaction = Mathf.Max(0f, dissatisfaction); // 음수 방지
            Debug.Log($"📛 불만 증가: {amount:F2}, 현재 총 불만: {dissatisfaction:F2}");
        }


        // [◆] - ▶▶▶ SetPolicyCardDistribution → HD_021: 모 아니면 도.
        public void SetPolicyCardDistribution(float low, float mid, float high, float crisis)
        {
            float total = low + mid + high + crisis;
            if (Mathf.Abs(total - 1f) > 0.01f)
            {
                Debug.LogWarning("🟨 정책 카드 확률 총합이 1이 아닙니다. 자동 보정합니다.");
                low /= total;
                mid /= total;
                high /= total;
                crisis /= total;
            }
            Debug.Log($"🃏 정책 카드 분포 설정됨 → 하급: {low:P0}, 중급: {mid:P0}, 상급: {high:P0}, 위기: {crisis:P0}");
            // 실제 정책 카드 확률 적용 로직은 여기에 구현
        }
        #endregion ▲▲▲▲▲ RouletteEffectManager ▲▲▲▲▲
    }
}