namespace JeaYoon.Roulette
{
    public static class RouletteData
    {
        public static readonly string[] SlotTexts = new string[]
        {
            "빠르고 강하게",
            "느리고 약하게",
            "악덕 상점",
            "그냥 귀엽잖아요",
            "좋은출발"
        };

        public static readonly string[] EffectDescriptions = new string[]
        {
            "아군의 데미지가 2배가 되지만 적의 공격력이 1.5배, 준비시간이 절반으로 줄어듭니다.",
            "아군과 적의 공격속도가 0.5배, 준비시간이 두배로 늘어납니다.",
            "상점의 가격이 랜덤하게 배정됩니다. 가끔 상점 시스템이 잠깁니다.",
            "지정된 펫 하나가 지급됩니다.",
            "첫 상점에서 2단계 이상의 포탑이 2개 나타납니다."
        };

        public static readonly string[] ItemIDs = new string[]
        {
            "HD_001",
            "HD_002",
            "HD_006",  // 악덕 상점
            "HD_007",  // 그냥 귀엽잖아요
            "HD_012"   // 좋은출발
        };
    }
}