using UnityEngine;

namespace MainGame.Units {
    public class EnemyUnitBase : UnitBase {
        #region Variables
        
        //적 유닛 통과 시 패널티
        [SerializeField, Tooltip("적 유닛 통과 시 입을 패널티")]
        protected int panelty;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Method
        #endregion

        #region Custom Method
        //TODO : 베이스에 도착했을 때 패널티 적용
        /*
             public class StatusEffect
    {
        public string effectName;
        public float duration;
        public float value;

        public StatusEffect(string name, float dur, float val)
        {
            effectName = name;
            duration = dur;
            value = val;
        }
    }
         */
        #endregion
    }

}
