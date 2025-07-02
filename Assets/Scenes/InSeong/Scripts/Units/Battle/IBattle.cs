using UnityEngine;

namespace MainGame.Units.Battle {
    public interface IBattle{

        #region Properties
        #endregion

        #region Unity Event Methods
        #endregion

        #region Custom Methods
        //피해를 받거나 체력을 회복
        public void TakeDamage(float damage);
        public void HealHealth(float healAmount);

        //사망 처리
        public void Die();

        //사거리 내에 공격 대상이 있는 지 확인 - 매개변수 : 공격 대상
        public bool IsInRange(UnitBase target);

        //적을 공격 - 매개변수 : 공격 대상
        public void Attack(GameObject target);
        #endregion
    }

}
