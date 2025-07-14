using JiHoon;       // ) ShopManager 활성화.
using MainGame.Manager;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.Events;
using JeaYoon.Politics;         // ) PoliticsManager 활성화.

namespace JeaYoon.Roulette
{
    public class RouletteEffectManager : MonoBehaviour
    {
        public static RouletteEffectManager Instance { get; private set; }

        [Header("게임 매니저 참조")]
        public GameManager gameManager;
        public ShopManager shopManager;
        public WaveManager waveManager;
        public UnitManager unitManager;
        public ResourceManager resourceManager;
        public PoliticsManager politicsManager;

        [Header("현재 활성 효과")]
        public List<string> activeEffects = new List<string>();

        [Header("효과 상태 변수")]
        public bool isCorruption = false;           // 부정 부패
        public bool isLazy = false;                 // 게으름
        public bool isEvilShop = false;             // 악덕 상점
        public bool isLaborExploitation = false;    // 노동 착취
        public bool isHeroMode = false;             // 뒤바뀐 입장
        public bool isDawnCurse = false;            // 새벽의 저주
        public bool isMasochist = false;            // 마조히스트
        public bool isMoneyPower = false;           // 돈이 곧 힘
        public bool isPlague = false;               // 역병
        public bool isSlimeRebellion = false;       // 슬라임 반란
        public bool isInfluencer = false;           // 인플루언서
        public bool isPacifist = false;             // 네? 잘못들었슘다?
        public bool isSpecialTile = false;          // 방장 사기맵

        [Header("효과 수치")]
        public float corruptionRate = 1f;           // 부정 부패 증가율
        public float plagueRate = 0.001f;           // 역병 데미지 (0.1% per second)
        public float moneyPowerMultiplier = 0.1f;   // 돈이 곧 힘 배율
        public float pacifistDesertion = 0.3f;      // 탈영 확률

        [Header("이벤트")]
        public UnityEvent<string> OnEffectApplied;
        public UnityEvent<string> OnEffectRemoved;

        private Coroutine corruptionCoroutine;
        private Coroutine plagueCoroutine;
        private Coroutine transformationCoroutine;





        /*
        // RouletteEffectManager.cs의 ApplyRouletteEffect 메서드 주석을 해제하고 이것으로 교체하세요

        public void ApplyRouletteEffect(string effectID, string effectName, string effectDescription)
        {
            Debug.Log($"🎯 룰렛 효과 적용 시작: ID={effectID}, Name={effectName}");

            // 이미 활성화된 효과라면 중복 적용 방지
            if (activeEffects.Contains(effectID))
            {
                Debug.LogWarning($"효과 {effectID}가 이미 활성화되어 있습니다.");
                return;
            }

            activeEffects.Add(effectID);
            OnEffectApplied?.Invoke(effectName);

            switch (effectID)
            {
                case "HD_001": ApplyFastAndStrong(); break;
                case "HD_002": ApplySlowAndWeak(); break;
                case "HD_006": ApplyEvilShop(); break;     // 악덕 상점
                case "HD_007": ApplyFreePet(); break;     // 그냥 귀엽잖아요
                case "HD_012": ApplyGoodStart(); break;   // 좋은출발
                default:
                    Debug.LogError($"❌ 알 수 없는 효과 ID: {effectID}");
                    // 효과를 찾을 수 없으면 활성 효과 목록에서 제거
                    activeEffects.Remove(effectID);
                    break;
            }

            Debug.Log($"🎯 룰렛 효과 적용 완료: {effectID} - {effectName}");
        }

        // 추가: 주석 해제된 메서드들
        private void ApplyFastAndStrong()
        {
            Debug.Log("🎯 빠르고 강하게 효과 적용");
            if (unitManager != null)
            {
                unitManager.SetDamageMultiplier(2f);
                unitManager.SetEnemyAttackMultiplier(1.5f);
                unitManager.SetPreparationTimeMultiplier(0.5f);
            }
        }

        private void ApplySlowAndWeak()
        {
            Debug.Log("🎯 느리고 약하게 효과 적용");
            if (unitManager != null)
            {
                unitManager.SetAttackSpeedMultiplier(0.5f);
                unitManager.SetPreparationTimeMultiplier(2f);
            }
        }

        private void ApplyEvilShop()
        {
            Debug.Log("🎯 악덕 상점 효과 적용");
            isEvilShop = true;
            if (shopManager != null)
            {
                shopManager.SetRandomPricing(true);
            }
        }

        private void ApplyFreePet()
        {
            Debug.Log("🎯 그냥 귀엽잖아요 효과 적용");
            if (unitManager != null)
            {
                unitManager.GiveRandomPet();
            }
        }

        private void ApplyGoodStart()
        {
            Debug.Log("🎯 좋은출발 효과 적용");
            if (shopManager != null)
            {
                shopManager.AddAdvancedTowers(2);
            }
        }
        */





