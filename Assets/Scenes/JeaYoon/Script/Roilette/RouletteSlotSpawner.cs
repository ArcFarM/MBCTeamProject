using UnityEngine;

namespace JeaYoon.Roulette
{
    public class RouletteSlotSpawner : MonoBehaviour
    {
        // ▶ 슬롯 프리팹 (TextMeshProUGUI를 포함한 단일 슬롯 UI 오브젝트)
        public GameObject slotPrefab;

        // ▶ 슬롯들이 배치될 부모 오브젝트 (예: Wheel RectTransform)
        public RectTransform slotParent;

        // ▶ 슬롯 하나의 높이 (슬롯 간 간격 계산에 사용됨)
        public float slotHeight = 100f;

        // ▶ 슬롯에 표시될 텍스트들 (효과 설명)
        [TextArea]
        public string[] slotTexts = new string[]
        {
            "1번 효과", "2번 효과", "3번 효과", "4번 효과", "5번 효과",
            "6번 효과", "7번 효과", "8번 효과", "9번 효과", "10번 효과"
        };

        // ▶ Start()에서 자동 실행 → 게임 시작 시 슬롯 자동 생성
        void Start()
        {
            SpawnSlots();
        }

        // ▶ 슬롯을 생성하여 부모에 배치하는 함수
        void SpawnSlots()
        {
            // ▶ 슬롯을 2배로 생성 (무한 스크롤 느낌을 위한 반복)
            int totalSlots = slotTexts.Length * 2;

            for (int i = 0; i < totalSlots; i++)
            {
                // ▶ 프리팹을 부모(slotParent) 아래에 인스턴스화
                GameObject newSlot = Instantiate(slotPrefab, slotParent);

                // ▶ 슬롯의 위치 설정 (위에서 아래로 차곡차곡 배치)
                RectTransform rt = newSlot.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(0, -i * slotHeight);  // 위에서 아래로 쌓음

                // ▶ 슬롯 텍스트 설정 (i % slotTexts.Length 로 순환)
                RouletteSlot slotScript = newSlot.GetComponent<RouletteSlot>();
                if (slotScript != null)
                {
                    slotScript.SetText(slotTexts[i % slotTexts.Length]);
                }

                // ▶ 슬롯 이름 지정 (에디터에서 보기 편하도록)
                newSlot.name = $"Slot_{i + 1}";
            }
        }
    }
}
