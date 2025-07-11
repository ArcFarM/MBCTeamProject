using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JeaYoon.Roulette;
using System.Collections;

namespace JeaYoon.Roulette
{
    public class RouletteController : MonoBehaviour
    {
        public RectTransform rouletteWheel;
        public RectTransform centerMarker;
        public float maxSpeed = 1000f;
        public float itemHeight = 50f; // RouletteSlotSpawner와 일치시킴

        public Button startButton;
        public Button stopButton;
        public TextMeshProUGUI descriptionText;

        private bool isSpinning = false;
        private bool isStopping = false;
        private Coroutine spinCoroutine;
        private int startCount = 0;
        private const int maxStartCount = 100;

        void Start()
        {
            // InitializeSlotTexts() 제거 - RouletteSlotSpawner가 처리
            Debug.Log($"RouletteController 시작 - 총 슬롯 개수: {rouletteWheel.childCount}");
        }

        public void StartRoulette()
        {
            if (!isSpinning && startCount < maxStartCount)
            {
                isStopping = false;
                spinCoroutine = StartCoroutine(SpinRoutine());
                startCount++;

                if (startCount >= maxStartCount)
                    startButton.interactable = false;
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
                if (isStopping)
                {
                    speed = Mathf.MoveTowards(speed, 0, Time.deltaTime * 300f);
                    if (speed <= 20f)
                        break;
                }

                rouletteWheel.anchoredPosition += Vector2.down * speed * Time.deltaTime;
                HandleInfiniteLoop();

                yield return null;
            }

            // 중앙 정렬
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

            // 선택된 슬롯 찾기 (개선된 버전)
            FindSelectedSlot();
        }

        private void FindSelectedSlot()
        {
            float centerY = centerMarker.position.y;
            float closestDistance = float.MaxValue;
            RouletteSlot selectedSlotScript = null;

            Debug.Log($"🎯 중앙 마커 Y 위치: {centerY}");
            Debug.Log($"🎯 총 슬롯 개수: {rouletteWheel.childCount}");

            for (int i = 0; i < rouletteWheel.childCount; i++)
            {
                RectTransform slot = rouletteWheel.GetChild(i) as RectTransform;
                RouletteSlot slotScript = slot.GetComponent<RouletteSlot>();

                if (slotScript != null)
                {
                    float slotY = slot.position.y;
                    float distance = Mathf.Abs(slotY - centerY);

                    Debug.Log($"슬롯 {i}: 텍스트='{slotScript.slotLabel.text}', Y={slotY}, 거리={distance}");

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        selectedSlotScript = slotScript;
                    }
                }
            }

            if (selectedSlotScript != null)
            {
                string selectedText = selectedSlotScript.slotLabel.text;
                Debug.Log($"🎯 선택된 슬롯 텍스트: '{selectedText}'");

                int effectIndex = FindEffectIndex(selectedText);
                Debug.Log($"🎯 효과 인덱스: {effectIndex}");

                if (effectIndex >= 0 && effectIndex < RouletteData.EffectDescriptions.Length)
                {
                    descriptionText.text = RouletteData.EffectDescriptions[effectIndex];
                    Debug.Log($"🎯 선택된 효과: {RouletteData.ItemIDs[effectIndex]} - {selectedText}");
                    Debug.Log($"🎯 효과 설명: {RouletteData.EffectDescriptions[effectIndex]}");
                }
                else
                {
                    Debug.LogError($"❌ 알 수 없는 슬롯 텍스트: '{selectedText}' (인덱스: {effectIndex})");
                    descriptionText.text = "알 수 없는 효과입니다.";
                }
            }
            else
            {
                Debug.LogError("❌ 선택된 슬롯을 찾을 수 없습니다.");
            }
        }

