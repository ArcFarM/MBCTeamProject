using UnityEngine;
using MainGame.Units;
using MainGame.Units.Battle;
using MainGame.Enum;
using System.Collections;

namespace JiHoon
{
    [CreateAssetMenu(fileName = "Ability_Revive", menuName = "Game/Abilities/Revive")]
    public class ReviveAbility : AbilityData
    {
        [Header("부활 설정")]
        [SerializeField] private float reviveHealthPercent = 0.3f;
        [SerializeField] private float reviveDelay = 3f;

        private static System.Collections.Generic.Dictionary<GameObject, bool> hasRevived = new System.Collections.Generic.Dictionary<GameObject, bool>();
        private static System.Collections.Generic.Dictionary<GameObject, bool> isReviving = new System.Collections.Generic.Dictionary<GameObject, bool>();

        public override void Execute(GameObject caster, GameObject target = null)
        {
            if (caster == null) return;

            if ((hasRevived.ContainsKey(caster) && hasRevived[caster]) ||
                (isReviving.ContainsKey(caster) && isReviving[caster]))
            {
                Debug.Log($"[{caster.name}] 이미 부활했거나 부활 중입니다.");
                return;
            }

            isReviving[caster] = true;
            caster.GetComponent<MonoBehaviour>().StartCoroutine(ReviveCoroutine(caster));
        }

