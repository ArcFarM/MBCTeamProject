using UnityEngine;

namespace JeaYoon.Roulette
{
    public class RouletteSlotSpawner : MonoBehaviour
    {
        // ▶ 슬롯 프리팹 (TextMeshProUGUI 포함된 슬롯 오브젝트)
        public GameObject slotPrefab;
        // ▶ 슬롯들이 배치될 부모 오브젝트 (RectTransform)
        public RectTransform slotParent;
        // ▶ 슬롯 하나의 높이 (슬롯 간격)
        public float slotHeight = 100f;
        // ▶ 슬롯에 표시될 텍스트들
        [TextArea]
        public string[] slotTexts = new string[]
        {
            "1번 효과", "2번 효과", "3번 효과", "4번 효과", "5번 효과",
            "6번 효과", "7번 효과", "8번 효과", "9번 효과", "10번 효과"
        };

        // ▶ 시작 시 슬롯 생성
        void Start()
        {
            SpawnSlots();
        }

        // ▶ 슬롯 생성 및 배치
        void SpawnSlots()
        {
            // 🔧 수정: 무한 루프 효과를 위해 3배 생성 (연결성 보장)
            int repetitions = 3;
            int totalSlots = slotTexts.Length * repetitions;

            for (int i = 0; i < totalSlots; i++)
            {
                GameObject newSlot = Instantiate(slotPrefab, slotParent);

                // 🔧 수정: 슬롯 위치 설정 (위에서 아래로 연속 배치)
                RectTransform rt = newSlot.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(0, (totalSlots - 1 - i) * slotHeight);

                // 텍스트 설정 (순환 반복)
                RouletteSlot slotScript = newSlot.GetComponent<RouletteSlot>();
                if (slotScript != null)
                {
                    int textIndex = i % slotTexts.Length;
                    slotScript.SetText(slotTexts[textIndex]);
                }

                // 에디터에서 보기 편하게 이름 설정
                newSlot.name = $"Slot_{i + 1}_{slotTexts[i % slotTexts.Length]}";
            }

            // 🔧 수정: 첫 번째 슬롯이 화면 중앙(파란색 부분)에 오도록 설정
            float centerOffset = -(slotTexts.Length * slotHeight * 0.5f);
            slotParent.anchoredPosition = new Vector2(0, centerOffset);
        }
    }
}