using UnityEngine;

namespace MainGame.Enum {
    //정책 카드의 효과 : 능력치 변경, 유닛 변경, 둘 다 변경, 상점에서 판매하는 유닛 풀에
    //유닛 추가/제거
    public enum CardEffect {
        Change_Stat = 0,
        Change_Unit,
        Change_Both,
        Add_Unit, //상점에 유닛 추가
        Remove_Unit, //상점에서 유닛 제거
    }

    //정책 카드의 등급 : 위기/하급/중급/상급
    public enum CardGrade {
        Crisis = 0,
        NotBad,
        Good,
        Awesome,
    }
}
