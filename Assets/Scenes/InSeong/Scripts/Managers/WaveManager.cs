using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace MainGame.Manager {
    public class WaveManager : SingletonManager<WaveManager> {
        #region Variables
        //TODO : 미리 만들어진 웨이브 프리팹 - 배열을 담고 있는 게임 오브젝트들의 배열
        [SerializeField] GameObject[] wavePrefabs;
        List<GameObject> currWaveData;
        //현재 웨이브 카운터
        [SerializeField] int waveCounter = 0;
        //웨이브 대기 시간
        [SerializeField] float waveWaitTime = 60f;
        //적 생성 타이머
        [SerializeField] float enemySpawnTimer = 0.5f;
        //웨이브 시작 여부
        bool isWaveStarted = false;
        //생성된 적 이동 경로 - TODO
        //[SerializeField] Waypoints[] wp;
        #endregion

        #region Properties
        public bool IsWaveStarted {
            get { return isWaveStarted; }
        }
        #endregion

        #region Unity Event Methods
        #endregion

        #region Custom Methods
        //웨이브 카운터에 맞춰서 프리팹을 불러와서 이번 웨이브로 할당
        public void GetCurrWave() {
            //리스트 초기화
            currWaveData = new List<GameObject>();
            //이번 웨이브 데이터 가져오기
            GameObject currWaveParent = wavePrefabs[waveCounter];
            if(currWaveParent != null) {
                //TODO : 웨이브 프리팹을 불러와서 WaveManager에 할당
                SetCurrData(currWaveParent);
            }
            else {
                Debug.LogError("Current wave prefab is null!");
            }
        }
        void SetCurrData(GameObject waveData) {
            //null check는 이미 GetCurrWave()에서 수행
            for (int i = 0; i < waveData.transform.childCount; i++) {
                //자식 오브젝트를 가져와서 현재 웨이브 데이터에 할당
                currWaveData[i] = waveData.transform.GetChild(i).gameObject;
            }
        }
        //웨이브 시작 시 타이머를 통해 웨이브를 생성
        public IEnumerator StartWave() {
            isWaveStarted = true;
            if (waveCounter >= 0) {
                GetCurrWave();
            }
            for (int i = 0; i < currWaveData.Count; i++) {
                //웨이브 데이터가 null이 아닐 때
                if (currWaveData[i] != null) {
                    //웨이브 데이터의 자식 오브젝트를 활성화
                    Instantiate(currWaveData[i], transform.position, Quaternion.identity);
                    //적 생성 타이머를 기다림
                    yield return new WaitForSeconds(enemySpawnTimer);
                }
                else {
                    Debug.LogWarning("Current wave data is null at index: " + i);
                }
            }

        }
        //TODO : 웨이브가 끝나면 결과에 따라 스탯을 적용하고 CardManager와 StoreManager에게
        //내장된 메서드를 호출하여 카드 발생 이벤트를 만들고, 상점을 갱신함
        //이후 대기 시간을 부여하고 다음 웨이브 까지 대기
        public void EndWave() {
            //웨이브 종료 및 카운터 상승으로 다음 웨이브 준비하기
            isWaveStarted = false;
            waveCounter++;

            //TODO : 결과에 따라 스탯 적용 - 놓친 적에 따라 패널티 적용
            /*
             개별 적 유닛은 패널티 스탯을 지니고 있음 - 놓친 적 유닛 정보를 놓친 적 리스트에 담고,
             그걸 읽어서 스탯을 적용
             */

            //TODO : CardManager를 통해 카드 발생 이벤트 호출

            //TODO : StoreManager를 통해 상점 갱신 이벤트 호출
            /*
             카드 이벤트가 끝나고 사용 가능한 유닛 리스트에 변화가 있다면 그걸 적용하고 새로고침,
            아니면 그냥 기존 유닛 풀에서 새로고침해서 유닛들을 상점에 무작위 배치
             */

            //대기 시간 동안 대기
            StartCoroutine(EndWave_Wait());
        }
        IEnumerator EndWave_Wait() {
            yield return new WaitForSeconds(waveWaitTime);
            //웨이브 종료 후 다음 웨이브 시작
            StartCoroutine(StartWave());
        }
        #endregion
    }

}
