using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChageScene : MonoBehaviour
{
    private string mainMenuScene = "New Scene";
    public Button continueButton;


    public GameObject confirmPanel; // 확인창 패널//
    

    private void Start()
    {
        // 이어하기 버튼 상태 업데이트
        UpdateContinueButton();
    }

    // 새 게임 시작
    public void GameStartButton()
    {
        if(HasSaveData())
        {
            // 저장된 데이터가 있다면 확인창 띄우기
            confirmPanel.SetActive(true);
            
        }
        else
        {
            SceneManager.LoadScene(mainMenuScene);
        }
    }

    // 이어하기 버튼
    // 처음에 게임스타트버튼누르면 다 초기화되고 메인메뉴씬은 저장안함 메인메뉴씬에서 이어하기 버튼을 누르면 저장된 씬으로 이동
    // 다른씬에 AutoSaveScene스크립트가 붙어있는 빈오브젝트가 있는 씬만 저장됨
    public void ContinueGameButton()
    {
        if (HasSaveData())
        {
            // 저장된 씬 로드 (저장된 레벨이 있다면)
            string savedScene = PlayerPrefs.GetString("SavedScene", mainMenuScene);
            SceneManager.LoadScene(savedScene);
        }
        else
        {
            Debug.LogWarning("저장된 게임이 없습니다!");
        }
    }


    public void OptionButton()
    {
        Debug.Log("옵션 씬 이동");
    }

    public void QuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // 저장 데이터가 있는지 확인
    private bool HasSaveData()
    {
        // PlayerPrefs를 사용한 간단한 저장 시스템
        return PlayerPrefs.HasKey("GameSaved");
    }

    // 이어하기 버튼 상태 업데이트
    private void UpdateContinueButton()
    {
        if (continueButton != null)
        {
            bool hasSave = HasSaveData();
            continueButton.interactable = hasSave;
            
        }
    }
}