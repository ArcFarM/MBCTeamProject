using JiHoon;
using System.Collections.Generic;
using UnityEngine;

/* [0] 개요 : AAARouletteEffectApplier
		- 룰렛 효과 적용 시스템
*/

namespace JeaYoon.Roulette
{
    public class AAARouletteEffectApplier : MonoBehaviour
    {
        // [1] Variable.
        #region ▼▼▼▼▼ Variable ▼▼▼▼▼
        // [◆] - ▶▶▶ Header.
        [Header("펫 프리팹들")]
        public List<GameObject> availablePets = new List<GameObject>();
        [Header("상점 관련")]
        public ShopManager shopManager;
        #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲





        // [2] Unity Event Method.
        #region ▼▼▼▼▼ Unity Event Method ▼▼▼▼▼
        // [◆] - ▶▶▶ ApplyRouletteEffect.
        public void ApplyRouletteEffect(string effectName)
        {
            switch (effectName)
            {
                case "빠르고 강하게":
                    ApplyFastAndStrong();
                    break;
                case "느리고 약하게":
                    ApplySlowAndWeak();
                    break;
                case "악덕 상점":
                    ApplyEvilShop();
                    break;
                case "그냥 귀엽잖아요":
                    ApplyRandomPet();
                    break;
                case "좋은 출발":
                    ApplyGoodStart();
                    break;
                default:
                    Debug.LogWarning($"알 수 없는 룰렛 효과: {effectName}");
                    break;
            }
        }
        #endregion ▲▲▲▲▲ Unity Event Method ▲▲▲▲▲





        // [3] Custom Method.
        #region ▼▼▼▼▼ Custom Method ▼▼▼▼▼
        // [◆] - ▶▶▶ ApplyFastAndStrong → 빠르고 강하게.
        private void ApplyFastAndStrong()
        {
            AAARouletteStateManager.Instance.allyDamageMultiplier = 2.0f;
            AAARouletteStateManager.Instance.enemyDamageMultiplier = 1.5f;
            AAARouletteStateManager.Instance.preparationTimeMultiplier = 0.5f;

            Debug.Log("빠르고 강하게 효과 적용: 아군 데미지 2배, 적 공격력 1.5배, 준비시간 절반");

            // 현재 활성화된 모든 유닛에 즉시 적용
            // ) ApplyToAllUnits();
        }


        // [◆] - ▶▶▶ ApplySlowAndWeak → 느리고 약하게.
        private void ApplySlowAndWeak()
        {
            AAARouletteStateManager.Instance.allyAttackSpeedMultiplier = 0.5f;
            AAARouletteStateManager.Instance.enemyAttackSpeedMultiplier = 0.5f;
            AAARouletteStateManager.Instance.preparationTimeMultiplier = 2.0f;

            Debug.Log("느리고 약하게 효과 적용: 모든 공격속도 0.5배, 준비시간 2배");

            // 현재 활성화된 모든 유닛에 즉시 적용
            // ) ApplyToAllUnits();
        }


        // [◆] - ▶▶▶ ApplyEvilShop → 악덕 상점.
        private void ApplyEvilShop()
        {
            AAARouletteStateManager.Instance.hasRandomShopPrices = true;

            // 20% 확률로 상점 잠금
            if (Random.Range(0f, 1f) < 0.2f)
            {
                AAARouletteStateManager.Instance.isShopLocked = true;
                Debug.Log("악덕 상점 효과 적용: 상점이 잠겼습니다!");
            }
            else
            {
                Debug.Log("악덕 상점 효과 적용: 상점 가격이 랜덤하게 설정됩니다.");
            }

            // 상점 매니저에 변경사항 알림
            if (shopManager != null)
            {
                // shopManager.OnEvilShopEffectApplied();
            }
        }


        // [◆] - ▶▶▶ ApplyRandomPet → 그냥 귀엽잖아요.
        private void ApplyRandomPet()
        {
            if (availablePets.Count > 0)
            {
                int randomIndex = Random.Range(0, availablePets.Count);
                GameObject selectedPet = availablePets[randomIndex];

                // 펫 생성 및 플레이어에게 지급
                GameObject newPet = Instantiate(selectedPet);
                AAARouletteStateManager.Instance.grantedPets.Add(newPet);

                Debug.Log($"그냥 귀엽잖아요 효과 적용: {selectedPet.name} 펫이 지급되었습니다!");

                // 펫 매니저에 알림 (있다면)
                AAAPetManager petManager = FindObjectOfType<AAAPetManager>();
                if (petManager != null)
                {
                    petManager.AddPet(newPet);
                }
            }
            else
            {
                Debug.LogWarning("사용 가능한 펫이 없습니다!");
            }
        }


