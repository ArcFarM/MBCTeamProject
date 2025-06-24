using UnityEngine;
using System.Collections.Generic;
using MainGame.UI;

namespace MainGame.Manager {
    public class UnitManager : SingletonManager<UnitManager> {
        #region Variables
        //카드가 배치될 리스트
        [SerializeField] CardList cl;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Method
        #endregion

        #region Custom Method
        //구매한 유닛 카드를 손패에 넣는다
        public void SetUnitCard(GameObject go) {
            cl.AddCard(go);
        }
        #endregion
    }

}
