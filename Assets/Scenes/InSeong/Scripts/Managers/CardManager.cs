using MainGame.Enum;
using MainGame.UI;
using MainGame.Units;
using UnityEngine;

namespace MainGame.Manager {
    public class CardManager : SingletonManager<CardManager> {
        #region Variables
        [SerializeField] GameObject cardPool; //카드 정보가 담겨있는 프리팹
        #endregion

        #region Properties
        #endregion

        #region Unity Event Method
        #endregion

        #region Custom Method
        //TODO : PolicyCard에서 카드 클릭하면 카드 효과 적용하기 - 카드 효과에 따라(추가)
        public void ApplyEffect(PolicyCard pc) {
            CardEffect ce = pc.GetSetCardEffect;
            switch (ce) {
                case CardEffect.Change_Stat:
                    //StatManager를 통해 능력치 조정
                    StatManager.Instance.AdjustStat(pc);
                    break;
                case CardEffect.Change_Unit:
                    //UnitManager를 통해 손패에 유닛 변경 사항 반영
                    foreach(GameObject go in pc.GetSetUnitsArr) {
                        //대상이 아군 유닛인 경우
                        if(go.TryGetComponent<AllyUnitBase>(out AllyUnitBase aub)) {
                            UnitManager.Instance.SetUnitCard(pc.GetAddFlag, go);
                        } else {
                            //적군 유닛인 경우
                            if(go.TryGetComponent<EnemyUnitBase>(out EnemyUnitBase eub)) {
                                ChangeEnemy(pc);
                            }
                        }
                    }
                    break;
                case CardEffect.Change_Both:
                    StatManager.Instance.AdjustStat(pc);
                    foreach (GameObject go in pc.GetSetUnitsArr) {
                        //대상이 아군 유닛인 경우
                        if (go.TryGetComponent<AllyUnitBase>(out AllyUnitBase aub)) {
                            UnitManager.Instance.SetUnitCard(pc.GetAddFlag, go);
                        }
                        else {
                            //적군 유닛인 경우
                            if (go.TryGetComponent<EnemyUnitBase>(out EnemyUnitBase eub)) {
                                ChangeEnemy(pc);
                            }
                        }
                    }
                    break;
            }
        }

        void ChangeEnemy(PolicyCard pc) {
            //TODO : WaveManager 에서 맞닥뜨릴 다음 웨이브를 마주하고,
            //그 웨이브의 적군 유닛을 수정
        }

        public void GetEvent() {
            //웨이브 끝나고 고정적으로 카드 출현
            //if (WaveManager.Instance.IsWaveStarted) return;
            //TODO : 카드 풀에서 카드를 3장 생성하여 UI에 배치 - HorizontalLayoutGroup을 가진 컴포넌트에 자식으로 붙이기
        }
        #endregion
    }
}

