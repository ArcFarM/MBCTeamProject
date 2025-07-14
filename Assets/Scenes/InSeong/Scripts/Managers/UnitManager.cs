using UnityEngine;
using System.Collections.Generic;
using MainGame.UI;
using System.Collections;       // ) .
using JeaYoon.Politics;       // ) .
using System.Linq;       // ) .

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



        /*
        // [X] ???.
        #region ▼▼▼▼▼ ??? ▼▼▼▼▼
        [Header("룰렛 효과 관련 변수")]
        [SerializeField] private List<GameObject> deployedUnits = new List<GameObject>();
        [SerializeField] private List<GameObject> enemyUnits = new List<GameObject>();
        [SerializeField] private List<GameObject> availablePets = new List<GameObject>();
        [SerializeField] private List<Transform> specialTiles = new List<Transform>();

        [Header("효과 배율")]
        public float damageMultiplier = 1f;
        public float enemyAttackMultiplier = 1f;
        public float attackSpeedMultiplier = 1f;
        public float preparationTimeMultiplier = 1f;

        [Header("효과 상태")]
        public bool isDamageToSatisfaction = false;
        public bool isMoneyPowerMode = false;
        public bool isPacifistMode = false;
        public bool isSpecialTileMode = false;
        public float moneyPowerRate = 0.1f;
        public float pacifistDesertionRate = 0.3f;
        public float enemyReviveChance = 0f;
        public float specialTileAttackBonus = 1.3f;
        public float specialTileHealthBonus = 1.2f;
        #endregion ▲▲▲▲▲ ??? ▲▲▲▲▲



        // [X] ???.
        #region ▼▼▼▼▼ ??? ▼▼▼▼▼
        void Start()
        {
            // 특수 타일들 자동 찾기
            FindSpecialTiles();
        }
        public void RemoveUnit(GameObject go)
        {
            if (deployedUnits.Contains(go))
            {
                deployedUnits.Remove(go);
            }
        }

        private void FindSpecialTiles()
        {
            // "SpecialTile" 태그를 가진 타일들 찾기
            GameObject[] tiles = GameObject.FindGameObjectsWithTag("SpecialTile");
            foreach (GameObject tile in tiles)
            {
                specialTiles.Add(tile.transform);
            }
        }

        #endregion ▲▲▲▲▲ ??? ▲▲▲▲▲



        // [X] RouletteEffectManager.
        #region ▼▼▼▼▼ RouletteEffectManager ▼▼▼▼▼
        // [◆] - ▶▶▶ SetDamageMultiplier → HD_001: 빠르고 강하게.
        public void SetDamageMultiplier(float multiplier)
        {
            // 설명 : 아군의 데미지가 2배가 되지만 적의 공격력이 1.5배, 준비시간이 절반으로 줄어듭니다.
            damageMultiplier = multiplier;

            // 모든 배치된 유닛에 즉시 적용
            foreach (GameObject unit in deployedUnits)
            {
                if (unit != null)
                {
                    var unitStats = unit.GetComponent<UnitStats>();
                    if (unitStats != null)
                    {
                        unitStats.SetDamageMultiplier(multiplier);
                    }
                }
            }

            Debug.Log($"아군 데미지 배율 설정: {multiplier}x");
        }

        // [◆] - ▶▶▶ SetEnemyAttackMultiplier → HD_001: 빠르고 강하게.
        public void SetEnemyAttackMultiplier(float multiplier)
        {
            // 설명 : 아군의 데미지가 2배가 되지만 적의 공격력이 1.5배, 준비시간이 절반으로 줄어듭니다.
            enemyAttackMultiplier = multiplier;

            // 모든 적 유닛에 즉시 적용
            foreach (GameObject enemy in enemyUnits)
            {
                if (enemy != null)
                {
                    var enemyStats = enemy.GetComponent<EnemyStats>();
                    if (enemyStats != null)
                    {
                        enemyStats.SetAttackMultiplier(multiplier);
                    }
                }
            }

            Debug.Log($"적군 공격력 배율 설정: {multiplier}x");
        }

        // [◆] - ▶▶▶ SetAttackSpeedMultiplier → HD_002: 느리고 약하게.
        public void SetAttackSpeedMultiplier(float multiplier)
        {
            // 설명 : 아군과 적의 공격속도가 0.5배, 준비시간이 두배로 늘어납니다.
            attackSpeedMultiplier = multiplier;

            // 모든 유닛에 공격속도 적용
            foreach (GameObject unit in deployedUnits)
            {
                if (unit != null)
                {
                    var unitStats = unit.GetComponent<UnitStats>();
                    if (unitStats != null)
                    {
                        unitStats.SetAttackSpeedMultiplier(multiplier);
                    }
                }
            }

            foreach (GameObject enemy in enemyUnits)
            {
                if (enemy != null)
                {
                    var enemyStats = enemy.GetComponent<EnemyStats>();
                    if (enemyStats != null)
                    {
                        enemyStats.SetAttackSpeedMultiplier(multiplier);
                    }
                }
            }

            Debug.Log($"공격속도 배율 설정: {multiplier}x");
        }

        // [◆] - ▶▶▶ SetPreparationTimeMultiplier → HD_002: 느리고 약하게.
        public void SetPreparationTimeMultiplier(float multiplier)
        {
            // 설명 : 아군과 적의 공격속도가 0.5배, 준비시간이 두배로 늘어납니다.
            preparationTimeMultiplier = multiplier;

            // 웨이브 매니저에 준비시간 변경 알림
            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.SetPreparationTimeMultiplier(multiplier);
            }

            Debug.Log($"준비시간 배율 설정: {multiplier}x");
        }

        // [◆] - ▶▶▶ GiveRandomPet → HD_007: 그냥 귀엽잖아요.
        public void GiveRandomPet()
        {
            // 설명 : 지정된 펫 하나가 지급됩니다.
            if (availablePets.Count > 0)
            {
                int randomIndex = Random.Range(0, availablePets.Count);
                GameObject selectedPet = availablePets[randomIndex];

                // 펫을 플레이어에게 지급
                if (selectedPet != null)
                {
                    GameObject petInstance = Instantiate(selectedPet);
                    deployedUnits.Add(petInstance);

                    Debug.Log($"랜덤 펫 지급: {selectedPet.name}");
                }
            }
            else
            {
                Debug.LogWarning("사용 가능한 펫이 없습니다!");
            }
        }

        // [◆] - ▶▶▶ SetDamageToSatisfaction → HD_011: 마조히스트.
        public void SetDamageToSatisfaction(bool enable)
        {
            // 설명 : 유닛들이 맞을 때 마다 불만이 0.01씩 내려갑니다.
            isDamageToSatisfaction = enable;

            // 모든 유닛에 마조히스트 효과 적용
            foreach (GameObject unit in deployedUnits)
            {
                if (unit != null)
                {
                    var unitStats = unit.GetComponent<UnitStats>();
                    if (unitStats != null)
                    {
                        unitStats.SetMasochistMode(enable);
                    }
                }
            }

            Debug.Log($"마조히스트 모드: {(enable ? "활성화" : "비활성화")}");
        }

        // [◆] - ▶▶▶ SetMoneyPowerMode → HD_013: 돈이 곧 힘.
        public void SetMoneyPowerMode(bool enable, float multiplier)
        {
            // 설명 : 재화 1당 아군 공격력이 0.1 씩 늘어납니다.
            isMoneyPowerMode = enable;
            moneyPowerRate = multiplier;

            if (enable)
            {
                StartCoroutine(MoneyPowerCoroutine());
            }

            Debug.Log($"돈이 곧 힘 모드: {(enable ? "활성화" : "비활성화")} (배율: {multiplier})");
        }

        private IEnumerator MoneyPowerCoroutine()
        {
            while (isMoneyPowerMode)
            {
                if (ResourceManager.Instance != null)
                {
                    float currentMoney = ResourceManager.Instance.GetMoney();
                    float powerBonus = currentMoney * moneyPowerRate;

                    // 모든 유닛에 돈 기반 공격력 보너스 적용
                    foreach (GameObject unit in deployedUnits)
                    {
                        if (unit != null)
                        {
                            var unitStats = unit.GetComponent<UnitStats>();
                            if (unitStats != null)
                            {
                                unitStats.SetMoneyPowerBonus(powerBonus);
                            }
                        }
                    }
                }

                yield return new WaitForSeconds(1f);
            }
        }

        // [◆] - ▶▶▶ RandomTransformUnits → HD_014: 뿜!.
        public void RandomTransformUnits(float chance)
        {
            // 설명 : 웨이브가 지날 때마다. 필드의 아군이 20% 확률로 다른 유닛으로 변하거나 사라집니다.
            List<GameObject> unitsToTransform = new List<GameObject>();

            foreach (GameObject unit in deployedUnits)
            {
                if (unit != null && Random.Range(0f, 1f) < chance)
                {
                    unitsToTransform.Add(unit);
                }
            }

            foreach (GameObject unit in unitsToTransform)
            {
                Vector3 position = unit.transform.position;
                deployedUnits.Remove(unit);

                if (Random.Range(0f, 1f) < 0.5f)
                {
                    // 50% 확률로 사라짐
                    Destroy(unit);
                    Debug.Log("유닛이 사라졌습니다!");
                }
                else
                {
                    // 50% 확률로 다른 유닛으로 변환
                    Destroy(unit);
                    if (availablePets.Count > 0)
                    {
                        int randomIndex = Random.Range(0, availablePets.Count);
                        GameObject newUnit = Instantiate(availablePets[randomIndex], position, Quaternion.identity);
                        deployedUnits.Add(newUnit);
                        Debug.Log($"유닛이 {newUnit.name}로 변환되었습니다!");
                    }
                }
            }
        }

        // [◆] - ▶▶▶ ApplyPlagueDamage → HD_015: 역병.
        public void ApplyPlagueDamage(float damageRate)
        {
            // 설명 : 모든 유닛의 체력이 초당 0.1% 씩 깎입니다.
            foreach (GameObject unit in deployedUnits)
            {
                if (unit != null)
                {
                    var unitStats = unit.GetComponent<UnitStats>();
                    if (unitStats != null)
                    {
                        unitStats.ApplyPlagueDamage(damageRate);
                    }
                }
            }

            foreach (GameObject enemy in enemyUnits)
            {
                if (enemy != null)
                {
                    var enemyStats = enemy.GetComponent<EnemyStats>();
                    if (enemyStats != null)
                    {
                        enemyStats.ApplyPlagueDamage(damageRate);
                    }
                }
            }
        }

        // [◆] - ▶▶▶ SetEnemyReviveChance → HD_010: 새벽의 저주.
        public void SetEnemyReviveChance(float chance)
        {
            // 설명 : 적들이 50%의 체력으로 한번 더 살아납니다.
            enemyReviveChance = chance;

            foreach (GameObject enemy in enemyUnits)
            {
                if (enemy != null)
                {
                    var enemyStats = enemy.GetComponent<EnemyStats>();
                    if (enemyStats != null)
                    {
                        enemyStats.SetReviveChance(chance);
                    }
                }
            }

            Debug.Log($"적 부활 확률 설정: {chance * 100}%");
        }

        // [◆] - ▶▶▶ SetPacifistMode → HD_019: 네? 잘못들었슘다?.
        public void SetPacifistMode(bool enable, float desertionRate)
        {
            // 설명 : 아군 유닛의 체력이 100% 늘어나지만 타격하지 않습니다. 웨이브가 끝나고 30% 확률로 탈영합니다.
            isPacifistMode = enable;
            pacifistDesertionRate = desertionRate;

            foreach (GameObject unit in deployedUnits)
            {
                if (unit != null)
                {
                    ApplyPacifistEffect(unit);
                }
            }

            Debug.Log($"평화주의 모드: {(enable ? "활성화" : "비활성화")} (탈영률: {desertionRate * 100}%)");
        }

        private void ApplyPacifistEffect(GameObject unit)
        {
            var unitStats = unit.GetComponent<UnitStats>();
            if (unitStats != null)
            {
                unitStats.SetHealthMultiplier(2f);  // 체력 100% 증가
                unitStats.SetCanAttack(false);     // 공격 불가
                unitStats.SetDesertionChance(pacifistDesertionRate);
            }
        }

        // [◆] - ▶▶▶ SetSpecialTileBonus → HD_020: 방장 사기맵.
        public void SetSpecialTileBonus(bool enable, float attackBonus, float healthBonus)
        {
            // 설명 : 특정 타일에 위치한 아군의 공격력이 30% 체력이 20%늘어납니다.
            isSpecialTileMode = enable;
            specialTileAttackBonus = attackBonus;
            specialTileHealthBonus = healthBonus;

            foreach (GameObject unit in deployedUnits)
            {
                if (unit != null)
                {
                    ApplySpecialTileEffect(unit);
                }
            }

            Debug.Log($"특수 타일 모드: {(enable ? "활성화" : "비활성화")} (공격력: {attackBonus}x, 체력: {healthBonus}x)");
        }

        private void ApplySpecialTileEffect(GameObject unit)
        {
            if (!isSpecialTileMode) return;

            bool isOnSpecialTile = false;
            foreach (Transform tile in specialTiles)
            {
                if (Vector3.Distance(unit.transform.position, tile.position) < 1f)
                {
                    isOnSpecialTile = true;
                    break;
                }
            }

            if (isOnSpecialTile)
            {
                var unitStats = unit.GetComponent<UnitStats>();
                if (unitStats != null)
                {
                    unitStats.SetSpecialTileBonus(specialTileAttackBonus, specialTileHealthBonus);
                }
            }
        }

        // 적 유닛 등록/해제
        public void RegisterEnemy(GameObject enemy)
        {
            if (enemy != null && !enemyUnits.Contains(enemy))
            {
                enemyUnits.Add(enemy);

                // 현재 활성화된 효과들 적용
                var enemyStats = enemy.GetComponent<EnemyStats>();
                if (enemyStats != null)
                {
                    enemyStats.SetAttackMultiplier(enemyAttackMultiplier);
                    enemyStats.SetAttackSpeedMultiplier(attackSpeedMultiplier);
                    enemyStats.SetReviveChance(enemyReviveChance);
                }
            }
        }

        public void UnregisterEnemy(GameObject enemy)
        {
            if (enemyUnits.Contains(enemy))
            {
                enemyUnits.Remove(enemy);
            }
        }

        // 웨이브 종료 시 호출
        public void OnWaveEnd()
        {
            if (isPacifistMode)
            {
                HandlePacifistDesertion();
            }
        }

        private void HandlePacifistDesertion()
        {
            List<GameObject> unitsToRemove = new List<GameObject>();

            foreach (GameObject unit in deployedUnits)
            {
                if (unit != null && Random.Range(0f, 1f) < pacifistDesertionRate)
                {
                    unitsToRemove.Add(unit);
                }
            }

            foreach (GameObject unit in unitsToRemove)
            {
                deployedUnits.Remove(unit);
                Destroy(unit);
                Debug.Log("유닛이 탈영했습니다!");
            }
        }

        // 마조히스트 효과로 불만 감소
        public void OnUnitTakeDamage()
        {
            if (isDamageToSatisfaction && PoliticsManager.Instance != null)
            {
                PoliticsManager.Instance.AddDissatisfaction(-0.01f);
            }
        }
        #endregion ▲▲▲▲▲ RouletteEffectManager ▲▲▲▲▲
        */
    }
}