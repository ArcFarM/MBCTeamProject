using UnityEngine;

namespace JeaYoon.Menu
{
    public class Quit : MonoBehaviour
    {
        public void QuitGame()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // WebGL에서는 종료 불가
                Debug.Log("WebGL에서는 게임을 종료할 수 없습니다.");
                return;
            }

            // 확인 창 표시 후 종료
            // 실제로는 UI 패널로 구현하는 것이 좋습니다
            Application.Quit();
        }
    }
}