        private IEnumerator ReviveCoroutine(GameObject caster)
        {
            var unitBase = caster.GetComponent<MainGame.Units.UnitBase>();
            var battleBase = caster.GetComponent<BattleBase>();
            var unitAnim = caster.GetComponent<UnitAnim>();

            if (unitBase == null || battleBase == null)
            {
                isReviving[caster] = false;
                yield break;
            }

            Debug.Log($"[{caster.name}] {abilityName} 발동! {reviveDelay}초 후 부활 예정...");

            // 죽음 상태로 설정
            unitBase.SetStat(StatType.CurrHealth, 0);

            // 죽음 애니메이션 재생
            if (unitAnim != null)
            {
                unitAnim.SetAnimTrigger("4_Death");
                unitAnim.SetAnimBool("isDeath", true);
            }

            // BattleBase의 상태를 Dead로 설정
            var stateField = battleBase.GetType().GetField("currentState",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            if (stateField != null)
            {
                stateField.SetValue(battleBase, CombatState.Dead);
            }

            // 모든 적들에게 이 유닛이 죽었다고 알림
            NotifyAllEnemiesOfDeath(caster);

            // 부활 대기
            yield return new WaitForSeconds(reviveDelay);

            // ========== 부활 처리 시작 ==========

            // 체력 회복
            float maxHealth = unitBase.GetStat(StatType.BaseHealth);
            float reviveHealth = maxHealth * reviveHealthPercent;
            unitBase.SetStat(StatType.CurrHealth, reviveHealth);

            // 부활 완료 표시
            hasRevived[caster] = true;
            isReviving[caster] = false;

            // ★ 전투 시스템 완전 초기화 ★
            // combatTargetList 초기화
            var combatListField = battleBase.GetType().GetField("combatTargetList",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            if (combatListField != null)
            {
                var list = combatListField.GetValue(battleBase) as System.Collections.Generic.List<GameObject>;
                list?.Clear();
            }

            // lockedTarget 초기화
            var lockedTargetField = battleBase.GetType().GetField("lockedTarget",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            if (lockedTargetField != null)
            {
                lockedTargetField.SetValue(battleBase, null);
            }

            // ★ movementCoroutine 초기화 ★
            var movementCoroutineField = battleBase.GetType().GetField("movementCoroutine",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            if (movementCoroutineField != null)
            {
                var coroutine = movementCoroutineField.GetValue(battleBase) as Coroutine;
                if (coroutine != null)
                {
                    battleBase.StopCoroutine(coroutine);
                    movementCoroutineField.SetValue(battleBase, null);
                }
            }

            // 애니메이션 초기화
            if (unitAnim != null)
            {
                unitAnim.SetAnimBool("isDeath", false);
                unitAnim.SetAnimBool("1_Move", false);
            }

            // ★ 중요: 상태를 Idle로 설정하기 전에 잠시 대기 ★
            yield return null;

            // 상태를 Idle로 변경
            if (stateField != null)
            {
                stateField.SetValue(battleBase, CombatState.Idle);
            }

            // ★ 전투 시스템이 이미 실행 중인지 확인하고 재시작 ★
            var stateMachineField = battleBase.GetType().GetField("stateMachineCoroutine",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            if (stateMachineField != null)
            {
                var existingCoroutine = stateMachineField.GetValue(battleBase) as Coroutine;
                if (existingCoroutine == null)
                {
                    // 상태 머신이 멈춰있으면 재시작
                    var startMethod = battleBase.GetType().GetMethod("StartStateMachine",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);
                    startMethod?.Invoke(battleBase, null);
                }
            }

            Debug.Log($"[{caster.name}] 부활 완료! 체력: {reviveHealth}/{maxHealth}");

            // ★ 부활 후 즉시 적 탐지하도록 상태 변경 ★
            yield return new WaitForSeconds(0.1f);

            // Detecting 상태로 강제 전환
            if (stateField != null)
            {
                stateField.SetValue(battleBase, CombatState.Detecting);
                Debug.Log($"[{caster.name}] 부활 후 즉시 Detecting 상태로 전환");
            }

            // 근처 적들에게도 알림
            NotifyNearbyEnemiesOfRevive(caster);
        }

        public static bool HasRevived(GameObject unit)
        {
            return hasRevived.ContainsKey(unit) && hasRevived[unit];
        }

        // ★ 모든 적에게 죽음 알림 ★
        private void NotifyAllEnemiesOfDeath(GameObject deadUnit)
        {
            var allUnits = GameObject.FindObjectsOfType<BattleBase>();
            foreach (var unit in allUnits)
            {
                if (unit.gameObject == deadUnit) continue;

                // combatTargetList에서 제거
                var combatListField = unit.GetType().GetField("combatTargetList",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                if (combatListField != null)
                {
                    var list = combatListField.GetValue(unit) as System.Collections.Generic.List<GameObject>;
                    if (list != null && list.Contains(deadUnit))
                    {
                        list.Remove(deadUnit);
                        Debug.Log($"[{unit.gameObject.name}]의 타겟 리스트에서 {deadUnit.name} 제거");
                    }
                }

                // lockedTarget 해제
                var lockedTargetField = unit.GetType().GetField("lockedTarget",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                if (lockedTargetField != null)
                {
                    var lockedTarget = lockedTargetField.GetValue(unit) as GameObject;
                    if (lockedTarget == deadUnit)
                    {
                        lockedTargetField.SetValue(unit, null);
                        Debug.Log($"[{unit.gameObject.name}]의 lockedTarget 해제");
                    }
                }
            }
        }


        public override void Initialize(GameObject caster)
        {
            if (!hasRevived.ContainsKey(caster))
            {
                hasRevived[caster] = false;
            }
            if (!isReviving.ContainsKey(caster))
            {
                isReviving[caster] = false;
            }

            Debug.Log($"[{caster.name}] {abilityName} 능력 초기화 완료");
        }

        public static void CleanupReviveRecord(GameObject unit)
        {
            if (hasRevived.ContainsKey(unit))
            {
                hasRevived.Remove(unit);
            }
            if (isReviving.ContainsKey(unit))
            {
                isReviving.Remove(unit);
            }
        }

        public static bool IsReviving(GameObject unit)
        {
            return isReviving.ContainsKey(unit) && isReviving[unit];
        }
        private void NotifyNearbyEnemiesOfRevive(GameObject revivedUnit)
        {
            var unitBase = revivedUnit.GetComponent<MainGame.Units.UnitBase>();
            if (unitBase == null) return;

            // 일정 범위 내의 모든 유닛 찾기
            Collider2D[] nearbyUnits = Physics2D.OverlapCircleAll(revivedUnit.transform.position, 10f);

            foreach (var collider in nearbyUnits)
            {
                if (collider.gameObject == revivedUnit) continue;

                var otherUnit = collider.GetComponent<MainGame.Units.UnitBase>();
                var otherBattle = collider.GetComponent<BattleBase>();

                if (otherUnit != null && otherBattle != null)
                {
                    // 적군인 경우만
                    if (otherUnit.GetFaction != unitBase.GetFaction)
                    {
                        // 상태를 Detecting으로 변경하여 다시 탐지하도록
                        var stateField = otherBattle.GetType().GetField("currentState",
                            System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Instance);

                        if (stateField != null)
                        {
                            var currentState = stateField.GetValue(otherBattle);
                            if (currentState.ToString() == "Idle" || currentState.ToString() == "Dead")
                            {
                                stateField.SetValue(otherBattle, CombatState.Detecting);
                                Debug.Log($"[{collider.gameObject.name}]을(를) Detecting 상태로 전환");
                            }
                        }
                    }
                }
            }
        }
    }
}