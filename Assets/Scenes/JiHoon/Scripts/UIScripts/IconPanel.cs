using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class IconPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("호버 시 표시할 패널")]
    [SerializeField] private GameObject targetPanel;

    [Header("설정")]
    [SerializeField] private bool hideOnStart = true;
    [SerializeField] private float showDelay = 0f;      // 표시 지연 시간
    [SerializeField] private float hideDelay = 0f;      // 숨김 지연 시간
    [SerializeField] private bool fadeInOut = false;    // 페이드 효과 사용
    [SerializeField] private float fadeDuration = 0.3f; // 페이드 지속 시간

    private Coroutine showCoroutine;
    private Coroutine hideCoroutine;
    private CanvasGroup panelCanvasGroup;

    void Start()
    {
        if (targetPanel != null)
        {
            // 페이드 효과를 위한 CanvasGroup 컴포넌트 확인
            if (fadeInOut)
            {
                panelCanvasGroup = targetPanel.GetComponent<CanvasGroup>();
                if (panelCanvasGroup == null)
                {
                    panelCanvasGroup = targetPanel.AddComponent<CanvasGroup>();
                }
            }

            // 시작 시 패널 숨기기
            if (hideOnStart)
            {
                targetPanel.SetActive(false);
                if (panelCanvasGroup != null)
                {
                    panelCanvasGroup.alpha = 0f;
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
        }

        showCoroutine = StartCoroutine(ShowPanelWithDelay());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
            showCoroutine = null;
        }

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HidePanelWithDelay());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭 시 패널 토글 (옵션)
        // 필요하면 주석 해제
        // TogglePanel();
    }

    private IEnumerator ShowPanelWithDelay()
    {
        if (showDelay > 0)
        {
            yield return new WaitForSeconds(showDelay);
        }

        if (targetPanel != null)
        {
            targetPanel.SetActive(true);

            if (fadeInOut && panelCanvasGroup != null)
            {
                float elapsedTime = 0f;
                float startAlpha = panelCanvasGroup.alpha;

                while (elapsedTime < fadeDuration)
                {
                    elapsedTime += Time.deltaTime;
                    panelCanvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsedTime / fadeDuration);
                    yield return null;
                }

                panelCanvasGroup.alpha = 1f;
            }
        }
    }

    private IEnumerator HidePanelWithDelay()
    {
        if (hideDelay > 0)
        {
            yield return new WaitForSeconds(hideDelay);
        }

        if (targetPanel != null)
        {
            if (fadeInOut && panelCanvasGroup != null)
            {
                float elapsedTime = 0f;
                float startAlpha = panelCanvasGroup.alpha;

                while (elapsedTime < fadeDuration)
                {
                    elapsedTime += Time.deltaTime;
                    panelCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
                    yield return null;
                }

                panelCanvasGroup.alpha = 0f;
            }

            targetPanel.SetActive(false);
        }
    }

    // 패널 토글 (수동으로 호출 가능)
    public void TogglePanel()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(!targetPanel.activeSelf);
        }
    }

    // 다른 패널들 숨기기 (한 번에 하나만 표시하고 싶을 때)
    public static void HideAllPanelsExcept(GameObject exceptPanel)
    {
        //UIIconHoverPanelAdvanced[] allHoverPanels = FindObjectsOfType<UIIconHoverPanelAdvanced>();
        //foreach (var panel in allHoverPanels)
        //{
        //    if (panel.targetPanel != exceptPanel && panel.targetPanel != null)
        //    {
        //        panel.targetPanel.SetActive(false);
        //    }
        //}
    }
}