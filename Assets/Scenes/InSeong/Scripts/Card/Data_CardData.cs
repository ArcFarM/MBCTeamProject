using UnityEngine;
using MainGame.Enum;

namespace MainGame.Card {
    [CreateAssetMenu(fileName = "New CardData", menuName = "Card/Card Data")]
    public class CardData : ScriptableObject {
        [Header("기본 정보")]
        public string cardName; //카드 이름
        [TextArea(3, 10)]
        public string description; //카드 설명
        public GameObject cardPrefab; // 이 데이터를 사용할 프리팹

        [Header("스프라이트")]
        public Sprite cardIcon; // 카드 이미지 (스프라이트)
        public Sprite cardFrame; //카드 틀

        [Header("게임 로직 데이터")]
        public CardGrade cardGrade;
        public CardEffect cardEffect;
        public int[] stats;
        // public GameObject[] units; // 유닛 프리팹을 참조해야 한다면 유지, 아니면 다른 방식으로 처리
        public bool isAdd; // 효과가 더하기인지 빼기인지 등
    }
}
