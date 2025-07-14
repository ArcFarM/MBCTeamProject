using UnityEngine;
using JeaYoon.Roulette;

namespace JeaYoon.Roulette
{
    public class RouletteSlotSpawner : MonoBehaviour
    {
        public GameObject slotPrefab;
        public RectTransform slotParent;
        public float slotHeight = 50f;

        void Start()
        {
            SpawnSlots();
        }

        void SpawnSlots()
        {
            int repetitions = 4;
            int totalSlots = RouletteData.SlotTexts.Length * repetitions;

            Debug.Log($"RouletteSlotSpawner: {totalSlots}개 슬롯 생성 시작");

            for (int i = 0; i < totalSlots; i++)
            {
                GameObject newSlot = Instantiate(slotPrefab, slotParent);
                RectTransform rt = newSlot.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(0, (totalSlots - 1 - i) * slotHeight);

                RouletteSlot slotScript = newSlot.GetComponent<RouletteSlot>();
                if (slotScript == null)
                {
                    Debug.LogError($"슬롯 {i}: RouletteSlot 스크립트가 없습니다!");
                    continue;
                }

                if (slotScript.slotLabel == null)
                {
                    Debug.LogError($"슬롯 {i}: slotLabel이 연결되지 않았습니다!");
                    continue;
                }

                int textIndex = i % RouletteData.SlotTexts.Length;
                string targetText = RouletteData.SlotTexts[textIndex];

                // 텍스트 설정
                slotScript.SetText(targetText);

                // 설정 확인
                string actualText = slotScript.slotLabel.text;
                if (actualText != targetText)
                {
                    Debug.LogWarning($"슬롯 {i}: 텍스트 설정 실패! 목표='{targetText}', 실제='{actualText}'");

                    // 직접 설정 시도
                    slotScript.slotLabel.text = targetText;
                    slotScript.slotLabel.SetAllDirty();
                }

                newSlot.name = $"Slot_{i + 1}_{targetText}";
                Debug.Log($"슬롯 {i} 생성: 이름='{newSlot.name}', 텍스트='{actualText}'");
            }

            float centerOffset = -(RouletteData.SlotTexts.Length * slotHeight * 0.5f);
            slotParent.anchoredPosition = new Vector2(0, centerOffset);

            Debug.Log($"RouletteSlotSpawner: 슬롯 생성 완료, 총 {totalSlots}개");
        }
    }
}