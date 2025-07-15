using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace JiHoon
{
    public class PauseMenuManager : MonoBehaviour
    {
        [Header("메뉴 UI")]
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private Button menuToggleButton;

        [Header("메뉴 버튼들")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;

        [Header("게임 속도 조절")]
        [SerializeField] private Button speed10xButton;
        [SerializeField] private Button speed15xButton;
        [SerializeField] private Button speed20xButton;

        [Header("속도 버튼 색상 설정")]
        [SerializeField] private Color normalButtonColor = new Color(0.7f, 0.7f, 0.7f, 1f);  // 기본 색상 (회색)
        [SerializeField] private Color selectedButtonColor = Color.yellow;  // 선택된 색상 (노란색)
        [SerializeField] private float colorTransitionDuration = 0.2f;  // 색상 전환 시간

        [Header("설정 패널")]
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Button settingsBackButton;

        private bool isPaused = false;
        private float currentGameSpeed = 1.0f;
        private float previousTimeScale = 1.0f;

        // 속도 버튼들의 원본 색상 저장용
        private Color speed10xOriginalColor;
        private Color speed15xOriginalColor;
        private Color speed20xOriginalColor;

        void Start()
        {
            // 초기 설정
            pauseMenuPanel.SetActive(false);
            if (settingsPanel != null)
                settingsPanel.SetActive(false);

            // 버튼들의 원본 색상 저장 (있을 경우)
            SaveOriginalButtonColors();

            // 버튼 이벤트 연결
            SetupButtonListeners();

            // 초기 게임 속도 설정 (1.0배속)
            SetGameSpeed(1.0f);
        }

        void SaveOriginalButtonColors()
        {
            if (speed10xButton != null && speed10xButton.image != null)
                speed10xOriginalColor = speed10xButton.image.color;

            if (speed15xButton != null && speed15xButton.image != null)
                speed15xOriginalColor = speed15xButton.image.color;

            if (speed20xButton != null && speed20xButton.image != null)
                speed20xOriginalColor = speed20xButton.image.color;
        }

        void SetupButtonListeners()
        {
            // 메뉴 토글 버튼
            if (menuToggleButton != null)
            {
                menuToggleButton.onClick.AddListener(TogglePauseMenu);
            }

            // 메뉴 내부 버튼들
            resumeButton?.onClick.AddListener(Resume);
            settingsButton?.onClick.AddListener(OpenSettings);
            mainMenuButton?.onClick.AddListener(GoToMainMenu);
            quitButton?.onClick.AddListener(QuitGame);

            // 게임 속도 버튼들
            speed10xButton?.onClick.AddListener(() => SetGameSpeed(1.0f));
            speed15xButton?.onClick.AddListener(() => SetGameSpeed(1.5f));
            speed20xButton?.onClick.AddListener(() => SetGameSpeed(2.0f));

            // 설정 패널
            settingsBackButton?.onClick.AddListener(CloseSettings);
            bgmSlider?.onValueChanged.AddListener(SetBGMVolume);
            sfxSlider?.onValueChanged.AddListener(SetSFXVolume);
        }

        public void TogglePauseMenu()
        {
            isPaused = !isPaused;
            pauseMenuPanel.SetActive(isPaused);

            if (isPaused)
            {
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = previousTimeScale;
                if (settingsPanel != null && settingsPanel.activeSelf)
                {
                    settingsPanel.SetActive(false);
                }
            }
        }

        void Resume()
        {
            isPaused = false;
            pauseMenuPanel.SetActive(false);
            Time.timeScale = currentGameSpeed;
        }

        void SetGameSpeed(float speed)
        {
            currentGameSpeed = speed;
            if (!isPaused)
            {
                Time.timeScale = speed;
            }
            previousTimeScale = speed;

            // 버튼 색상 업데이트
            UpdateSpeedButtonColors(speed);

            Debug.Log($"게임 속도 변경: {speed}x");
        }

        void UpdateSpeedButtonColors(float speed)
        {
            // 모든 버튼을 기본 색상으로 설정
            if (speed10xButton != null && speed10xButton.image != null)
            {
                speed10xButton.image.color = normalButtonColor;
                // 텍스트 색상도 변경하고 싶다면
                var text = speed10xButton.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null) text.color = Color.white;
            }

            if (speed15xButton != null && speed15xButton.image != null)
            {
                speed15xButton.image.color = normalButtonColor;
                var text = speed15xButton.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null) text.color = Color.white;
            }

            if (speed20xButton != null && speed20xButton.image != null)
            {
                speed20xButton.image.color = normalButtonColor;
                var text = speed20xButton.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null) text.color = Color.white;
            }

            // 선택된 버튼만 하이라이트 색상으로 변경
            Button selectedButton = null;
            if (speed == 1.0f) selectedButton = speed10xButton;
            else if (speed == 1.5f) selectedButton = speed15xButton;
            else if (speed == 2.0f) selectedButton = speed20xButton;

            if (selectedButton != null && selectedButton.image != null)
            {
                selectedButton.image.color = selectedButtonColor;

                // 텍스트 색상도 변경 (선택적)
                var text = selectedButton.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null) text.color = Color.black;  // 노란색 배경에 검은색 텍스트

                // 애니메이션 효과 추가 (선택적)
                StartCoroutine(PulseEffect(selectedButton.transform));
            }
        }

        // 선택된 버튼에 펄스 효과 추가 (선택적)
        System.Collections.IEnumerator PulseEffect(Transform buttonTransform)
        {
            Vector3 originalScale = buttonTransform.localScale;
            Vector3 targetScale = originalScale * 1.1f;

            // 커지기
            float elapsed = 0f;
            while (elapsed < 0.1f)
            {
                elapsed += Time.unscaledDeltaTime;
                buttonTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / 0.1f);
                yield return null;
            }

            // 원래 크기로 돌아오기
            elapsed = 0f;
            while (elapsed < 0.1f)
            {
                elapsed += Time.unscaledDeltaTime;
                buttonTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / 0.1f);
                yield return null;
            }

            buttonTransform.localScale = originalScale;
        }

        void OpenSettings()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
                if (bgmSlider != null)
                    bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
                if (sfxSlider != null)
                    sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
            }
        }

        void CloseSettings()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }

        void SetBGMVolume(float value)
        {
            PlayerPrefs.SetFloat("BGMVolume", value);
            Debug.Log($"BGM 볼륨: {value}");
        }

        void SetSFXVolume(float value)
        {
            PlayerPrefs.SetFloat("SFXVolume", value);
            Debug.Log($"SFX 볼륨: {value}");
        }

        void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        void QuitGame()
        {
            Debug.Log("게임 종료");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && !isPaused)
            {
                TogglePauseMenu();
            }
        }
    }
}