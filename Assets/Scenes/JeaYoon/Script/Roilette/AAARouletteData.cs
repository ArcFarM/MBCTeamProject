using System.Collections.Generic;

[System.Serializable]
public class RouletteEffect
{
    public string name;      // 슬롯에 보일 텍스트
    public string description;  // 설명창에 나올 내용
}

public static class AAARouletteData
{
    public static List<RouletteEffect> Effects = new List<RouletteEffect>
    {
        new RouletteEffect { name = "빠르고 강하게", description = "아군의 데미지가 2배가 되지만 적의 공격력은 1.5배, 준비시간이 절반으로 줄어듭니다." },
        new RouletteEffect { name = "느리고 약하게", description = "아군과 적의 공격속도가 0.5배, 준비시간이 두배로 늘어납니다." },
        new RouletteEffect { name = "악덕 상점", description = "상점의 가격이 랜덤하게 배정됩니다. 가끔 상점 시스템이 잠깁니다." },
        new RouletteEffect { name = "그냥 귀엽잖아요", description = "지정된 펫 하나가 지급됩니다." },
        new RouletteEffect { name = "좋은 출발", description = "첫 상점에서 2단계 이상의 포탑이 2개 나타납니다." }
    };
}
