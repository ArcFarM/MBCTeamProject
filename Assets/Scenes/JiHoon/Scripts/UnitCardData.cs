using System.Collections.Generic;
using UnityEngine;

namespace JiHoon
{
    [CreateAssetMenu(fileName = "UnitCardData_", menuName = "Game/Unit Card Data")]
    public class UnitCardData : ScriptableObject
    {
        [Header("유닛 정보")]
        public GameObject unitPrefab;

        [Header("카드 표시")]
        public Sprite unitIcon;
        public Sprite hoverIcon;

        [Header("툴팁 정보")]
        public Sprite tooltipImage;
        [TextArea(5, 10)]
        public string tooltipText;

        [Header("특수능력")]
        public List<AbilityData> specialAbilities = new List<AbilityData>();  // 특수능력 리스트

        [Header("툴팁 텍스트 색상")]
        public Color tooltipTextColor = Color.white;

    }
}