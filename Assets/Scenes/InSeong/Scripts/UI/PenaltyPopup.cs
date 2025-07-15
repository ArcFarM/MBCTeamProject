using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using MainGame.Units;
using MainGame.Manager;
using UnityEngine.UI;

namespace MainGame.UI {

    public class PenaltyPopup : MonoBehaviour {
        #region Variables
        //패널티 팝업창에 띄울 텍스트들 - 한 줄에 하나씩-
        [SerializeField] TextMeshProUGUI[] penaltyPopup;
        //사용할 패널티 리스트
        List<StatStruct> penalty;
        // 패널티 팝업창을 닫는 버튼
        [SerializeField] UnityEngine.UI.Button closeButton;
        bool canClick = true;
        //코루틴 감지 플래그
        int isCoroutineRunning = 0;
        //최종 적용할 패널티
        public List<StatStruct> finalPenalty;
        
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        private void OnEnable() {
            //패널티 계산
            penalty = StatManager.Instance.GetPenalty;
            finalPenalty = RewardPenalty.CalcPenalty(penalty);
            isCoroutineRunning = finalPenalty.Count;
            SetString();
            closeButton.onClick.AddListener(OnButtonClicked);
            //결과 완료되기 전까진 버튼 비활성화
            closeButton.interactable = false;
            //클릭 이벤트 추가
            if(TryGetComponent<Button>(out Button button)) {
                button.onClick.AddListener(OnClickEvent);
            }
        }

        private void Update() {
            if(isCoroutineRunning <= 0) {
                //모든 코루틴이 완료되면 버튼 활성화
                closeButton.interactable = true;
            }
        }
        #endregion

        #region Custom Methods
        //TODO : 일단 빨리 해야해서 이렇게 했지만, 더 좋은 구조로 바꿔야 함
        void SetString() {
            penaltyPopup[0].text += penalty.Count.ToString();
            for(int i = 2; i - 2 < finalPenalty.Count; i++) {
                penaltyPopup[i].text = $"{finalPenalty[i - 2].stat} :";
                StartCoroutine(ValueElevate(i, finalPenalty[i - 2].value));
            }
        }

        //적절한 애니메이션을 통해 값이 상승/하강하는 연출
        IEnumerator ValueElevate(int index, int value) {
            if(value == 0) {
                penaltyPopup[index].text += " 0";
                isCoroutineRunning--;
                yield break; //값이 0인 경우 바로 종료
            }
            float animTime = 2f;
            float elapsedTime = 0f;
            int startValue = 0;
            float deltaValue = 0;

            string originalText = penaltyPopup[index].text;

            //animTime 동안 value로 값이 변화
            while (elapsedTime < animTime) {
                elapsedTime += Time.deltaTime;
                deltaValue = Mathf.Lerp(startValue, value, elapsedTime / animTime);
                string tmp = originalText + " " + (int)deltaValue;
                penaltyPopup[index].text = tmp;
                yield return null;
            }
            isCoroutineRunning--;
        }

        void OnClickEvent() {
            if (!canClick) return;
            canClick = false; //버튼 추가 클릭 방지 << interactable false를 안쓰는 이유 : 얘는 버튼처럼 보이면 안돼서 시각적 변화를 주지 않기 위함
            //코루틴 진행중일 경우 중단시키고 강제로 텍스트를 최종값으로 설정 (애니메이션 강제 완료)
            if (isCoroutineRunning > 0) {
                StopAllCoroutines();
                for (int i = 2; i < penaltyPopup.Length; i++) {
                    penaltyPopup[i].text = $"{penalty[i - 2].stat} : {penalty[i - 2].value}";
                }
            }

            closeButton.interactable = true; //패널티 적용 후 버튼 활성화
        }

        void OnButtonClicked() {
            closeButton.interactable = false; //버튼 비활성화 
            StatManager.Instance.AdjustStat(finalPenalty);
            //창 닫기
            gameObject.SetActive(false);
        }
        #endregion
    }
}