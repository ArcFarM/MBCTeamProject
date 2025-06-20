using UnityEngine;
using MainGame.UI;
using MainGame.Enum;
//능력치를 현재 수치와 동기화하여 시각 효과
namespace MainGame.Manager {
    public class StatManager : SingletonManager<StatManager> {
        #region Variables
        //정치 스탯들
        public SetStats[] statArr;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Method
        #endregion

        #region Custom Method
        //정책카드를 클릭하여 능력치 변화가 생겼을 때 그만큼 수치를 조정
        public void AdjustStat(PolicyCard pc) {
            int[] statDelta = pc.GetSetStatsArr;
            for(int i = 0; i < statArr.Length; i++) {
                if(statDelta[i] != 0) {
                    statArr[i].OnValueChange(statDelta[i]);
                }
            }
        }
        //TODO : 유닛을 구매하여 능력치 변화가 생겼을 때 그만큼 수치를 조정
        
        #endregion
    }
}

