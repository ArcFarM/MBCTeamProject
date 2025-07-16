using UnityEngine;
using System.Collections.Generic;
using MainGame.Units;
using MainGame.Units.Battle;
using MainGame.Enum;
using JiHoon;

public class HeroAbility : MonoBehaviour
{
    [System.Serializable]
    public class WaveConfig
    {
        public int waveNumber;
        public float health = 100;
        public float damage = 10;
        public float attackRange = 1;
        public int targetCount = 1;
    }

    [Header("용사 웨이브별 설정")]
    public List<WaveConfig> waveConfigs = new List<WaveConfig>()
    {
        new WaveConfig { waveNumber = 1, health = 100, damage = 10, attackRange = 1, targetCount = 1 },
        new WaveConfig { waveNumber = 2, health = 200, damage = 20, attackRange = 1, targetCount = 1 },
        new WaveConfig { waveNumber = 3, health = 300, damage = 30, attackRange = 1, targetCount = 1 },
        new WaveConfig { waveNumber = 4, health = 400, damage = 40, attackRange = 1, targetCount = 1 },
        new WaveConfig { waveNumber = 5, health = 500, damage = 50, attackRange = 1, targetCount = 1 },
        new WaveConfig { waveNumber = 6, health = 600, damage = 70, attackRange = 1, targetCount = 1 },
        new WaveConfig { waveNumber = 7, health = 700, damage = 80, attackRange = 2, targetCount = 2 },
        new WaveConfig { waveNumber = 8, health = 800, damage = 90, attackRange = 2, targetCount = 2 },
        new WaveConfig { waveNumber = 9, health = 900, damage = 95, attackRange = 2, targetCount = 2 },
        new WaveConfig { waveNumber = 10, health = 1000, damage = 100, attackRange = 2, targetCount = 2 }
    };

    private EnemyUnitBase enemyUnit;
    private BattleBase battleBase;
    private int currentWaveNumber = 0;

    void Start()
    {
        enemyUnit = GetComponent<EnemyUnitBase>();
        battleBase = GetComponent<BattleBase>();

        if (enemyUnit == null)
        {
            Debug.LogError("[HeroAbility] EnemyUnitBase를 찾을 수 없습니다!");
            enabled = false;
            return;
        }

        // 초기 설정
        Invoke(nameof(Initialize), 0.5f);
    }

    void Initialize()
    {
        // 초기 웨이브 적용
        ApplyWaveStats();

        // 웨이브 변경 이벤트 구독
        if (WaveController.Instance != null)
        {
            WaveController.OnWaveStarted += OnWaveChanged;
        }
    }

    void OnDestroy()
    {
        if (WaveController.Instance != null)
        {
            WaveController.OnWaveStarted -= OnWaveChanged;
        }
    }

    private void OnWaveChanged(int waveNumber)
    {
        currentWaveNumber = waveNumber;
        ApplyWaveStats();
    }

    private void ApplyWaveStats()
    {
        // 현재 웨이브 번호 가져오기
        if (WaveController.Instance != null)
        {
            currentWaveNumber = WaveController.Instance.CurrentWaveNumber;
        }

        // 해당 웨이브 설정 찾기
        var config = waveConfigs.Find(x => x.waveNumber == currentWaveNumber);
        if (config == null)
        {
            config = waveConfigs[0]; // 기본값
        }

        Debug.Log($"[HeroAbility] Wave {currentWaveNumber} 스탯 적용 - HP: {config.health}, DMG: {config.damage}, Range: {config.attackRange}");

        // 스탯 적용
        ApplyStats(config);
    }

    private void ApplyStats(WaveConfig config)
    {
        // EnemyUnitBase 스탯 설정
        enemyUnit.SetStat(StatType.BaseHealth, config.health);
        enemyUnit.SetStat(StatType.CurrHealth, config.health);
        enemyUnit.SetStat(StatType.BaseDamage, config.damage);
        enemyUnit.SetStat(StatType.CurrDamage, config.damage);
        enemyUnit.SetStat(StatType.BaseRange, config.attackRange);
        enemyUnit.SetStat(StatType.CurrRange, config.attackRange);

        // BattleBase 설정 (Reflection 사용)
        if (battleBase != null)
        {
            var type = battleBase.GetType();

            // maxCombatTarget 설정
            var maxTargetField = type.GetField("maxCombatTarget",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            maxTargetField?.SetValue(battleBase, config.targetCount);

            // detectingRange 설정
            var detectRangeField = type.GetField("detectingRange",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            detectRangeField?.SetValue(battleBase, config.attackRange * 1.5f);
        }
    }

    // 디버그용
    [ContextMenu("현재 스탯 확인")]
    private void LogCurrentStats()
    {
        if (enemyUnit != null)
        {
            Debug.Log($"HP: {enemyUnit.GetStat(StatType.CurrHealth)}/{enemyUnit.GetStat(StatType.BaseHealth)}");
            Debug.Log($"Damage: {enemyUnit.GetStat(StatType.CurrDamage)}");
            Debug.Log($"Range: {enemyUnit.GetStat(StatType.CurrRange)}");
        }
    }

    [ContextMenu("강제 웨이브 적용")]
    private void ForceApplyWave()
    {
        ApplyWaveStats();
    }
}