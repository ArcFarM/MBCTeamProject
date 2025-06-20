using MainGame.Enum;
using MainGame.UI;
using UnityEngine;

namespace MainGame.Manager {
    public class CardManager : SingletonManager<CardManager> {
        #region Variables
        #endregion

        #region Properties
        #endregion

        #region Unity Event Method
        #endregion

        #region Custom Method
        //TODO : PolicyCard에서 카드 클릭하면 카드 효과 적용하기
        public void ApplyEffect(PolicyCard pc) {
            CardEffect ce = pc.GetSetCardEffect;
            switch (ce) {
                case CardEffect.Change_Stat:
                    //StatManager를 통해 능력치 조정
                    StatManager.Instance.AdjustStat(pc);
                    break;
                case CardEffect.Change_Unit:

                    break;
                case CardEffect.Change_Both:

                    break;
            }
        }
        #endregion
    }
}

