using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace JiHoon
{
    public class WaveController : MonoBehaviour
    {
        [Header("적 스포너들 (EnemySpawnerManager)")]
        public List<EnemySpawnerManager> spawners;

        [Header("카드 매니저")]
        public UnitCardManager cardManager;

        [Header("웨이브 시작 버튼 UI")]
        public Button startWaveButton;

        [Header("첫 시작 시 지급할 카드 수")]
        public int initialCardCount = 5;

        [Header("웨이브당 카드 수")]
        public int cardsPerWave = 3;

        [Header("자동 재시작 대기 시간")]
        public float autoDelay = 50f;

        // 내부 용도
        private int _finishedCount;
        private Coroutine _autoRoutine;
        private bool _hasStarted = false;

        void Start()
        {
            // 1) 씬 시작 시점에 미리 카드 초기 지급
            cardManager.AddRandomCards(initialCardCount);

            // 2) Start 버튼 눌렀을 때 웨이브만 시작하도록 세팅
            startWaveButton.onClick.AddListener(OnStartClicked);

            // 3) 웨이브 완료 이벤트 구독
            foreach (var sp in spawners)
                sp.OnWaveFinished += OnOneSpawnerFinished;
        }

        void OnStartClicked()
        {
            // 버튼 비활성화
            startWaveButton.interactable = false;
            _finishedCount = 0;

            // 자동 재시작 예약이 있으면 취소
            if (_autoRoutine != null)
            {
                StopCoroutine(_autoRoutine);
                _autoRoutine = null;
            }

            // 실제 웨이브 시작
            foreach (var sp in spawners)
                sp.StartWave();
        }

        // 각 스포너가 웨이브를 끝낼 때마다 호출됩니다.
        void OnOneSpawnerFinished()
        {
            _finishedCount++;
            // 모든 스포너가 끝났다면
            if (_finishedCount >= spawners.Count)
                HandleWaveFinished();
        }

        // 진짜 웨이브 전체 완료 처리
        void HandleWaveFinished()
        {
            // 1) 카드 생성
            cardManager.AddRandomCards(cardsPerWave);

            // 2) 버튼 다시 활성화
            startWaveButton.interactable = true;

            // 3) 자동 재시작 예약
            _autoRoutine = StartCoroutine(AutoStartNext());
        }

        IEnumerator AutoStartNext()
        {
            yield return new WaitForSeconds(autoDelay);
            _autoRoutine = null;
            OnStartClicked();
        }

        void OnDestroy()
        {
            // 이벤트 구독 해제
            foreach (var sp in spawners)
                sp.OnWaveFinished -= OnOneSpawnerFinished;
        }
    }
}