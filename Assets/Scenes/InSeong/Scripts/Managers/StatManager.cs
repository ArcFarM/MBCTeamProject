using UnityEngine;
using MainGame.UI;
using MainGame.Enum;
using MainGame.SystemProcess;
using MainGame.Card;
using Unity.VisualScripting;
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
        #endregion

        #region Properties
        #endregion

        #region Unity Event Method
        private void Start() {
            InitStat();
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
        public void AdjustStat(PolicyCard pc) {
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
        }

        public void AdjustStat(CardData cd) {
            int[] statDelta = cd.stats;
            Debug.Log(statDelta.Length);
            for (int i = 0; i < statArr.Length; i++) {
                Debug.Log(statDelta[i]);
                if (statDelta[i] != 0) {
                    statArr[i].OnValueChange(statDelta[i]);
                    if (isEnd(statArr[i])) {
                        GameOver(i);
                        break;
                    }
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
        #endregion
    }
}

