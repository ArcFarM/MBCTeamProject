using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/* [0] 개요 : HighlightFrame
		- 
*/

namespace JeaYoon.Tutorial
{
    public class HighlightFrame : MonoBehaviour
    {
        // [1] Variable.
        #region ▼▼▼▼▼ Variable ▼▼▼▼▼
        // [◆] - ▶▶▶ Header → 하이라이트 설정.
        [Header("하이라이트 설정")]
        public Image frameImage;
        public float animationDuration = 1f;
        public float minAlpha = 0.3f;
        public float maxAlpha = 0.8f;


        // [◆] - ▶▶▶ .
        private Coroutine animationCoroutine;
        #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲






        // [2] Unity Event Method.
        #region ▼▼▼▼▼ Unity Event Method ▼▼▼▼▼
        // [◆] - ▶▶▶ Start.
        private void Start()
        {
            SetupFrame();
        }
        #endregion ▲▲▲▲▲ Unity Event Method ▲▲▲▲▲




        // [3] Custom Method.
        #region ▼▼▼▼▼ Custom Method ▼▼▼▼▼
        // [◆] - ▶▶▶ SetupFrame.
        private void SetupFrame()
        {
            // 프레임 기본 설정 - 초기에는 보이지 않게
            //frameImage.color = new Color(frameImage.color.r, frameImage.color.g, frameImage.color.b, 0f);

            // 점선 테두리 효과를 위한 설정
            CreateDashedBorder();
        }


        // [◆] - ▶▶▶ CreateDashedBorder.
        private void CreateDashedBorder()
        {
            // 점선 테두리 효과 구현
            // Unity UI에서는 직접적인 점선 지원이 없으므로 여러 개의 작은 이미지로 구현
            // 또는 Outline 컴포넌트 사용
            Outline outline = frameImage.gameObject.GetComponent<Outline>();
            if (outline == null)
            {
                outline = frameImage.gameObject.AddComponent<Outline>();
            }
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(2, 2);
            // 깜빡이는 효과 시작
            StartBlinkAnimation();
        }


        // [◆] - ▶▶▶ StartBlinkAnimation.
        private void StartBlinkAnimation()
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }
            animationCoroutine = StartCoroutine(BlinkAnimation());
        }
        #endregion ▲▲▲▲▲ Custom Method ▲▲▲▲▲





        // [5] IEnumerator.
        #region ▼▼▼▼▼ IEnumerator ▼▼▼▼▼
        // [◆] - ▶▶▶ BlinkAnimation.
        private IEnumerator BlinkAnimation()
        {
            Outline outline = frameImage.gameObject.GetComponent<Outline>();
            Color originalColor = frameImage.color;

            while (gameObject.activeInHierarchy)
            {
                // 페이드 인
                float elapsed = 0f;
                while (elapsed < animationDuration / 2)
                {
                    elapsed += Time.deltaTime;
                    float alpha = Mathf.Lerp(minAlpha, maxAlpha, elapsed / (animationDuration / 2));

                    // frameImage의 알파값 변경
                    frameImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

                    // outline의 알파값 변경
                    if (outline != null)
                    {
                        Color outlineColor = outline.effectColor;
                        outlineColor.a = alpha;
                        outline.effectColor = outlineColor;
                    }
                    yield return null;
                }

                // 페이드 아웃
                elapsed = 0f;
                while (elapsed < animationDuration / 2)
                {
                    elapsed += Time.deltaTime;
                    float alpha = Mathf.Lerp(maxAlpha, minAlpha, elapsed / (animationDuration / 2));

                    // frameImage의 알파값 변경
                    frameImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

                    // outline의 알파값 변경
                    if (outline != null)
                    {
                        Color outlineColor = outline.effectColor;
                        outlineColor.a = alpha;
                        outline.effectColor = outlineColor;
                    }
                    yield return null;
                }
            }
        }
        #endregion ▲▲▲▲▲ IEnumerator ▲▲▲▲▲





        // [6] ETC.
        #region ▼▼▼▼▼ ETC ▼▼▼▼▼
        private void OnDisable()
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }
        }

        public void SetRoundedCorners(float cornerRadius)
        {
            // 둥근 모서리 설정 (Sprite가 있다면)
            // frameImage.sprite = CreateRoundedRectSprite(cornerRadius);
        }
        #endregion ▲▲▲▲▲ ETC ▲▲▲▲▲
    }
}