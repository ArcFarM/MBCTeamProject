using UnityEngine;

namespace MainGame.Enum {
    //정책 카드의 효과 : 능력치 변경, 유닛 변경, 둘 다 변경
    public enum CardEffect {
        Change_Stat = 0,
        Change_Unit,
        Change_Both,
    }

    //정책 카드의 등급 : 위기/하급/중급/상급
    public enum CardGrade {
        Crisis = 0,
        NotBad,
        Good,
        Awesome,
    }
}
