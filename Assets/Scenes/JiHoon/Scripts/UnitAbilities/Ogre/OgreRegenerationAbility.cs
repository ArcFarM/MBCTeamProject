using UnityEngine;
using MainGame.Units;
using MainGame.Units.Battle;
using MainGame.Enum;

namespace JiHoon
{
    [CreateAssetMenu(fileName = "Ability_OgreRegeneration", menuName = "Game/Abilities/OgreRegeneration")]
    public class OgreRegenerationAbility : AbilityData
    {
        [Header("재생 설정")]
        [SerializeField] private float healAmount = 20f;    // 회복량

        public override void Execute(GameObject caster, GameObject target = null)
        {
            if (caster == null) return;

            // BattleBase 컴포넌트 가져오기
            var battleBase = caster.GetComponent<BattleBase>();
            if (battleBase == null)
            {
                Debug.LogWarning($"[{caster.name}] BattleBase 컴포넌트를 찾을 수 없습니다.");
                return;
            }

            // UnitBase 컴포넌트 가져오기 (체력 확인용)
            var unitBase = caster.GetComponent<MainGame.Units.UnitBase>();
            if (unitBase == null || unitBase.IsDead) return;

            // 현재 체력과 최대 체력 확인
            float currentHealth = unitBase.GetStat(StatType.CurrHealth);
            float maxHealth = unitBase.GetStat(StatType.BaseHealth);

            // 이미 최대 체력이면 회복하지 않음
            if (currentHealth >= maxHealth)
            {
                Debug.Log($"[{caster.name}] 이미 최대 체력입니다. (현재: {currentHealth}/{maxHealth})");
                return;
            }

            // 체력 회복
            battleBase.HealHealth(healAmount);

            // 회복 후 체력 확인
            float newHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
            Debug.Log($"[{caster.name}] {abilityName} 발동: {healAmount} 회복! ({currentHealth} → {newHealth}/{maxHealth})");
        }

        public override void Initialize(GameObject caster)
        {
            // 초기화가 필요한 경우 여기에 작성
            Debug.Log($"[{caster.name}] {abilityName} 능력 초기화 완료");
        }
    }
}