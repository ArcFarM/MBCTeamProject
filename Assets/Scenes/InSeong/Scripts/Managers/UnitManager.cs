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
        //구매한 유닛 카드를 손패에 넣거나 사용한 카드를 빼기
        public void SetUnitCard(bool isAdd, GameObject go) {
            if (isAdd) cl.AddCard(go);
            else cl.RemoveCard(go);
        }

        public void DeployUnit(GameObject go) {

        }
        #endregion
    }
}
