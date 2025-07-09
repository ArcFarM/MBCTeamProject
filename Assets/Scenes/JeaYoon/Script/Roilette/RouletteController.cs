using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JeaYoon.Roulette
{
    public class RouletteController : MonoBehaviour
    {
        public RectTransform rouletteWheel;          // 회전하는 룰렛 휠
        public RectTransform centerMarker;           // 🎯 룰렛 중앙 기준 (empty object를 UI 중앙에 두고 연결)
        public float maxSpeed = 100f;               // 최대 회전 속도
        public float itemHeight = 50f;              // 슬롯 하나의 높이

        public Button startButton;                   // 시작 버튼
        public Button stopButton;                    // 정지 버튼
        public TextMeshProUGUI descriptionText;      // 결과 설명 텍스트

        private bool isSpinning = false;
        private bool isStopping = false;
        private Coroutine spinCoroutine;
        private int startCount = 0;
        private const int maxStartCount = 100;

        // 룰렛 효과 설명
        private string[] effectDescriptions = new string[]
        {
            "1번 효과: 적 전체에게 피해를 줍니다.",
            "2번 효과: 체력을 회복합니다.",
            "3번 효과: 방어력을 증가시킵니다.",
            "4번 효과: 랜덤 적을 스턴시킵니다.",
            "5번 효과: 공격력 2배 증가!",
            "6번 효과: 마나를 50 회복합니다.",
            "7번 효과: 무작위 아이템 획득.",
            "8번 효과: 이동속도 증가.",
            "9번 효과: 팀원 전체 버프 부여.",
            "10번 효과: 즉시 턴 획득!",
        };

        // 슬롯 텍스트 배열 (효과 설명과 동일한 순서)
        private string[] slotTexts = new string[]
        {
            "1번 효과", "2번 효과", "3번 효과", "4번 효과", "5번 효과",
            "6번 효과", "7번 효과", "8번 효과", "9번 효과", "10번 효과"
        };

        void Start()
        {
            // 시작 시 모든 슬롯 텍스트 초기화 및 위치 설정
            InitializeSlotTexts();
        }

        public void StartRoulette()
        {
            if (!isSpinning && startCount < maxStartCount)
            {
                isStopping = false;
                spinCoroutine = StartCoroutine(SpinRoutine());
                startCount++;

                if (startCount >= maxStartCount)
                {
                    startButton.interactable = false;
                }
            }
        }

        public void StopRoulette()
        {
            isStopping = true;
        }

        // 🔧 수정: 슬롯 텍스트 초기화 및 중앙 마커 기준으로 위치 설정
        private void InitializeSlotTexts()
        {
            float startY = 100f; // 슬롯 시작 위치를 Y = +100으로 고정

            for (int i = 0; i < rouletteWheel.childCount; i++)
            {
                RectTransform slot = rouletteWheel.GetChild(i) as RectTransform;
                RouletteSlot slotScript = slot.GetComponent<RouletteSlot>();

                if (slotScript != null)
                {
                    int textIndex = i % slotTexts.Length;
                    slotScript.SetText(slotTexts[textIndex]);
                }

                // 🎯 여기에 추가!
                slot.sizeDelta = new Vector2(slot.sizeDelta.x, itemHeight);
                // 🔧 핵심 수정: 슬롯 위치를 중앙 마커 기준으로 설정
                // 첫 번째 슬롯(i=0)이 중앙 마커 위치에 오도록 설정
                float slotY = startY - (i * itemHeight);
                slot.anchoredPosition = new Vector2(slot.anchoredPosition.x, slotY);
            }

            // 🔧 추가: 룰렛 휠 자체의 위치를 0으로 초기화
            rouletteWheel.anchoredPosition = Vector2.zero;
        }

        private IEnumerator SpinRoutine()
        {
            isSpinning = true;
            float speed = maxSpeed;

            while (true)
            {
                if (isStopping)
                {
                    speed = Mathf.MoveTowards(speed, 0, Time.deltaTime * 300f);
                    if (speed <= 20f)
                        break;
                }

                // 🔧 수정: 룰렛이 아래로 이동하도록 변경 (Vector2.down 사용)
                rouletteWheel.anchoredPosition += Vector2.down * speed * Time.deltaTime;

                // 🔧 수정: 무한 순환 로직 개선
                HandleInfiniteLoop();

                yield return null;
            }

            // 부드러운 정렬
            Vector2 startPos = rouletteWheel.anchoredPosition;
            float nearestY = Mathf.Round(startPos.y / itemHeight) * itemHeight;
            Vector2 targetPos = new Vector2(startPos.x, nearestY);

            float t = 0f;
            float duration = 0.2f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                rouletteWheel.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                yield return null;
            }

            isSpinning = false;

            // 중앙에 가장 가까운 슬롯 찾기
            float centerY = centerMarker.position.y;
            float closestDistance = float.MaxValue;
            int selectedSlotIndex = 0;

            for (int i = 0; i < rouletteWheel.childCount; i++)
            {
                RectTransform slot = rouletteWheel.GetChild(i) as RectTransform;
                float slotY = slot.position.y;
                float distance = Mathf.Abs(slotY - centerY);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    selectedSlotIndex = i;
                }
            }

            // 선택된 슬롯의 텍스트 확인
            RectTransform selectedSlot = rouletteWheel.GetChild(selectedSlotIndex) as RectTransform;
            RouletteSlot selectedSlotScript = selectedSlot.GetComponent<RouletteSlot>();

            if (selectedSlotScript != null)
            {
                string selectedText = selectedSlotScript.slotLabel.text;
                Debug.Log($"🎯 선택된 슬롯 텍스트: {selectedText}");

                // 텍스트에서 숫자 추출 (예: "5번 효과" -> 5)
                int effectNumber = ExtractEffectNumber(selectedText);

                if (effectNumber >= 1 && effectNumber <= effectDescriptions.Length)
                {
                    descriptionText.text = effectDescriptions[effectNumber - 1];
                    Debug.Log($"🎯 선택된 효과: {effectNumber}번");
                }
                else
                {
                    Debug.LogError($"잘못된 효과 번호: {effectNumber}");
                }
            }
        }

        // 🔧 개별 슬롯 순환 로직 (자연스러운 무한 순환)
        private void HandleInfiniteLoop()
        {
            float totalHeight = itemHeight * slotTexts.Length;
            float resetY = 100f; // 슬롯이 되돌아오는 기준 지점

            for (int i = 0; i < rouletteWheel.childCount; i++)
            {
                RectTransform slot = rouletteWheel.GetChild(i) as RectTransform;
                float localY = slot.anchoredPosition.y + rouletteWheel.anchoredPosition.y;

                // 아래로 충분히 내려갔으면 위로 재배치 (Y = 100을 기준으로)
                if (localY < -itemHeight * 1.5f)
                {
                    slot.anchoredPosition += new Vector2(0, totalHeight);
                }
                // 위로 충분히 올라갔으면 아래로 재배치
                else if (localY > resetY + (itemHeight * 0.5f))
                {
                    slot.anchoredPosition -= new Vector2(0, totalHeight);
                }
            }
        }
      


        // 텍스트에서 효과 번호 추출
        private int ExtractEffectNumber(string text)
        {
            // "5번 효과" 형태에서 숫자 추출
            string[] parts = text.Split('번');
            if (parts.Length > 0)
            {
                if (int.TryParse(parts[0], out int number))
                {
                    return number;
                }
            }
            return 1; // 기본값
        }
        public void ResetRoulette()
        {
            startCount = 0;
            startButton.interactable = true;
        }
    }
}