using UnityEngine;
using UnityEngine.UI;       // ) Image 활성화.
using System.Collections;       // ) IEnumerator 활성화.
using UnityEngine.SceneManagement;      // ) SceneManager 활성화.

namespace JeaYoon
{
    //씬 시작시 페이드인, 씬 종료시 페이드 아웃 효과 구현
    public class SceneFader : MonoBehaviour
    {
        // [1] Variable.
        #region ▼▼▼▼▼ Variable ▼▼▼▼▼
        // [◆] - ▶▶▶ 123.
        public Image img;                      // ) 이미지 활성화 → 검은색 화면.
        public AnimationCurve curve;        // ) 애니메이션에서 Curve를 활성화 → 검은색 화면이 자연스럽게 페이드 아웃하게 함.
        #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲





        // [2] Unity Event Method.
        #region ▼▼▼▼▼ Unity Event Method ▼▼▼▼▼
        // [◆] - ▶▶▶ Start.
        private void Start()
        {
            // [◇] - [◆] - ) Coroutine을 시작함 → FadeIn.
            StartCoroutine(FadeIn());       // ) StartCoroutine()은 프레임을 기다리거나, 지연하거나, 순차 처리를 할 때 사용하는 함수.
        }


        // [◆] - ▶▶▶ FadeTo → 다른 씬으로 이동할 때 호출됨.
        public void FadeTo(string sceneName = "")
        {
            StartCoroutine(FadeOut(sceneName));
        }
        #endregion ▲▲▲▲▲ Unity Event Method ▲▲▲▲▲





        // [4] Custom Method.
        #region ▼▼▼▼▼ Custom Method ▼▼▼▼▼
        // [◆] - ▶▶▶ FadeIn → 1초동안 검정에서 투명하게 바뀜.
        IEnumerator FadeIn()        // ) 코루틴(Coroutine)을 만들기 위해 반드시 사용하는 반환 타입.
        {
            // [◇] - [◆] - ) 페이드 인 효과의 "시간 진행도"를 나타내는 변수.
            float t = 1f;
            // [◇] - [◆] - ) t가 1보다 클 경우.
            while (t > 0)
            {
                t -= Time.deltaTime;                         // ) 점점 투명하게 만듬.
                float a = curve.Evaluate(t);                  // ) t에 따라 애니메이션 커브에서 투명도 계산.
                img.color = new Color(0f, 0f, 0f, a);       // ) 알파값 a를 이용해 페이드 인/아웃 효과를 만드는 것.
                yield return null;                                // ) IEnumerator는 yield return 문을 통해 "중간에 잠깐 멈췄다가 다시 실행되는" 함수.
            }
        }


        // [◆] - ▶▶▶ FadeOut.
        IEnumerator FadeOut(string sceneName)
        {
            // [◇] - [◆] - ) 페이드 인 효과의 "시간 진행도"를 나타내는 변수.
            float t = 0f;
            // [◇] - [◆] - ) t가 1보다 작을 경우.
            while (t < 0)
            {
                t -= Time.deltaTime;                         // ) 점점 투명하게 만듬.
                float a = curve.Evaluate(t);                  // ) t에 따라 애니메이션 커브에서 투명도 계산.
                img.color = new Color(0f, 0f, 0f, a);       // ) 알파값 a를 이용해 페이드 인/아웃 효과를 만드는 것.
                yield return null;                                // ) IEnumerator는 yield return 문을 통해 "중간에 잠깐 멈췄다가 다시 실행되는" 함수.
            }
            // [◇] - [◆] - ) sceneName이 빈 문자열이 아닐 때 → 씬 이름이 실제로 전달됐을 때만 씬을 전환하기.
            if (sceneName != "")
            {
                SceneManager.LoadScene(sceneName);      // ) 현재 게임 화면을 지정한 씬으로 바꾸기.
            }
        }
        #endregion ▲▲▲▲▲ Custom Method ▲▲▲▲▲
    }
}