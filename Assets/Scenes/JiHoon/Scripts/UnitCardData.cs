using UnityEngine;

namespace JiHoon
{
    [CreateAssetMenu(fileName = "UnitCardData_", menuName = "Game/Unit Card Data", order = 1)]
    public class UnitCardData : ScriptableObject
    {
        [Header("유닛 정보")]
        public GameObject unitPrefab;      // 유닛 프리팹

        [Header("카드 표시")]
        public Sprite unitIcon;            // 카드에 표시되는 아이콘
        public Sprite hoverIcon;           // 마우스 오버 시 카드가 바뀌는 이미지

        [Header("툴팁 정보")]
        public Sprite tooltipImage;        // 툴팁에 표시될 이미지
        [TextArea(5, 10)]
        public string tooltipText;         // 툴팁에 표시될 텍스트 (유닛 설명)
    }
}