        private void HandleInfiniteLoop()
        {
            float totalHeight = itemHeight * RouletteData.SlotTexts.Length;

            for (int i = 0; i < rouletteWheel.childCount; i++)
            {
                RectTransform slot = rouletteWheel.GetChild(i) as RectTransform;
                float localY = slot.anchoredPosition.y + rouletteWheel.anchoredPosition.y;

                if (localY < -itemHeight * 1.5f)
                {
                    slot.anchoredPosition += new Vector2(0, totalHeight);
                }
                else if (localY > itemHeight * (RouletteData.SlotTexts.Length + 0.5f))
                {
                    slot.anchoredPosition -= new Vector2(0, totalHeight);
                }
            }
        }

        // RouletteController.cs의 FindEffectIndex 메서드를 이것으로 교체하세요

        private int FindEffectIndex(string selectedText)
        {
            Debug.Log($"🔍 FindEffectIndex 호출: 선택된 텍스트 = '{selectedText}'");
            Debug.Log($"🔍 RouletteData.SlotTexts 배열 길이: {RouletteData.SlotTexts.Length}");
            Debug.Log($"🔍 RouletteData.EffectDescriptions 배열 길이: {RouletteData.EffectDescriptions.Length}");
            Debug.Log($"🔍 RouletteData.ItemIDs 배열 길이: {RouletteData.ItemIDs.Length}");

            // 모든 배열 길이가 같은지 확인
            if (RouletteData.SlotTexts.Length != RouletteData.EffectDescriptions.Length ||
                RouletteData.SlotTexts.Length != RouletteData.ItemIDs.Length)
            {
                Debug.LogError("❌ RouletteData 배열들의 길이가 일치하지 않습니다!");
                Debug.LogError($"SlotTexts: {RouletteData.SlotTexts.Length}, EffectDescriptions: {RouletteData.EffectDescriptions.Length}, ItemIDs: {RouletteData.ItemIDs.Length}");
                return -1;
            }

            // 선택된 텍스트가 null이거나 빈 문자열인지 확인
            if (string.IsNullOrEmpty(selectedText))
            {
                Debug.LogError("❌ 선택된 텍스트가 null이거나 빈 문자열입니다!");
                return -1;
            }

            // 1. 정확한 매칭 시도
            for (int i = 0; i < RouletteData.SlotTexts.Length; i++)
            {
                if (RouletteData.SlotTexts[i] == selectedText)
                {
                    Debug.Log($"🔍 텍스트 '{selectedText}' 정확 매칭 성공! 인덱스: {i}");
                    return i;
                }
            }

            // 2. 공백 제거 후 매칭 시도
            string trimmedSelectedText = selectedText.Trim().Replace(" ", "");
            for (int i = 0; i < RouletteData.SlotTexts.Length; i++)
            {
                string trimmedSlotText = RouletteData.SlotTexts[i].Trim().Replace(" ", "");
                if (trimmedSlotText == trimmedSelectedText)
                {
                    Debug.Log($"🔍 텍스트 '{selectedText}' 공백 제거 후 매칭 성공! 인덱스: {i}");
                    return i;
                }
            }

            // 3. 부분 매칭 시도 (포함 관계)
            for (int i = 0; i < RouletteData.SlotTexts.Length; i++)
            {
                if (RouletteData.SlotTexts[i].Contains(selectedText) || selectedText.Contains(RouletteData.SlotTexts[i]))
                {
                    Debug.Log($"🔍 텍스트 '{selectedText}' 부분 매칭 성공! 인덱스: {i}");
                    return i;
                }
            }

            // 매칭 실패 시 디버깅 정보 출력
            Debug.LogError($"❌ 텍스트 '{selectedText}' 매칭 실패!");
            Debug.LogError("📋 사용 가능한 텍스트들:");
            for (int i = 0; i < RouletteData.SlotTexts.Length; i++)
            {
                Debug.LogError($"  [{i}] '{RouletteData.SlotTexts[i]}' (길이: {RouletteData.SlotTexts[i].Length})");
            }
            Debug.LogError($"📋 선택된 텍스트 정보: '{selectedText}' (길이: {selectedText.Length})");

            return -1;
        }

        public void ResetRoulette()
        {
            startCount = 0;
            startButton.interactable = true;
        }
    }
}