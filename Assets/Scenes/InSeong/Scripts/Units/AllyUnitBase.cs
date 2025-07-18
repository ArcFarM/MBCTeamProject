using UnityEngine;
using JiHoon;
using System.Collections;
using System.Collections.Generic;
using MainGame.Units.Battle;

namespace MainGame.Units
{
    public class AllyUnitBase : UnitBase
    {
        #region Variables
        [SerializeField, Tooltip("아군 유닛의 가격")]
        protected int[] prices = { 0, 0, 0, 0 };

        [Header("유닛 카드 데이터 참조")]
        [SerializeField] private UnitCardData unitCardData; // 이 유닛의 카드 데이터

        [Header("특수능력 (자동 로드됨)")]
        [SerializeField] private List<AbilityData> specialAbilities = new List<AbilityData>();

        // 주기적 능력의 코루틴 관리
        private Dictionary<AbilityData, Coroutine> periodicAbilityCoroutines = new Dictionary<AbilityData, Coroutine>();

        // 전투 관련 컴포넌트 참조
        private BattleBase battleBase;
        #endregion

        #region Unity Event Method
        protected override void Awake()
        {
            base.Awake();
            battleBase = GetComponent<BattleBase>();

            // UnitCardData에서 특수능력 자동 로드
            LoadAbilitiesFromCardData();
        }

        protected override void Start()
        {
            base.Start();

            // 특수능력 초기화
            InitializeAbilities();

            // 생성 시 발동 능력 처리
            TriggerAbilities(AbilityTriggerType.OnSpawn);
        }

        private void OnDestroy()
        {
            // 모든 주기적 능력 코루틴 중지
            StopAllPeriodicAbilities();

            // 부활 기록 정리
            ReviveAbility.CleanupReviveRecord(gameObject);
        }
        #endregion

        #region 특수능력 시스템
        // UnitCardData에서 특수능력 로드
        private void LoadAbilitiesFromCardData()
        {
            // UnitCardData가 할당되어 있으면 그곳에서 특수능력 가져오기
            if (unitCardData != null && unitCardData.specialAbilities != null)
            {
                specialAbilities.Clear(); // 기존 리스트 초기화
                specialAbilities.AddRange(unitCardData.specialAbilities);

                Debug.Log($"[{gameObject.name}] UnitCardData에서 {specialAbilities.Count}개의 특수능력 로드됨");
                foreach (var ability in specialAbilities)
                {
                    if (ability != null)
                    {
                        Debug.Log($"  - {ability.abilityName}");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] UnitCardData가 없거나 특수능력이 없습니다.");
            }
        }

        // 특수능력 초기화
        private void InitializeAbilities()
        {
            foreach (var ability in specialAbilities)
            {
                if (ability == null) continue;

                // 능력 초기화
                ability.Initialize(gameObject);

                // 패시브 능력 즉시 적용
                if (ability.triggerType == AbilityTriggerType.Passive)
                {
                    ability.Execute(gameObject);
                    Debug.Log($"[{gameObject.name}] 패시브 능력 적용: {ability.abilityName}");
                }

                // 주기적 능력 코루틴 시작
                else if (ability.triggerType == AbilityTriggerType.Periodic && ability.interval > 0)
                {
                    var coroutine = StartCoroutine(PeriodicAbilityCoroutine(ability));
                    periodicAbilityCoroutines[ability] = coroutine;
                    Debug.Log($"[{gameObject.name}] 주기적 능력 시작: {ability.abilityName} (간격: {ability.interval}초)");
                }
            }

            if (specialAbilities.Count > 0)
            {
                Debug.Log($"[{gameObject.name}] 특수능력 {specialAbilities.Count}개 초기화 완료");
            }
        }

        // 주기적 능력 실행 코루틴
        private IEnumerator PeriodicAbilityCoroutine(AbilityData ability)
        {
            // 첫 발동까지 대기
            yield return new WaitForSeconds(ability.interval);

            while (!IsDead)
            {
                ability.Execute(gameObject);
                Debug.Log($"[{gameObject.name}] {ability.abilityName} 발동!");

                yield return new WaitForSeconds(ability.interval);
            }
        }

        // 특정 트리거의 능력들 실행
        public void TriggerAbilities(AbilityTriggerType triggerType, GameObject target = null)
        {
            foreach (var ability in specialAbilities)
            {
                if (ability != null && ability.triggerType == triggerType)
                {
                    ability.Execute(gameObject, target);
                    Debug.Log($"[{gameObject.name}] {triggerType} 트리거로 {ability.abilityName} 발동!");
                }
            }
        }

        // 모든 주기적 능력 중지
        private void StopAllPeriodicAbilities()
        {
            foreach (var coroutine in periodicAbilityCoroutines.Values)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }
            periodicAbilityCoroutines.Clear();
        }
        #endregion

        #region 전투 이벤트 연동
        // BattleBase에서 호출할 메서드들
        public void OnAttackStart(GameObject target)
        {
            TriggerAbilities(AbilityTriggerType.OnAttack, target);
        }

        public void OnAttackHit(GameObject target)
        {
            TriggerAbilities(AbilityTriggerType.OnHit, target);
        }

        public void OnKillEnemy(GameObject enemy)
        {
            TriggerAbilities(AbilityTriggerType.OnKill, enemy);
        }
        #endregion

        #region 특수능력 관리 메서드
        // 외부에서 UnitCardData 설정 (유닛 생성 시 사용)
        public void SetUnitCardData(UnitCardData cardData)
        {
            unitCardData = cardData;
            LoadAbilitiesFromCardData();

            // 이미 Start()가 호출된 경우 능력 초기화
            if (gameObject.activeInHierarchy)
            {
                InitializeAbilities();
                TriggerAbilities(AbilityTriggerType.OnSpawn);
            }
        }

        // 런타임에 개별 특수능력 추가
        public void AddSpecialAbility(AbilityData ability)
        {
            if (ability != null && !specialAbilities.Contains(ability))
            {
                specialAbilities.Add(ability);

                // 추가된 능력 초기화
                ability.Initialize(gameObject);

                // 패시브면 즉시 적용
                if (ability.triggerType == AbilityTriggerType.Passive)
                {
                    ability.Execute(gameObject);
                }
                // 주기적이면 코루틴 시작
                else if (ability.triggerType == AbilityTriggerType.Periodic && ability.interval > 0)
                {
                    var coroutine = StartCoroutine(PeriodicAbilityCoroutine(ability));
                    periodicAbilityCoroutines[ability] = coroutine;
                }

                Debug.Log($"[{gameObject.name}] 특수능력 추가: {ability.abilityName}");
            }
        }

        // 특수능력 제거
        public void RemoveSpecialAbility(AbilityData ability)
        {
            if (specialAbilities.Contains(ability))
            {
                specialAbilities.Remove(ability);

                // 주기적 능력이면 코루틴 중지
                if (periodicAbilityCoroutines.ContainsKey(ability))
                {
                    StopCoroutine(periodicAbilityCoroutines[ability]);
                    periodicAbilityCoroutines.Remove(ability);
                }

                Debug.Log($"[{gameObject.name}] 특수능력 제거: {ability.abilityName}");
            }
        }

        // 디버그용 - 모든 능력 즉시 발동
        [ContextMenu("모든 특수능력 테스트")]
        public void TestAllAbilities()
        {
            foreach (var ability in specialAbilities)
            {
                if (ability != null)
                {
                    ability.Execute(gameObject);
                    Debug.Log($"[테스트] {ability.abilityName} 발동!");
                }
            }
        }
        #endregion
    }
}   