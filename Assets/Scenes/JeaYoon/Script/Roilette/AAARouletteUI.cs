using UnityEngine;
using UnityEngine.SceneManagement;


namespace JeaYoon.Roulette
{
    public class AAARouletteUI : MonoBehaviour
    {
        // [1] Variable.
        #region ▼▼▼▼▼ Variable ▼▼▼▼▼
        // [◆] - ▶▶▶ Scene.
        public SceneFader fader;                                                 // ) 스크립트 중 "SceneFader"를 추가.
        [SerializeField] private string loadToScene = "MainPlayUI";        // ) Scene 중 "MainPlayUI"로 연결.


        // [◆] - ▶▶▶ UI 오브젝트.
        public GameObject rouletteUI;                                            // ) UI 중 "RouletteUI"로 연결.
        #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲





        // [2] Custom Method.
        #region ▼▼▼▼▼ Custom Method ▼▼▼▼▼
        // [◆] - ▶▶▶ Toggle.
        public void Toggle()
        {
            // [◇] - [◆] - ) Store UI 창 켜고 끄기.
            rouletteUI.SetActive(!rouletteUI.activeSelf);
            // [◇] - [◆] - ) Store UI 창이 열리면 Pause ON.
            if (rouletteUI.activeSelf)
            {
                // ) .Time.timeScale = 0f; → 활성화 할 경우 룰렛이 돌아가지 않음.
            }
            // [◇] - [◆] - ) Store UI 창이 열리면 Pause OFF.
            else
            {
                // ) Time.timeScale = 1f; → 활성화 할 경우 룰렛이 돌아가지 않음.
            }
        }


        // [◆] - ▶▶▶ Retry.
        public void Retry()
        {
            // [◇] - [◆] - ) 게임이 정지상태였다가 다시 시간을 흐르게 함.
            Time.timeScale = 1f;
            // [◇] - [◆] - ) 페이드효과 + 현재 활성화된 씬을 이름으로 다시 불러옴.
            fader.FadeTo(SceneManager.GetActiveScene().name);
        }


        // [◆] - ▶▶▶ MainPlay.
        public void MainPlay()
        {
            // [◇] - [◆] - ) 게임이 정지상태였다가 다시 시간을 흐르게 함.
            Time.timeScale = 1f;
            // [◇] - [◆] - ) 페이드효과 + 지정된 Scene로 전환함.
            fader.FadeTo(loadToScene);
        }
        #endregion ▲▲▲▲▲ Custom Method ▲▲▲▲▲
    }
}