        // [◆] - ▶▶▶ ApplyGoodStart → 좋은 출발.
        private void ApplyGoodStart()
        {
            AAARouletteStateManager.Instance.hasGoodStart = true;
            Debug.Log("좋은 출발 효과 적용: 첫 상점에서 2단계 이상 포탑 2개가 나타납니다!");

            // 상점 매니저에 변경사항 알림
            if (shopManager != null)
            {
                // shopManager.OnGoodStartEffectApplied();
            }
        }
        /*
                namespace JiHoon / ShopManager를 직접적으로 수정하지 않고, 아이템 데이터(포탑)를 하나 만들기.
                룰렛에서 '좋은 출발'가 걸리면 상점에서 아까 만든 아이템 데이터(카드덱)을 추가 → 구매하는건 구현되어있음.

                ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                // 상점 매니저 (기본 구조)
public class ShopManager : MonoBehaviour
{
    [Header("상점 설정")]
    public List<ShopItem> shopItems = new List<ShopItem>();
    public int[] normalPrices = { 100, 200, 300, 500, 800 };
    
    public void OnEvilShopEffectApplied()
    {
        if (GameStateManager.Instance.isShopLocked)
        {
            // 상점 UI 비활성화
            gameObject.SetActive(false);
            return;
        }
        
        if (GameStateManager.Instance.hasRandomShopPrices)
        {
            // 상점 가격 랜덤화
            RandomizeShopPrices();
        }
    }
    
    public void OnGoodStartEffectApplied()
    {
        if (GameStateManager.Instance.hasGoodStart)
        {
            // 첫 상점에서 고급 포탑 2개 보장
            EnsureHighTierTowers();
        }
    }
    
    private void RandomizeShopPrices()
    {
        foreach (ShopItem item in shopItems)
        {
            // 원래 가격의 50%~200% 범위에서 랜덤 설정
            float randomMultiplier = Random.Range(0.5f, 2.0f);
            item.currentPrice = Mathf.RoundToInt(item.originalPrice * randomMultiplier);
        }
        
        Debug.Log("상점 가격이 랜덤하게 변경되었습니다!");
    }
    
    private void EnsureHighTierTowers()
    {
        // 2단계 이상의 포탑을 상점 상위에 배치
        List<ShopItem> highTierItems = shopItems.FindAll(item => item.tier >= 2);
        
        if (highTierItems.Count >= 2)
        {
            // 상점 첫 두 슬롯에 고급 포탑 배치
            shopItems[0] = highTierItems[Random.Range(0, highTierItems.Count)];
            shopItems[1] = highTierItems[Random.Range(0, highTierItems.Count)];
        }
        
        Debug.Log("좋은 출발 효과로 고급 포탑이 상점에 배치되었습니다!");
    }
}

// 상점 아이템 클래스
[System.Serializable]
public class ShopItem
{
    public string itemName;
    public int tier;
    public int originalPrice;
    public int currentPrice;
    public GameObject itemPrefab;
    
    public ShopItem(string name, int itemTier, int price, GameObject prefab)
    {
        itemName = name;
        tier = itemTier;
        originalPrice = price;
        currentPrice = price;
        itemPrefab = prefab;
    }
}
                ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                 */


        // [◆] - ▶▶▶ ApplyToAllUnits → 유닛 효과 관련된 것은 인성씨가 한다고 함.
        private void ApplyToAllUnits()
        {
            // 아군 유닛들에 효과 적용
            /*
            AllyUnit[] allyUnits = FindObjectsOfType<AllyUnit>();
            foreach (AllyUnit unit in allyUnits)
            {
                unit.UpdateStatsFromGameState();
            }

            // 적 유닛들에 효과 적용
            EnemyUnit[] enemyUnits = FindObjectsOfType<EnemyUnit>();
            foreach (EnemyUnit unit in enemyUnits)
            {
                unit.UpdateStatsFromGameState();
            }
            */
        }
        #endregion ▲▲▲▲▲ Custom Method ▲▲▲▲▲
    }
}
