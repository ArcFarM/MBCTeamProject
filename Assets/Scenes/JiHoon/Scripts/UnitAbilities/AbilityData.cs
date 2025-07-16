using UnityEngine;
namespace JiHoon
{
    public enum AbilityTriggerType
    {
        Passive,        // 패시브 (항상 적용)
        OnAttack,       // 공격할 때
        OnHit,          // 공격이 명중했을 때
        OnDamaged,      // 피해를 받았을 때
        OnDeath,        // 죽을 때
        OnKill,         // 적을 처치했을 때
        OnSpawn,        // 생성될 때
        Periodic        // 주기적으로 발동
    }

    // 특수능력 베이스 클래스
    public abstract class AbilityData : ScriptableObject
    {
        [Header("기본 정보")]
        public string abilityName;
        [TextArea(3, 5)]
        public string description;
        public Sprite abilityIcon;

        [Header("발동 조건")]
        public AbilityTriggerType triggerType;

        [Header("주기적 발동 설정")]
        public float interval = 0f;  // Periodic 타입일 때만 사용

        // 능력 실행
        public abstract void Execute(GameObject caster, GameObject target = null);

        // 능력 초기화 (필요한 경우)
        public virtual void Initialize(GameObject caster) { }
    }
}