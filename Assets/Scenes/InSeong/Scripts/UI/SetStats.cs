using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MainGame.UI {
    public class SetStats : MonoBehaviour {
        #region Variables
        //능력치 정확한 수치 표시
        [SerializeField] TextMeshProUGUI statValue;
        [SerializeField] int currStat = 50;
        int currStatMin = 0, currStatMax = 100; //TODO : 최소치, 최대치도 능력치 관리 스크립트에서 가져올 것
        //주 이미지 : 현재 능력치를 시각적으로 표시, 보조 이미지 : 능력치의 증가/감소를 연출
        [SerializeField] Image mainImage;
        [SerializeField] Image subImage;
        //테스트용 버튼 : 5 ~ 25 사이의 값을 무작위로 오고 가는 버튼
        [SerializeField] Button plusButton;
        [SerializeField] Button minusButton;
        //값이 변화하는 효과의 시간
        [SerializeField] float animTime = 2f;
        #endregion

        #region Properties
        public int GetStat {
            get { return currStat; }
            set { currStat = value; }
        }
        public int GetStatMin {
            get { return currStatMin; }
            set { currStatMin = value; }
        }
        public int GetStatMax {
            get { return currStatMax; }
            set { currStatMin = value; }
        }
        #endregion

        #region Unity Event Method
        private void Start() {
            //수치 표현
            statValue.text = currStat.ToString() + " / " + currStatMax.ToString();
            //버튼에 이벤트 추가 - TODO : 범위와 값은 별도 스크립트에 저장된 값에서 받아와야 함
            plusButton.onClick.AddListener(() => OnClick(5, 25));
            minusButton.onClick.AddListener(() => OnClick(-25, -5));
        }
        #endregion

        #region Custom Method
        //무작위 정수 min ~ max 범위 사이로 반환
        public void OnClick(int min, int max) {
            int rnd = Random.Range(min, max + 1);
            OnValueChange(rnd);
        }

        public void OnValueChange(float value) {
            if (value > 0) {
                //값 증가 : 서브가 먼저 움직여서 녹색 영역을 만들고, 메인 영역이 따라감
                subImage.color = SetColor(128, 255, 0);
                subImage.fillAmount = Mathf.Clamp(currStat + value, 0, 100) / currStatMax;
                StartCoroutine(SliderValueEffect(mainImage, value, animTime));
            }
            else {
                //값 감소 : 메인이 먼저 움직여서 빨간 영역을 만들고, 서브 영역이 따라감
                subImage.color = SetColor(255, 80, 90);
                mainImage.fillAmount = Mathf.Clamp(currStat + value, 0, 100) / currStatMax;
                StartCoroutine(SliderValueEffect(subImage, value, animTime));
            }
           
        }

        //animTime(단위 : 초)에 걸쳐서 이미지의 fillAmount 값을 val 만큼 증가/감소
        IEnumerator SliderValueEffect(Image image, float val, float animTime) {
            float start = currStat;
            float target = currStat + val;
            //타이머 경과 시간
            float elapsed = 0f;

            while (elapsed < animTime) {
                elapsed += Time.deltaTime;
                //fillAmount 값은 0 ~ 1 이므로 비율만큼 나눠줌
                image.fillAmount = Mathf.Lerp(start, target, elapsed/animTime) / currStatMax;
                statValue.text = ((int)(image.fillAmount * currStatMax)).ToString() + " / " + currStatMax.ToString();
                yield return null;
            }
            // 최종값 보정
            elapsed = animTime;
            image.fillAmount = target / currStatMax;
            currStat = (int)target;
        }

        //Inspector에 맞춰서 0 ~ 255 값으로 색 생성
        Color SetColor(float red, float blue, float green) {
            return new Color(red / 255, blue / 255, green / 255);
        }
        Color SetColor(float red, float blue, float green, float alpha) {
            return new Color(red / 255, blue / 255, green / 255, alpha / 255);
        }
        #endregion
    }
}

