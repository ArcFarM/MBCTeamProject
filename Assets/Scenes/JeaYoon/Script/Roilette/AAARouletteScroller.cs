using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class RouletteScroller : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public GameObject slotPrefab; // 슬롯 하나의 프리팹 (TextMeshProUGUI 포함)
    public TextMeshProUGUI descriptionText;

    private List<RouletteEffect> effects = new();
    private float scrollSpeed = 2.0f;
    private bool isSpinning = false;

    private Coroutine spinCoroutine;

    private void Start()
    {
        effects = AAARouletteData.Effects;
        FillSlots();
    }

    void FillSlots()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        foreach (var effect in effects)
        {
            var go = Instantiate(slotPrefab, content);
            go.GetComponentInChildren<TextMeshProUGUI>().text = effect.name;
        }

        // 한 바퀴 돌 수 있게 맨 앞에 마지막, 맨 뒤에 처음 추가
        Instantiate(slotPrefab, content).GetComponentInChildren<TextMeshProUGUI>().text = effects[0].name;
    }

    public void StartSpinning()
    {
        if (!isSpinning)
            spinCoroutine = StartCoroutine(Spin());
    }

    public void StopSpinning()
    {
        if (isSpinning)
        {
            StopCoroutine(spinCoroutine);
            StartCoroutine(SlowToStop());
        }
    }

    IEnumerator Spin()
    {
        isSpinning = true;
        float position = 0f;

        while (true)
        {
            position += scrollSpeed * Time.deltaTime;
            scrollRect.verticalNormalizedPosition = 1 - (position % 1); // 반복
            yield return null;
        }
    }

    IEnumerator SlowToStop()
    {
        float speed = scrollSpeed;
        float position = 1 - scrollRect.verticalNormalizedPosition;

        while (speed > 0.05f)
        {
            position += speed * Time.deltaTime;
            scrollRect.verticalNormalizedPosition = 1 - (position % 1);
            speed -= Time.deltaTime * 0.5f; // 감속
            yield return null;
        }

        // 가장 가까운 슬롯 찾기
        float slotHeight = slotPrefab.GetComponent<RectTransform>().rect.height;
        int index = Mathf.RoundToInt((content.anchoredPosition.y) / slotHeight) % effects.Count;

        if (index < 0) index += effects.Count;
        descriptionText.text = effects[index].description;
        isSpinning = false;
    }
}
