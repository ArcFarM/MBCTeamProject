using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/* [0] 개요 : TutorialManager
		- 
*/

namespace JeaYoon.Tutorial
{
    public class TutorialManager : MonoBehaviour
    {

        // [1] Variable.
        #region ▼▼▼▼▼ Variable ▼▼▼▼▼
        // [◆] - ▶▶▶ Header → 튜토리얼 UI 요소.
        [Header("튜토리얼 UI 요소들")]
        public GameObject tutorialCanvas;           // 튜토리얼 전용 캔버스
        public Image darkBackground;                // 어두운 배경
        public GameObject speechBubble;             // 말풍선 오브젝트
        public TextMeshProUGUI speechText;          // 말풍선 텍스트
        // ) public GameObject greenSlime;                 // 그린슬라임
        // ) public GameObject highlightFrame;           // 하이라이트 프레임
        private string TileMapPlayScene = "TileMapSceneTest1";


        // [◆] - ▶▶▶ Header → 게임 UI 요소.
        [Header("게임 UI 요소들")]
        public Transform[] targetUIElements;        // 하이라이트할 UI 요소들


        // [◆] - ▶▶▶ Header → 튜토리얼 설정.
        [Header("튜토리얼 설정")]
        public string[] tutorialTexts;              // 각 단계별 설명 텍스트
        public Vector2[] speechBubblePositions;     // 말풍선 위치 오프셋


        // [◆] - ▶▶▶ 개요.
        private int currentStep = 0;
        private bool isTutorialActive = false;
        private RectTransform greenSlimetRect;
        private RectTransform highlightRect;
        private RectTransform speechBubbleRect;
        #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲





        // [2] Unity Event Method.
        #region ▼▼▼▼▼ Unity Event Method ▼▼▼▼▼
        // [◆] - ▶▶▶ Start.
        private void Start()
        {
            // [◇] - [◆] - ) 초기 설정.
            // ) greenSlimetRect = greenSlime.GetComponent<RectTransform>();
            // ) highlightRect = highlightFrame.GetComponent<RectTransform>();
            speechBubbleRect = speechBubble.GetComponent<RectTransform>();
            
            // [◇] - [◆] - ) 텍스트 컴포넌트 설정 (줄바꿈 활성화)
            SetupTextComponent();
            
            // [◇] - [◆] - ) 튜토리얼 UI 초기 비활성화.
            tutorialCanvas.SetActive(false);
            // [◇] - [◆] - ) 게임 시작 시 튜토리얼 시작.
            StartTutorial();
        }


        // [◆] - ▶▶▶ Update.
        private void Update()
        {
            // [◇] - [◆] - ) 튜토리얼이 활성화되어 있고 마우스 왼쪽 버튼을 누르면 다음 단계로 가기.
            if (isTutorialActive && Input.GetMouseButtonDown(0))
            {
                NextStep();
            }
        }

        // [◆] - ▶▶▶ GameStartButton.
        public void GameStartButton()
        {
                SceneManager.LoadScene(TileMapPlayScene);
        }
        #endregion ▲▲▲▲▲ Unity Event Method ▲▲▲▲▲





            // [3] Custom Method.
            #region ▼▼▼▼▼ Custom Method ▼▼▼▼▼

            // [◆] - ▶▶▶ SetupTextComponent (새로 추가)
        private void SetupTextComponent()
        {
            if (speechText != null)
            {
                // 자동 줄바꿈 활성화
                speechText.enableWordWrapping = true;
                
                // 텍스트 오버플로우 모드 설정 (필요에 따라)
                speechText.overflowMode = TextOverflowModes.Overflow;
                
                // 텍스트 정렬 설정 (필요에 따라 수정)
                speechText.alignment = TextAlignmentOptions.TopLeft;
            }
        }
        
        
        // [◆] - ▶▶▶ StartTutorial.
        public void StartTutorial()
        {
            isTutorialActive = true;
            currentStep = 0;

            // 튜토리얼 UI 활성화
            tutorialCanvas.SetActive(true);

            // 배경 어둡게 만들기
            StartCoroutine(FadeBackground(0f, 0.8f, 0.5f));

            // 첫 번째 단계 시작
            ShowTutorialStep(currentStep);
        }


        // [◆] - ▶▶▶ ShowTutorialStep.
        private void ShowTutorialStep(int step)
        {
            if (step >= targetUIElements.Length)
            {
                EndTutorial();
                return;
            }
            // 대상 UI 요소 위치 가져오기
            RectTransform targetRect = targetUIElements[step].GetComponent<RectTransform>();
            // 하이라이트 프레임 위치 및 크기 설정
            // ) . SetHighlightFrame(targetRect);
            // 말풍선 위치 설정
            SetSpeechBubblePosition(targetRect, step);
            // 텍스트 설정 (줄바꿈 문자 처리)
            speechText.text = ProcessTextForLineBreaks(tutorialTexts[step]);
            // UI 요소들 활성화
            // ) greenSlime.SetActive(true);
            // ) highlightFrame.SetActive(true);
            speechBubble.SetActive(true);
        }
        
        
        // [◆] - ▶▶▶ ProcessTextForLineBreaks (새로 추가)
        private string ProcessTextForLineBreaks(string originalText)
        {
            // 수동 줄바꿈이 필요한 경우 \n을 사용하여 줄바꿈 처리
            return originalText.Replace("\\n", "\n");
        }


