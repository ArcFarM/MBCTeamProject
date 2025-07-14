using MainGame.Enum;
using MainGame.UI;
using MainGame.Units;
using MainGame.Card;
using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace MainGame.Manager {
    public class CardManager : SingletonManager<CardManager> {
        #region Variables
        Dictionary<CardEffect, Action<CardData>> effectProcess;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Method
        protected override void Awake() {
            base.Awake();
            Initialize();
        }
        #endregion

        #region Custom Method

        private void Initialize() {
            effectProcess = new Dictionary<CardEffect, Action<CardData>>() {
                { CardEffect.Change_Stat, ChangeStat},
                { CardEffect.Change_Unit, ChangeUnit},
                { CardEffect.Change_Both, ChangeBoth}
          };
        }

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
                                //ChangeEnemy(pc);
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
                                //ChangeEnemy(pc);
                            }
                        }
                    }
                    break;
            }
        }

        public void ApplyEffect(CardData cd) {
            //null check
            if(cd == null) {
                Debug.LogWarning("CardData is null.");
                return;
            }

            //카드 데이터를 받아서 로직 적용
            CardEffect ce = cd.cardEffect;
            //값을 구할 수 있으면 알아서 value 값으로 실행
            if(effectProcess.TryGetValue(ce, out var method)) {
                method(cd);
            }
        }

        /*void ChangeEnemy(PolicyCard pc) {
            //TODO : WaveManager 에서 맞닥뜨릴 다음 웨이브를 마주하고,
            //그 웨이브의 적군 유닛을 수정
        }*/

        void ChangeStat(CardData data) {
            StatManager.Instance.AdjustStat(data);
        }

        void ChangeUnit(CardData data) {
            //상점 목록에서 유닛을 추가로 해금하거나,
            //손패에서 카드를 추가/제거
        }

        void ChangeBoth(CardData data) {
            ChangeStat(data);
            ChangeUnit(data);
        }
        #endregion
    }
}