        /*
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public void ApplyRouletteEffect(string effectID, string effectName, string effectDescription)
        {
            Debug.Log($"🎯 룰렛 효과 적용: {effectID} - {effectName}");

            // 이미 활성화된 효과라면 중복 적용 방지
            if (activeEffects.Contains(effectID))
            {
                Debug.LogWarning($"효과 {effectID}가 이미 활성화되어 있습니다.");
                return;
            }

            activeEffects.Add(effectID);
            OnEffectApplied?.Invoke(effectName);

            switch (effectID)
            {
                case "HD_001": ApplyFastAndStrong(); break;
                case "HD_002": ApplySlowAndWeak(); break;
                case "HD_003": ApplyLaziness(); break;
                case "HD_004": ApplyCorruption(); break;
                case "HD_005": ApplyCuteSlime(); break;
                case "HD_006": ApplyEvilShop(); break;
                case "HD_007": ApplyFreePet(); break;
                case "HD_008": ApplyLaborExploitation(); break;
                case "HD_009": ApplyHeroMode(); break;
                case "HD_010": ApplyDawnCurse(); break;
                case "HD_011": ApplyMasochist(); break;
                case "HD_012": ApplyGoodStart(); break;
                case "HD_013": ApplyMoneyPower(); break;
                case "HD_014": ApplyTransformation(); break;
                case "HD_015": ApplyPlague(); break;
                case "HD_016": ApplySlimeRebellion(); break;
                case "HD_017": ApplySingleLane(); break;
                case "HD_018": ApplyInfluencer(); break;
                case "HD_019": ApplyPacifist(); break;
                case "HD_020": ApplySpecialTile(); break;
                case "HD_021": ApplyAllOrNothing(); break;
                default:
                    Debug.LogError($"알 수 없는 효과 ID: {effectID}");
                    break;
            }
        }

        // HD_001: 빠르고 강하게
        private void ApplyFastAndStrong()
        {
            if (unitManager != null)
            {
                unitManager.SetDamageMultiplier(2f);        // 아군 데미지 2배
                unitManager.SetEnemyAttackMultiplier(1.5f); // 적 공격력 1.5배
                unitManager.SetPreparationTimeMultiplier(0.5f); // 준비시간 절반
            }
        }

        // HD_002: 느리고 약하게
        private void ApplySlowAndWeak()
        {
            if (unitManager != null)
            {
                unitManager.SetAttackSpeedMultiplier(0.5f); // 공격속도 0.5배
                unitManager.SetPreparationTimeMultiplier(2f); // 준비시간 2배
            }
        }

        // HD_003: 게으름
        private void ApplyLaziness()
        {
            isLazy = true;
            if (waveManager != null)
            {
                waveManager.SetLazyMode(true);
            }
        }

        // HD_004: 부정 부패
        private void ApplyCorruption()
        {
            isCorruption = true;
            if (corruptionCoroutine != null) StopCoroutine(corruptionCoroutine);
            corruptionCoroutine = StartCoroutine(CorruptionEffect());
        }

        private IEnumerator CorruptionEffect()
        {
            while (isCorruption)
            {
                if (resourceManager != null && politicsManager != null)
                {
                    resourceManager.AddMoney(corruptionRate * Time.deltaTime);
                    float dissatisfactionIncrease = corruptionRate * Time.deltaTime;

                    // 지배 비율에 비례해 불만 감소
                    float dominanceRatio = politicsManager.GetDominanceRatio();
                    dissatisfactionIncrease *= (1f - dominanceRatio);

                    politicsManager.AddDissatisfaction(dissatisfactionIncrease);
                }
                yield return null;
            }
        }

        // HD_005: 귀여운 슬라임
        private void ApplyCuteSlime()
        {
            if (shopManager != null)
            {
                shopManager.EnableSlimeKing(true);
            }
        }

        // HD_006: 악덕 상점
        private void ApplyEvilShop()
        {
            isEvilShop = true;
            if (shopManager != null)
            {
                shopManager.SetRandomPricing(true);
            }
        }

        // HD_007: 그냥 귀엽잖아요
        private void ApplyFreePet()
        {
            if (unitManager != null)
            {
                unitManager.GiveRandomPet();
            }
        }

        // HD_008: 노동 착취
        private void ApplyLaborExploitation()
        {
            isLaborExploitation = true;
            if (resourceManager != null)
            {
                resourceManager.SetLaborExploitation(true);
            }
        }

        // HD_009: 뒤바뀐 입장
        private void ApplyHeroMode()
        {
            isHeroMode = true;
            if (gameManager != null)
            {
                gameManager.SetHeroMode(true);
            }
        }

        // HD_010: 새벽의 저주
        private void ApplyDawnCurse()
        {
            isDawnCurse = true;
            if (unitManager != null)
            {
                unitManager.SetEnemyReviveChance(0.5f);
            }
        }

        // HD_011: 마조히스트
        private void ApplyMasochist()
        {
            isMasochist = true;
            if (unitManager != null)
            {
                unitManager.SetDamageToSatisfaction(true);
            }
        }

        // HD_012: 좋은 출발
        private void ApplyGoodStart()
        {
            if (shopManager != null)
            {
                shopManager.AddAdvancedTowers(2);
            }
        }

        // HD_013: 돈이 곧 힘
        private void ApplyMoneyPower()
        {
            isMoneyPower = true;
            if (unitManager != null)
            {
                unitManager.SetMoneyPowerMode(true, moneyPowerMultiplier);
            }
        }

        // HD_014: 뿜!
        private void ApplyTransformation()
        {
            if (transformationCoroutine != null) StopCoroutine(transformationCoroutine);
            transformationCoroutine = StartCoroutine(TransformationEffect());
        }

        private IEnumerator TransformationEffect()
        {
            while (true)
            {
                if (waveManager != null && waveManager.IsWaveActive())
                {
                    yield return new WaitForSeconds(waveManager.GetWaveInterval());

                    if (unitManager != null)
                    {
                        unitManager.RandomTransformUnits(0.2f);
                    }
                }
                yield return null;
            }
        }

        // HD_015: 역병
        private void ApplyPlague()
        {
            isPlague = true;
            if (plagueCoroutine != null) StopCoroutine(plagueCoroutine);
            plagueCoroutine = StartCoroutine(PlagueEffect());
        }

        private IEnumerator PlagueEffect()
        {
            while (isPlague)
            {
                if (unitManager != null)
                {
                    unitManager.ApplyPlagueDamage(plagueRate);
                }
                yield return new WaitForSeconds(1f);
            }
        }

        // HD_016: 슬라임 반란
        private void ApplySlimeRebellion()
        {
            isSlimeRebellion = true;
            if (shopManager != null)
            {
                shopManager.ReplaceAllUnitsWithSlime(true);
            }
        }

        // HD_017: 로, 하이, 미들, 하이,
        private void ApplySingleLane()
        {
            if (waveManager != null)
            {
                waveManager.SetSingleLaneMode(true);
            }
        }

        // HD_018: 오~늘온 마왕성에 다녀왔어요!
        private void ApplyInfluencer()
        {
            isInfluencer = true;
            if (waveManager != null)
            {
                waveManager.SetEnemyMultiplier(1.5f);
            }
        }

        // HD_019: 네? 잘못들었슘다?
        private void ApplyPacifist()
        {
            isPacifist = true;
            if (unitManager != null)
            {
                unitManager.SetPacifistMode(true, pacifistDesertion);
            }
        }

        // HD_020: 방장 사기맵
        private void ApplySpecialTile()
        {
            isSpecialTile = true;
            if (unitManager != null)
            {
                unitManager.SetSpecialTileBonus(true, 1.3f, 1.2f);
            }
        }


        // HD_021: 모 아니면 도
        private void ApplyAllOrNothing()
        {
            if (politicsManager != null)
            {
                politicsManager.SetPolicyCardDistribution(0.4f, 0f, 0.3f, 0.3f);
            }
        }

        public void RemoveEffect(string effectID)
        {
            if (activeEffects.Contains(effectID))
            {
                activeEffects.Remove(effectID);
                OnEffectRemoved?.Invoke(effectID);

                // 효과 제거 로직
                switch (effectID)
                {
                    case "HD_004":
                        isCorruption = false;
                        if (corruptionCoroutine != null) StopCoroutine(corruptionCoroutine);
                        break;
                    case "HD_015":
                        isPlague = false;
                        if (plagueCoroutine != null) StopCoroutine(plagueCoroutine);
                        break;
                    case "HD_014":
                        if (transformationCoroutine != null) StopCoroutine(transformationCoroutine);
                        break;
                }
            }
        }

        public void ClearAllEffects()
        {
            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                RemoveEffect(activeEffects[i]);
            }
        }

        public bool HasEffect(string effectID)
        {
            return activeEffects.Contains(effectID);
        }

        public List<string> GetActiveEffects()
        {
            return new List<string>(activeEffects);
        }

        private void OnDestroy()
        {
            if (corruptionCoroutine != null) StopCoroutine(corruptionCoroutine);
            if (plagueCoroutine != null) StopCoroutine(plagueCoroutine);
            if (transformationCoroutine != null) StopCoroutine(transformationCoroutine);
        }
                */
    }
}

/*
 추가 확인사항
 // UnitManager에 추가할 메서드들
public void SetDamageMultiplier(float multiplier);
public void SetAttackSpeedMultiplier(float multiplier);
public void ApplyPlagueDamage(float rate);

// ShopManager에 추가할 메서드들
public void EnableSlimeKing(bool enable);
public void SetRandomPricing(bool enable);
 
 */