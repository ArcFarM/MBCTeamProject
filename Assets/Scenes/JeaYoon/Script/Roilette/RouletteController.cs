using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace JeaYoon.Roulette
{
    public class RouletteController : MonoBehaviour
    {
        public RectTransform rouletteWheel;
        public float maxSpeed = 1000f;
        public float itemHeight = 100f;

        public Button startButton;
        public Button stopButton;

        private bool isSpinning = false;
        private bool isStopping = false;
        private Coroutine spinCoroutine;

        public void StartRoulette()
        {
            if (!isSpinning)
            {
                isStopping = false;
                spinCoroutine = StartCoroutine(SpinRoutine());
            }
        }

        public void StopRoulette()
        {
            isStopping = true;
        }

        private IEnumerator SpinRoutine()
        {
            isSpinning = true;
            float speed = maxSpeed;

            while (true)
            {
                // 감속 시작
                if (isStopping)
                {
                    speed = Mathf.MoveTowards(speed, 0, Time.deltaTime * 300f);

                    if (speed <= 20f)
                        break;
                }

                // 룰렛 이동
                rouletteWheel.anchoredPosition += Vector2.up * speed * Time.deltaTime;

                // 루프 스크롤 처리
                float halfHeight = (itemHeight * rouletteWheel.childCount) / 2f;

                for (int i = 0; i < rouletteWheel.childCount; i++)
                {
                    RectTransform slot = rouletteWheel.GetChild(i) as RectTransform;
                    float slotGlobalY = slot.localPosition.y + rouletteWheel.anchoredPosition.y;

                    if (slotGlobalY > halfHeight)
                    {
                        float newY = slot.localPosition.y - itemHeight * rouletteWheel.childCount;
                        slot.localPosition = new Vector2(slot.localPosition.x, newY);
                    }
                }

                yield return null;
            }

            // 멈춘 후 정렬: Lerp로 부드럽게 맞추기
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

            // 선택된 슬롯 인덱스 계산
            int selectedIndex = Mathf.RoundToInt(nearestY / itemHeight) % rouletteWheel.childCount;
            if (selectedIndex < 0) selectedIndex += rouletteWheel.childCount;

            Debug.Log($"🎯 룰렛 멈춤! 선택된 인덱스: {selectedIndex}");
        }
    }
}
