using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChageScene : MonoBehaviour
{
    private string mainMenuScene = "MainMenu";
    public Button continueButton;

    private void Start()
    {
        // �̾��ϱ� ��ư ���� ������Ʈ
        UpdateContinueButton();
    }

    // �� ���� ����
    public void GameStartButton()
    {
        // ���� ���� ������ �ʱ�ȭ (���û���)
        PlayerPrefs.DeleteAll();

        SceneManager.LoadScene(mainMenuScene);
    }

    // �̾��ϱ� ��ư
    // ó���� ���ӽ�ŸƮ��ư������ �� �ʱ�ȭ�ǰ� ���θ޴����� ������� ���θ޴������� �̾��ϱ� ��ư�� ������ ����� ������ �̵�
    // �ٸ����� AutoSaveScene��ũ��Ʈ�� �پ��ִ� �������Ʈ�� �ִ� ���� �����
    public void ContinueGameButton()
    {
        if (HasSaveData())
        {
            // ����� �� �ε� (����� ������ �ִٸ�)
            string savedScene = PlayerPrefs.GetString("SavedScene", mainMenuScene);
            SceneManager.LoadScene(savedScene);
        }
        else
        {
            Debug.LogWarning("����� ������ �����ϴ�!");
        }
    }


    public void OptionButton()
    {
        Debug.Log("�ɼ� �� �̵�");
    }

    public void QuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ���� �����Ͱ� �ִ��� Ȯ��
    private bool HasSaveData()
    {
        // PlayerPrefs�� ����� ������ ���� �ý���
        return PlayerPrefs.HasKey("GameSaved");
    }

    // �̾��ϱ� ��ư ���� ������Ʈ
    private void UpdateContinueButton()
    {
        if (continueButton != null)
        {
            bool hasSave = HasSaveData();
            continueButton.interactable = hasSave;
            
        }
    }
}