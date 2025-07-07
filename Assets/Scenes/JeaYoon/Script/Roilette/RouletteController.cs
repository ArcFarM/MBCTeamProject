using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JeaYoon.Roulette
{
    public class RouletteController : MonoBehaviour
    {
        // 🎯 회전하는 룰렛 휠 (슬롯들을 자식으로 가짐)
        public RectTransform rouletteWheel;

        // 🎯 최대 회전 속도
        public float maxSpeed = 1000f;

        // 🎯 슬롯 하나의 높이 (간격 계산에 사용)
        public float itemHeight = 100f;

        // 🎯 시작 / 정지 버튼
        public Button startButton;
        public Button stopButton;

        // 🎯 룰렛 결과 설명을 출력할 텍스트
        public TextMeshProUGUI descriptionText;

        // 내부 상태값들
        private bool isSpinning = false;        // 룰렛이 회전 중인지 여부
        private bool isStopping = false;        // 감속 중인지 여부
        private Coroutine spinCoroutine;        // 회전 코루틴 핸들

        // 시작 횟수 제한
        private int startCount = 0;             // Start 버튼 누른 횟수
        private const int maxStartCount = 100;  // Start 버튼 최대 허용 횟수

        // 룰렛 효과 설명 목록 (슬롯 인덱스에 대응)
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
            "11번 효과: 공격속도 2배 증가!",
        };

        // ▶ Start 버튼 눌렀을 때 실행되는 함수
        public void StartRoulette()
        {
            if (!isSpinning && startCount < maxStartCount)
            {
                isStopping = false;
                spinCoroutine = StartCoroutine(SpinRoutine());
                startCount++;

                // 최대 횟수에 도달하면 버튼 비활성화
                if (startCount >= maxStartCount)
                {
                    startButton.interactable = false;
                }
            }
        }

        // ▶ Stop 버튼 눌렀을 때 감속 시작
        public void StopRoulette()
        {
            isStopping = true;
        }

        // ▶ 룰렛 회전 코루틴
        private IEnumerator SpinRoutine()
        {
            isSpinning = true;
            float speed = maxSpeed;

            while (true)
            {
                // 감속 처리
                if (isStopping)
                {
                    speed = Mathf.MoveTowards(speed, 0, Time.deltaTime * 300f);

                    if (speed <= 20f)
                        break; // 속도가 충분히 느려지면 회전 종료
                }

                // 룰렛 휠 이동 (위쪽으로 계속 움직임)
                rouletteWheel.anchoredPosition += Vector2.up * speed * Time.deltaTime;

                // 슬롯 루프 처리 (화면 위로 지나간 슬롯을 아래로 재배치)
                float halfHeight = (itemHeight * rouletteWheel.childCount) / 2f;

                for (int i = 0; i < rouletteWheel.childCount; i++)
                {
                    RectTransform slot = rouletteWheel.GetChild(i) as RectTransform;
                    float slotGlobalY = slot.localPosition.y + rouletteWheel.anchoredPosition.y;

                    // 슬롯이 반 바퀴 이상 넘어가면 뒤로 보냄
                    if (slotGlobalY > halfHeight)
                    {
                        float newY = slot.localPosition.y - itemHeight * rouletteWheel.childCount;
                        slot.localPosition = new Vector2(slot.localPosition.x, newY);
                    }
                }

                yield return null;
            }

            // ▶ 정지 직후 Lerp로 정중앙에 가까운 위치로 정렬
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

            // ▶ 정지 후, 중앙에 가장 가까운 슬롯 찾기
            float centerY = 0f;
            float closestDistance = float.MaxValue;
            int selectedIndex = 0;

            for (int i = 0; i < rouletteWheel.childCount; i++)
            {
                RectTransform slot = rouletteWheel.GetChild(i) as RectTransform;
                float slotGlobalY = slot.localPosition.y + rouletteWheel.anchoredPosition.y;

                float distance = Mathf.Abs(slotGlobalY - centerY);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    selectedIndex = i;
                }
            }

            // ▶ 효과 설명 출력 (슬롯 수가 descriptions보다 많을 경우 대비)
            int actualIndex = selectedIndex % effectDescriptions.Length;

            if (actualIndex >= 0 && actualIndex < effectDescriptions.Length)
            {
                descriptionText.text = effectDescriptions[actualIndex];
            }
            else
            {
                descriptionText.text = "설명 없음";
            }

            // ▶ 콘솔 로그 출력
            Debug.Log($"🎯 선택된 인덱스: {actualIndex + 1}");
        }
    }
}
