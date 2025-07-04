using System.Collections.Generic;
using UnityEngine;

namespace JiHoon
{
    public class UnitCardManager : MonoBehaviour
    {
        [Header("UnitSpawner (unitPresets 담긴 컴포넌트)")]
        public UnitSpawner spawner;       

        [Header("카드 UI 프리팹 (UnitCardUI 붙어 있어야 함)")]
        public GameObject cardUIPrefab;

        [Header("유닛 프리셋 리스트 (순서대로 icon 포함)")]
        public List<UnitPreset> unitPresets;

        [Header("카드 덱을 표시할 부모 오브젝트")]
        public Transform deckContainer;

        [Header("배치 매니저")]
        public UnitPlacementManager placementMgr;





        // 기존 카드는 유지하고, count장만 추가로 뿌립니다.
        public void AddRandomCards(int count)
        {
            
            var presets = spawner.unitPresets;
            int total = presets.Length;

            for (int i = 0; i < count; i++)
            {
                // 0 ~ total-1 사이 균등 랜덤
                int idx = Random.Range(0, total);

                // 카드 UI 생성 & 초기화
                var go = Instantiate(cardUIPrefab, deckContainer);
                var ui = go.GetComponent<UnitCardUI>();
                ui.Init(idx, presets[idx].icon, placementMgr);
            }
        }
        public void AddCardFromShopItem(ItemData item)
        {
            if (item == null || item.unitPrefab == null)
            {
                Debug.LogError("구매한 아이템에 unitPrefab이 없습니다!");
                return;
            }

            // 카드 UI 생성
            var go = Instantiate(cardUIPrefab, deckContainer);
            var ui = go.GetComponent<UnitCardUI>();

            // 아이템 정보로 카드 초기화
            ui.InitFromShopItem(item, placementMgr);
            Debug.Log($"상점에서 구매한 {item.itemName} 카드가 덱에 추가되었습니다!");
        }
    }
}
