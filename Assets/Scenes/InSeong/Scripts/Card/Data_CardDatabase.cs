using UnityEngine;
using System.Collections.Generic;

namespace MainGame.Card {
    [CreateAssetMenu(fileName = "CardDatabase", menuName = "Card/Database")]
    public class CardDatabase : ScriptableObject {
        [Header("일반 카드 등급 설정")]
        public List<CardGradeData> gradePools;

        [Header("위기 카드 설정")]
        public CardGradeData crisisPool;
    }
}
