using System.Collections.Generic;

/* [0] 개요 : RouletteEffectData
		- 직렬화.
*/

[System.Serializable]
    public class RouletteEffectData
{        // [1] Variable.
    #region ▼▼▼▼▼ Variable ▼▼▼▼▼
    // [◆] - ▶▶▶ .
    public float allyDamageMultiplier;
    public float enemyDamageMultiplier;
    public float allyAttackSpeedMultiplier;
    public float enemyAttackSpeedMultiplier;
    public float preparationTimeMultiplier;

    public bool hasRandomShopPrices;
    public bool isShopLocked;
    public bool hasGoodStart;


    // [◆] - ▶▶▶ .
    // 펫 이름 리스트 (직렬화에 GameObject 직접 넣기 어렵기 때문)
    public List<string> grantedPetNames = new List<string>();
    #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲
}