using System.Collections.Generic;
using UnityEngine;
using JiHoon;

/* [0] 개요 : StoreSystemManager
*/

namespace JeaYoon
{
    public class StoreSystemManager : MonoBehaviour
    {
        // [1] Variable.
        #region ▼▼▼▼▼ Variable ▼▼▼▼▼
        // [◆] - ▶▶▶ 정의.
        [Header("유닛 슬롯들 (3x3 = 9)")]
        public List<UnitSlot> unitSlots;

        [Header("상점에 등장 가능한 유닛 목록")]
        public List<UnitData> allAvailableUnits;

        [Header("한번에 등장할 유닛의 수")]
        public int unitsPerRefresh = 9;
        #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲





        // [2] Unity Event Method.
        #region ▼▼▼▼▼ Unity Event Method ▼▼▼▼▼
        // [◆] - ▶▶▶ 상점을 열었을 때 초기화.
        private void Start()
        {
            RefreshShopUnits();
        }


        // [◆] - ▶▶▶ RefreshShopUnits → 유닛목록을 초기화 하고 다시 무작위로 배치.
        public void RefreshShopUnits()
        {
            // [◇] - [◆] - ) 현재 슬롯 비우기.
            foreach (var slot in unitSlots)
            {
                slot.ClearSlot();
            }
            // [◇] - [◆] - ) 무작위 유닛 선택.
            List<UnitData> selectedUnits = GetRandomUnits(unitsPerRefresh);
            // [◇] - [◆] - ) 무작위로 섞인 슬롯에 유닛을 배치.
            List<int> slotIndices = new List<int>();
            for (int i = 0; i < unitSlots.Count; i++) slotIndices.Add(i);
            ShuffleList(slotIndices);
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                unitSlots[slotIndices[i]].SetUnit(selectedUnits[i]);
            }
        }


        // [◆] - ▶▶▶ GetRandomUnits → 무작위 유닛 추출.
        List<UnitData> GetRandomUnits(int count)
        {
            // [◇] - [◆] - ) 무작위로 추출된 유닛을 담기위한 리스트.
            List<UnitData> result = new List<UnitData>();
            // [◇] - [◆] - ) 상점에 등장 가능한 유닛 목록을 복사해 새로운 리스트 생성.
            List<UnitData> temp = new List<UnitData>(allAvailableUnits);
            ShuffleList(temp);
            for (int i = 0; i < Mathf.Min(count, temp.Count); i++)
            {
                result.Add(temp[i]);
            }
            return result;
        }


        // [◆] - ▶▶▶ ShuffleList → 리스트 섞기.
        void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int rand = Random.Range(0, i + 1);
                (list[i], list[rand]) = (list[rand], list[i]);
            }
        }


        // [◆] - ▶▶▶ OnWaveEnded → 웨이브 종료 후 호출.
        public void OnWaveEnded()
        {
            RefreshShopUnits();
        }
        #endregion ▲▲▲▲▲ Unity Event Method ▲▲▲▲▲
    }
}






/*
 상점 시스템의 스크립트를 만들고 싶어.
필요조건
 - 상점에서 유닛구매 창에서 유닛을 구매할 수 있다. 유닛구매 창에서 유닛의 배치는 3X3이며 무작위로 배치가 된다.
 - 웨이브가 끝나면 상점의 유닛구매 창 목록이 초기화가 되며 새로운 유닛이 추가가 된다.
 - 새로운 유닛이 추가가 되면서 유닛구매 창의 유닛이 무작위로 배치가된다.

라는 조건을 가진 스크립트를 만들어줘.

dasdas

아이템? 상점?

- 상점시스템
- 상점 UI 눌렀을 때 정치스텟 UI랑 결부시켜서 아이템 가격+스텟연동
- 웨이브가 끝났을 때 상점 물품이 

1. 상점에는 '현재 해금된 유닛들' 이 무작위로 배치되어 있다 (가급적이면 중복을 피한다)
	ㄴ 상점에서 해금된 유닛은 총 몇개? → 3X3 ~ 3X4
2. 웨이브가 끝나면 상점 목록이 초기화되고, '현재 해금된 유닛들'에 변화가 필요한 지 판단한다
    ㄴ 상점 목록이 초기화되는건 웨이브가 끝났을 때만 적용
3. 변화가 필요하다면 그것을 적용하고, 현재 해금된 유닛들 중에서 무작위로 다시 배치한다
4. (추가) 진행 상황에 따라서 유닛의 비율을 조정한다
+ 챗gpt한테 아이디어 물어보기

01234526789
100 0 0 0 0 0 0 0 0 0 0 0
90 10 0 0 0 0 0 0 0 0 0 0


1345153152

 */