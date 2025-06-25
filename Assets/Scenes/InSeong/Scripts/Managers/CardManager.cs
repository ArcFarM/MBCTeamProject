using MainGame.Enum;
using MainGame.UI;
using MainGame.Units;
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
                    //UnitManager를 통해 손패에 유닛 변경 사항 반영
                    foreach(GameObject go in pc.GetSetUnitsArr) {
                        AllyUnitBase aub = go.GetComponent<AllyUnitBase>();
                        //대상이 아군 유닛이 맞다면 추가
                        if(aub != null) {
                            UnitManager.Instance.SetUnitCard(pc.GetAddFlag, go);
                        }
                    }
                    break;
                case CardEffect.Change_Both:

                    break;
            }
        }
        #endregion
    }
}

