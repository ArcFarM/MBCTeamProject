using JiHoon;
using UnityEngine;
using System.Collections.Generic;
using MainGame.Manager;

namespace MainGame.Units {
    public class EnemyUnitBase : UnitBase {
        #region Variables

        //적 유닛 통과 시 패널티
        [Header("적 유닛 통과시 입을 패널티")]
        public List<StatStruct> penalty;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Method
        protected override void Start() {
            base.Start();
            //MainGame.Manager.WaveManager.Instance.enemyCount++;
            //scriptable 참조해서 해당 ID로 된 penalty를 가져온다
        }
        #endregion

        #region Custom Method
        //TODO : 베이스에 도착했을 때 패널티 적용
        public void GivePenalty() {
            //StatManager에서 처리
            //1. 베이스에 도착한 적 유닛들의 정보를 statmanager에게 넘겨준다
            StatManager.Instance.AddPenalty(penalty);
            //2. Wave가 끝나면 StatManager에서 Rewardpenalty에 있는 Calcpenalty를 실행한다
            //3. 이걸 StatManager에서 반환값을 받아와서 적용한다
        }
        #endregion
    }

}