        // [◆] - ▶▶▶ SetHighlightFrame.
        private void SetHighlightFrame(RectTransform targetRect)
        {
            // 하이라이트 프레임을 대상 UI 요소와 같은 위치, 크기로 설정
            highlightRect.position = targetRect.position;
            // 약간 여유 공간
            highlightRect.sizeDelta = targetRect.sizeDelta + new Vector2(10, 10); 
            /*
            // 하이라이트 프레임 스타일 설정 (검은색 점선 테두리, 투명한 내부)
            Image frameImage = highlightFrame.GetComponent<Image>();
            frameImage.color = new Color(0, 0, 0, 0); // 투명한 내부
            // Outline 컴포넌트로 테두리 효과 구현
            Outline outline = highlightFrame.GetComponent<Outline>();
            if (outline == null)
            {
                outline = highlightFrame.AddComponent<Outline>();
            }
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(2, 2);
            */
        }


        // [◆] - ▶▶▶ SetSpeechBubblePosition.
        private void SetSpeechBubblePosition(RectTransform targetRect, int step)
        {
            // 말풍선 위치를 대상 UI 요소 근처로 설정
            Vector3 bubblePosition = targetRect.position + (Vector3)speechBubblePositions[step];
            speechBubbleRect.position = bubblePosition;
            // 화면 경계를 벗어나지 않도록 조정
            Vector3[] corners = new Vector3[4];
            speechBubbleRect.GetWorldCorners(corners);
            // 화면 경계 체크 및 조정
            if (corners[2].x > Screen.width)
            {
                speechBubbleRect.position = new Vector3(Screen.width - speechBubbleRect.sizeDelta.x / 2, speechBubbleRect.position.y, speechBubbleRect.position.z);
            }
            if (corners[0].x < 0)
            {
                speechBubbleRect.position = new Vector3(speechBubbleRect.sizeDelta.x / 2, speechBubbleRect.position.y, speechBubbleRect.position.z);
            }
            if (corners[2].y > Screen.height)
            {
                speechBubbleRect.position = new Vector3(speechBubbleRect.position.x, Screen.height - speechBubbleRect.sizeDelta.y / 2, speechBubbleRect.position.z);
            }
            if (corners[0].y < 0)
            {
                speechBubbleRect.position = new Vector3(speechBubbleRect.position.x, speechBubbleRect.sizeDelta.y / 2, speechBubbleRect.position.z);
            }
        }


        // [◆] - ▶▶▶ NextStep.
        private void NextStep()
        {
            currentStep++;

            if (currentStep >= targetUIElements.Length)
            {
                EndTutorial();
            }
            else
            {
                ShowTutorialStep(currentStep);
            }
        }


        // [◆] - ▶▶▶ EndTutorial.
        private void EndTutorial()
        {
            isTutorialActive = false;
            // UI 요소들 비활성화
            // ) greenSlime.SetActive(false);
            // ) highlightFrame.SetActive(false);
            speechBubble.SetActive(false);
            // 배경 다시 밝게 만들기
            StartCoroutine(FadeBackground(0.8f, 0f, 0.5f));
            // 페이드 완료 후 튜토리얼 캔버스 비활성화 및 씬 전환
            StartCoroutine(DeactivateTutorialCanvasAndLoadScene(0.5f));
        }
        #endregion ▲▲▲▲▲ Custom Method ▲▲▲▲▲





        // [4] IEnumerator.
        #region ▼▼▼▼▼ IEnumerator ▼▼▼▼▼
        // [◆] - ▶▶▶ FadeBackground.
        private IEnumerator FadeBackground(float startAlpha, float endAlpha, float duration)
        {
            float elapsed = 0f;
            Color backgroundColor = darkBackground.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
                backgroundColor.a = alpha;
                darkBackground.color = backgroundColor;
                yield return null;
            }

            backgroundColor.a = endAlpha;
            darkBackground.color = backgroundColor;
        }


        // [◆] - ▶▶▶ DeactivateTutorialCanvasAndLoadScene (수정됨)
        private IEnumerator DeactivateTutorialCanvasAndLoadScene(float delay)
        {
            yield return new WaitForSeconds(delay);
            tutorialCanvas.SetActive(false);

            // 씬 전환
            SceneManager.LoadScene(TileMapPlayScene);
        }

        // 튜토리얼 건너뛰기 기능
        public void SkipTutorial()
        {
            EndTutorial();
        }
        #endregion ▲▲▲▲▲ IEnumerator ▲▲▲▲▲
    }
}