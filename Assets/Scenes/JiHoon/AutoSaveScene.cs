using UnityEngine;
using UnityEngine.SceneManagement;

//씬마다 빈오브젝트에 올려놓으면 됌
public class AutoSceneSaver : MonoBehaviour
{
    private void Start()
    {
        // 현재 씬이 메인메뉴나 설정씬이 아닌 경우에만 저장
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene != "MainMenu" && currentScene != "SettingsScene")
        {
            SaveCurrentScene();
        }
    }

    private void SaveCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("LastScene", currentScene);
        PlayerPrefs.SetInt("GameSaved", 1);
        PlayerPrefs.Save();

        Debug.Log($"현재 씬 저장됨: {currentScene}");
    }

    // 애플리케이션 종료시에도 저장
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveCurrentScene();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveCurrentScene();
        }
    }
}