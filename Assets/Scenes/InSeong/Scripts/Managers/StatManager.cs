using UnityEngine;
using MainGame.UI;
using MainGame.Enum;
using MainGame.SystemProcess;
using MainGame.Card;
using System.Collections.Generic;
using System;
using MainGame.Units;
//능력치를 현재 수치와 동기화하여 시각 효과
namespace MainGame.Manager {
    public class StatManager : SingletonManager<StatManager> {
        #region Variables
        //정치 스탯들
        public SetStats[] statArr;
        //정치 스탯 최소치와 최대치, 게임 시작시 기본값
        [SerializeField] int[] minStat;
        [SerializeField] int[] maxStat;
        [SerializeField] int defaultStat;
        //게임오버 이벤트 관리자
        [SerializeField] GameOverEvent goe;
        //적 패널티
        [SerializeField] List<StatStruct> penalty;
        //웨이브 종료 시 패널티 팝업
        [SerializeField] GameObject penaltyPopup;
        #endregion

        #region Properties
        public List<StatStruct> GetPenalty {
            get { return penalty; }
        }
        #endregion

        #region Unity Event Method
        private void Start() {
            InitStat();
            // ) penaltyPopup.SetActive(false);
        }
        #endregion

        #region Custom Method
        //능력치 초기화
        void InitStat() {
            //각 능력치 최소/최대치 설정 후 기본값으로 설정
            for (int i = 0; i < statArr.Length; i++) {
                statArr[i].GetStatMin = minStat[i];
                statArr[i].GetStatMax = maxStat[i];
                statArr[i].GetStat = defaultStat;
                statArr[i].Initialize();
            }

        }
        //정책카드를 클릭하여 능력치 변화가 생겼을 때 그만큼 수치를 조정
        /*public void AdjustStat(PolicyCard pc) {
            int[] statDelta = pc.GetSetStatsArr;
            for(int i = 0; i < statArr.Length; i++) {
                if(statDelta[i] != 0) {
                    statArr[i].OnValueChange(statDelta[i]);
                    if (isEnd(statArr[i])) {
                        GameOver(i);
                        break;
                    }
                }
            }
        }*/

        public void AdjustStat(CardData cd) {
            int[] statDelta = cd.stats;
            Debug.Log(statDelta.Length);
            for (int i = 0; i < statArr.Length; i++) {
                Debug.Log(statDelta[i]);
                if (statDelta[i] != 0) {
                    statArr[i].OnValueChange(statDelta[i]);
                }
            }
            // 전체 작업 후 게임 오버 조건 검사
            for (int i = 0; i < statArr.Length; i++) {
                if (isEnd(statArr[i])) {
                    GameOver(i);
                    break;
                }
            }
        }

        public void AdjustStat(List<StatStruct> list) {
            foreach(var item in list) {
                //statstruct 리스트에 있는 item의 스탯에 해당하는 setstat을 statarr에서 불러와서 값 적용
                for (int i = 0; i < statArr.Length; i++) {
                    if (statArr[i].stats == item.stat) {
                        statArr[i].OnValueChange(item.value);
                        break;
                    }
                }
            }

            // 전체 작업 후 게임 오버 조건 검사
            for (int i = 0; i < statArr.Length; i++) {
                if (isEnd(statArr[i])) {
                    GameOver(i);
                    break;
                }
            }
        }
        //TODO : 유닛을 구매하여 능력치 변화가 생겼을 때 그만큼 수치를 조정

        //스탯이 0 또는 최대치에 도달하면 게임 오버
        bool isEnd(SetStats ss) {
            //단, 재정은 상한선이 없다
            if(ss.stats == Enum.Stats.Money) {
                return ss.GetStat <= ss.GetStatMin;
            }
            return ss.GetStat <= ss.GetStatMin || ss.GetStat >= ss.GetStatMax;
        }

        //능력치 관리 못해서 게임 오버
        public void GameOver(int idx) {
            goe.DoGameOver((Stats)idx);
        }

        //적이 기지에 도착했을 때 적에게서 패널티를 받아오기
        public void AddPenalty(List<StatStruct> input) {
            foreach(var item in input) {
                penalty.Add(item);
            }
        }

        public void CalcPenalty() {
            //전체 패널티 계산은 EnemyUnitBase에서 계속 했으므로 계산만 하면 됨
            //페널티가 없으면 바로 종료
            if(penalty == null || penalty.Count == 0) {
                Debug.Log("페널티 없음");
                return;
            }
            penaltyPopup.SetActive(true);
            //정산 완료된 페널티 삭제
            penalty.Clear();
        }
        #endregion
    }
}

