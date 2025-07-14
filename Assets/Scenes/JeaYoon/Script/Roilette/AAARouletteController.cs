using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class AAARouletteController : MonoBehaviour
{
    public TextMeshProUGUI slotText;      // 슬롯에 표시될 텍스트 (n번 효과)
    public TextMeshProUGUI descriptionText; // 설명 창 텍스트
    public Button startButton;
    public Button stopButton;

    private Coroutine spinningCoroutine;
    private int currentIndex = 0;
    private bool isSpinning = false;

    private void Start()
    {
        startButton.onClick.AddListener(StartSpinning);
        stopButton.onClick.AddListener(BeginSlowStop);
        UpdateTexts();
    }

    void StartSpinning()
    {
        if (!isSpinning)
        {
            spinningCoroutine = StartCoroutine(SpinRoulette());
        }
    }

    void BeginSlowStop()
    {
        if (isSpinning)
        {
            StopCoroutine(spinningCoroutine);
            StartCoroutine(SlowStopRoulette());
        }
    }

    IEnumerator SpinRoulette()
    {
        isSpinning = true;

        while (true)
        {
            currentIndex = (currentIndex + 1) % AAARouletteData.Effects.Count;
            UpdateTexts();
            yield return new WaitForSeconds(0.05f); // 빠르게 회전 중
        }
    }

    IEnumerator SlowStopRoulette()
    {
        float delay = 0.1f;

        for (int i = 0; i < 20; i++) // 감속 단계 반복
        {
            currentIndex = (currentIndex + 1) % AAARouletteData.Effects.Count;
            UpdateTexts();
            yield return new WaitForSeconds(delay);
            delay += 0.03f; // 점점 느려짐
        }

        isSpinning = false;
        UpdateTexts(); // 최종 텍스트로 설명도 갱신
    }

    void UpdateTexts()
    {
        var effect = AAARouletteData.Effects[currentIndex];
        slotText.text = effect.name;
        descriptionText.text = effect.description;
    }
}
