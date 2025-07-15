using System.Collections.Generic;
using UnityEngine;

/* [0] 개요 : AAARouletteStateManager
        - 배수는 따로 스크립트 만들기 <<< 만들기
        ㄴ 인성씨 D:\Project\MBCTeamProject\Assets\Scenes\InSeong\Prefabs\Card 참고
        - 유닛 베이스에 연결해서 따로 연결하기
*/

namespace JeaYoon.Roulette
{
    [System.Serializable]
    public class RouletteEffectData
    {
        public float allyDamageMultiplier;
        public float enemyDamageMultiplier;
        public float allyAttackSpeedMultiplier;
        public float enemyAttackSpeedMultiplier;
        public float preparationTimeMultiplier;

        public bool isShopLocked;
        public bool hasRandomShopPrices;
        public bool hasGoodStart;

        public List<string> grantedPetNames = new List<string>();
    }

    public class AAARouletteStateManager : MonoBehaviour
    {
        // [1] Variable.
        #region ▼▼▼▼▼ Variable ▼▼▼▼▼
        // [◆] - ▶▶▶ AAARouletteStateManager을 싱글톤 패턴 선언.
        public static AAARouletteStateManager Instance { get; private set; }

        // [◆] - ▶▶▶ Header.
        [Header("게임 상태")]
        public float allyDamageMultiplier = 1.0f;
        public float enemyDamageMultiplier = 1.0f;
        public float allyAttackSpeedMultiplier = 1.0f;
        public float enemyAttackSpeedMultiplier = 1.0f;
        public float preparationTimeMultiplier = 1.0f;
        public bool isShopLocked = false;
        public bool hasRandomShopPrices = false;
        public List<GameObject> grantedPets = new List<GameObject>();
        public bool hasGoodStart = false;
        #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲


        // [2] Unity Event Method.
        #region ▼▼▼▼▼ Unity Event Method ▼▼▼▼▼
        // [◆] - ▶▶▶ Awake.
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
        #endregion ▲▲▲▲▲ Unity Event Method ▲▲▲▲▲


        // [3] Custom Method.
        #region ▼▼▼▼▼ Custom Method ▼▼▼▼▼
        // [◆] - ▶▶▶ ResetGameState → 게임 상태 초기화.
        public void ResetGameState()
        {
            allyDamageMultiplier = 1.0f;
            enemyDamageMultiplier = 1.0f;
            allyAttackSpeedMultiplier = 1.0f;
            enemyAttackSpeedMultiplier = 1.0f;
            preparationTimeMultiplier = 1.0f;
            isShopLocked = false;
            hasRandomShopPrices = false;
            grantedPets.Clear();
            hasGoodStart = false;
        }

        // [◆] - ▶▶▶ GetData → 현재 상태를 직렬화용 데이터로 변환.
        public RouletteEffectData GetData()
        {
            RouletteEffectData data = new RouletteEffectData();
            data.allyDamageMultiplier = allyDamageMultiplier;
            data.enemyDamageMultiplier = enemyDamageMultiplier;
            data.allyAttackSpeedMultiplier = allyAttackSpeedMultiplier;
            data.enemyAttackSpeedMultiplier = enemyAttackSpeedMultiplier;
            data.preparationTimeMultiplier = preparationTimeMultiplier;

            data.isShopLocked = isShopLocked;
            data.hasRandomShopPrices = hasRandomShopPrices;
            data.hasGoodStart = hasGoodStart;

            data.grantedPetNames = new List<string>();
            foreach (var pet in grantedPets)
            {
                if (pet != null)
                    data.grantedPetNames.Add(pet.name);
            }

            return data;
        }

        // [◆] - ▶▶▶ LoadFromData → 직렬화용 데이터로부터 상태 복원.
        public void LoadFromData(RouletteEffectData data)
        {
            allyDamageMultiplier = data.allyDamageMultiplier;
            enemyDamageMultiplier = data.enemyDamageMultiplier;
            allyAttackSpeedMultiplier = data.allyAttackSpeedMultiplier;
            enemyAttackSpeedMultiplier = data.enemyAttackSpeedMultiplier;
            preparationTimeMultiplier = data.preparationTimeMultiplier;

            isShopLocked = data.isShopLocked;
            hasRandomShopPrices = data.hasRandomShopPrices;
            hasGoodStart = data.hasGoodStart;

            // grantedPets는 런타임에서 생성되는 오브젝트이므로 이름만 저장, 초기화
            grantedPets.Clear();
        }
        #endregion ▲▲▲▲▲ Custom Method ▲▲▲▲▲
    }
}
