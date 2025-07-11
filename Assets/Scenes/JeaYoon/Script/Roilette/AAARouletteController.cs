using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AAARouletteController : MonoBehaviour
{
    [Header("UI Components")]
    public ScrollRect scrollRect;
    public RectTransform content;
    public GameObject slotPrefab;
    public TextMeshProUGUI descriptionText;
    public Button startButton;
    public Button stopButton;

    private List<string> slotNames = new();
    private List<string> slotDescriptions = new();
    private int currentIndex = 0;
    private bool isSpinning = false;
    private Coroutine spinCoroutine;

    private float scrollSpeed = 0.5f;

    void Start()
    {
        // 데이터 세팅
        foreach (var effect in AAARouletteData.Effects)
        {
            slotNames.Add(effect.name);
            slotDescriptions.Add(effect.description);
        }

        FillSlots();

        startButton.onClick.AddListener(StartSpinning);
        stopButton.onClick.AddListener(BeginSlowStop);
    }

    void FillSlots()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        // 슬롯 채우기
        foreach (var name in slotNames)
        {
            GameObject slot = Instantiate(slotPrefab, content);
            slot.GetComponentInChildren<TextMeshProUGUI>().text = name;
        }

        // 맨 마지막에 첫 번째 복사해서 감속용 루프 대비
        GameObject extra = Instantiate(slotPrefab, content);
        extra.GetComponentInChildren<TextMeshProUGUI>().text = slotNames[0];
    }

    void StartSpinning()
    {
        if (!isSpinning)
        {
            spinCoroutine = StartCoroutine(SpinEffect());
        }
    }

    void BeginSlowStop()
    {
        if (isSpinning)
        {
            StopCoroutine(spinCoroutine);
            StartCoroutine(SlowStopEffect());
        }
    }

    IEnumerator SpinEffect()
    {
        isSpinning = true;
        float pos = scrollRect.verticalNormalizedPosition;

        while (true)
        {
            pos += scrollSpeed * Time.deltaTime;
            if (pos > 1f) pos -= 1f; // 무한 루프처럼 반복
            scrollRect.verticalNormalizedPosition = 1f - pos;
            yield return null;
        }
    }

    IEnumerator SlowStopEffect()
    {
        float speed = scrollSpeed;
        float pos = 1f - scrollRect.verticalNormalizedPosition;

        while (speed > 0.01f)
        {
            pos += speed * Time.deltaTime;
            scrollRect.verticalNormalizedPosition = 1f - (pos % 1f);
            speed -= Time.deltaTime * 0.1f; // 감속
            yield return null;
        }

        // 멈출 때 가장 가까운 슬롯 인덱스를 계산
        float slotHeight = slotPrefab.GetComponent<RectTransform>().rect.height;
        float totalHeight = content.rect.height;
        float offsetY = content.anchoredPosition.y;

        int index = Mathf.RoundToInt(offsetY / slotHeight) % slotNames.Count;
        if (index < 0) index += slotNames.Count;

        descriptionText.text = slotDescriptions[index];

        isSpinning = false;
    }
}
