using UnityEngine;
using MainGame.Enum;
using MainGame.Manager;
using MainGame.UI;

namespace MainGame.System {
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
        }

        public void OnStatMax(Stats stat) {
            //TODO : 준비된 오브젝트 활성화
        }

        public void DoGameOver(Stats stat) {
            SetStats target = StatManager.Instance.statArr[(int)stat];
            if (target.GetStat >= target.GetStatMax) OnStatMax(stat);
            else if (target.GetStat <= target.GetStatMin) OnStatMin(stat);
        }
        #endregion
    }
}

