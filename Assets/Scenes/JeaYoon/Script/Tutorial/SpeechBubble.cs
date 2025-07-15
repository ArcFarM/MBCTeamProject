using UnityEngine;
using UnityEngine.UI;

/* [0] 개요 : SpeechBubble
		- 
*/

namespace JeaYoon.Tutorial
{
    public class SpeechBubble : MonoBehaviour
    {
        // [1] Variable.
        #region ▼▼▼▼▼ Variable ▼▼▼▼▼
        // [◆] - ▶▶▶ Header → 말풍선 설정.
        [Header("말풍선 설정")]
        public RectTransform bubbleRect;
        public RectTransform arrowRect;
        public Image bubbleImage;
        public Image arrowImage;


        // [◆] - ▶▶▶ Header → 화살표 설정.
        [Header("화살표 설정")]
        public float arrowSize = 20f;
        public Vector2 arrowOffset = new Vector2(0, -10);
        #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲





        // [2] Unity Event Method.
        #region ▼▼▼▼▼ Unity Event Method ▼▼▼▼▼
        // [◆] - ▶▶▶ Start.
        private void Start()
        {
            SetupBubbleStyle();
        }
        #endregion ▲▲▲▲▲ Unity Event Method ▲▲▲▲▲





        // [3] Custom Method.
        #region ▼▼▼▼▼ Custom Method ▼▼▼▼▼
        // [◆] - ▶▶▶ SetupBubbleStyle.
        private void SetupBubbleStyle()
        {
            // 말풍선 스타일 설정
            bubbleImage.color = new Color(1f, 1f, 0.8f, 0.95f); // 연한 노란색 배경
            // 둥근 모서리 효과를 위한 이미지 설정 (스프라이트가 있다면)
            // bubbleImage.sprite = roundedRectSprite;
            // 그림자 효과 추가
            Shadow shadow = bubbleImage.gameObject.GetComponent<Shadow>();
            if (shadow == null)
            {
                shadow = bubbleImage.gameObject.AddComponent<Shadow>();
            }
            shadow.effectColor = new Color(0, 0, 0, 0.3f);
            shadow.effectDistance = new Vector2(2, -2);
            // 테두리 효과 추가
            Outline outline = bubbleImage.gameObject.GetComponent<Outline>();
            if (outline == null)
            {
                outline = bubbleImage.gameObject.AddComponent<Outline>();
            }
            outline.effectColor = new Color(0.7f, 0.7f, 0.7f, 1f);
            outline.effectDistance = new Vector2(1, 1);
        }


        // [◆] - ▶▶▶ SetArrowDirection.
        public void SetArrowDirection(Vector2 targetPosition)
        {
            // 말풍선 위치와 대상 위치를 비교하여 화살표 방향 결정
            Vector2 bubblePosition = bubbleRect.position;
            Vector2 direction = (targetPosition - bubblePosition).normalized;
            // 화살표 위치 설정
            arrowRect.anchoredPosition = direction * arrowOffset;
            // 화살표 회전 설정
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrowRect.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        #endregion ▲▲▲▲▲ Custom Method ▲▲▲▲▲




    }
}