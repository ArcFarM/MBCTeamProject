using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPanel;   //팝업 패널

    public Button gameStartBtn; //게임 시작 버튼
    public Button yesBtn; //예 버튼
    public Button noBtn; //아니오 버튼

    private void Start()
    {
        //처음엔 팝업 숨기기
        popupPanel.SetActive(false);
    }
    


    public void OnClickYes()
    {
        //기존 저장데이터 삭제 후 새 게임 시작
        DeleteSaveData();
        StartNewGame();
    }
    public void OnClickNo()
    {
        //팝업 닫기
        popupPanel.SetActive(false);
    }

    

    
    private void DeleteSaveData()
    {
        //playerPrefs에 저장된 데이터 삭제(필요한 경우만 삭제)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
    private void StartNewGame()
    {
        //예 버튼 : 첫 씬으로 이동
        SceneManager.LoadScene("Mainmenu");
    }
}


