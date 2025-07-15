using UnityEngine;
using TMPro;
using JiHoon;        // ) TextMeshProUGUI 활성화.

/* [0] 개요 : PlayerPoliticalStatUI
		- 메인 플레이 화면에서 10시방향에 있는 4가지 정책의 수치를 관리하는 클래스.
*/

namespace JeaYoon.Store
{
    public class PlayerPoliticalStatUI : MonoBehaviour
    {

        // [1] Variable.
        #region ▼▼▼▼▼ Variable ▼▼▼▼▼
        // [◆] - ▶▶▶ 정의.
        public TextMeshProUGUI unrestText;      // ) 불만 텍스트 수치.
        public TextMeshProUGUI financesText;        // ) 재정 텍스트 수치.
        public TextMeshProUGUI dominanceText;       // ) 지배 텍스트 수치.
        public TextMeshProUGUI ManpowerText;       // ) 혼돈 텍스트 수치.


        public ShopManager shopManager;
        #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲





        // [2] Unity Event Method.
        #region ▼▼▼▼▼ Unity Event Method ▼▼▼▼▼
        // [◆] - ▶▶▶ Update.
        private void Update()
        {
            //unrestText.text = shopManager.playerUnrest.ToString();         // ) 불만.
            //financesText.text = shopManager.playerGold.ToString();              // ) 재정.
            //dominanceText.text = shopManager.playerDominance.ToString();       // ) 지배.
            //ManpowerText.text = shopManager.playerManpower.ToString();                   // ) 혼돈.
        }
        #endregion ▲▲▲▲▲ Unity Event Method ▲▲▲▲▲
    }
}