using UnityEngine;
using System.Collections.Generic;
using MainGame.Enum;

namespace MainGame.Card {
    [CreateAssetMenu(fileName = "New CardGradeData", menuName = "Card/Card Grade Data")]
    public class CardGradeData : ScriptableObject {
        public CardGrade grade;
        //확률 가중치 : 확률 -> 내 가중치/전체 가중치 합
        public float probabilityWeight;

        // 해당 등급에 속하는 모든 CardData 에셋들의 리스트
        public List<CardData> cards;
    }
}
