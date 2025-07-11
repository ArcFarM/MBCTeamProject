using UnityEngine;
using MainGame.Enum;
using MainGame.Manager;
using MainGame.UI;

namespace MainGame.SystemProcess {
    public class GameOverEvent : MonoBehaviour {
        //게임 오버시 보여줄 팝업창
        #region Variables
        [SerializeField] GameObject[] gameoverMinPopups;
        [SerializeField] GameObject[] gameoverMaxPopups;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Method
        #endregion

        #region Custom Method
        public void OnStatMin(Stats stat) {
            //TODO : 준비된 오브젝트 활성화
            /*int idx = (int)stat;
            if (gameoverMinPopups[idx] != null) {
                gameoverMinPopups[idx].SetActive(true);
            }
            else {
                Debug.LogError("게임오버 최소치 팝업이 설정되지 않았습니다: " + stat);
            }*/
        }

        public void OnStatMax(Stats stat) {
            //TODO : 준비된 오브젝트 활성화
            /*int idx = (int)stat;
            if (gameoverMaxPopups[idx] != null) {
                gameoverMaxPopups[idx].SetActive(true);
            }
            else {
                Debug.LogError("게임오버 최대치 팝업이 설정되지 않았습니다: " + stat);
            }*/
        }

        public void DoGameOver(Stats stat) {
            SetStats target = StatManager.Instance.statArr[(int)stat];
            if (target.GetStat >= target.GetStatMax) OnStatMax(stat);
            else if (target.GetStat <= target.GetStatMin) OnStatMin(stat);
        }
        #endregion
    }
}

