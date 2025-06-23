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
        //유닛 카드를 배치하기
        public void SetUnitCard(GameObject go) {
            
        }
        #endregion
    }

}
