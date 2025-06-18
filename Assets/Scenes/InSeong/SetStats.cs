using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetStats : MonoBehaviour
{
    #region Variables
    //능력치 정확한 수치 표시
    [SerializeField] TextMeshProUGUI statValue;
    //주 슬라이더 : 현재 능력치를 시각적으로 표시, 보조 슬라이더 : 능력치의 증가/감소를 연출
    [SerializeField] Slider mainSlider;
    [SerializeField] Slider subSlider;
    //테스트용 버튼 : 5 ~ 25 사이의 값을 무작위로 오고 가는 버튼
    [SerializeField] Button plusButton;
    [SerializeField] Button minusButton;
    //값이 변화하는 효과의 시간
    [SerializeField] float animTime = 2f;
    #endregion

    #region Properties
    #endregion

    #region Unity Event Method
    private void Start() {
        //수치 표현
        statValue.text = mainSlider.value.ToString();
        //버튼에 이벤트 추가
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
        //무작위 값에 따른 슬라이더 값 변화
        if(value > 0) {
            subSlider.fillRect.GetComponent<Image>().color = new Color(128/255, 1, 155/255);
            subSlider.value = Mathf.Clamp(mainSlider.value + value, mainSlider.minValue, mainSlider.maxValue);
            StartCoroutine(SliderValueEffect(mainSlider, value, animTime));
            //값 증가 : 먼저 서브 슬라이더 증가, 메인이 천천히 증가
            
        } else {
            subSlider.fillRect.GetComponent<Image>().color = new Color(1, 90/255, 80/255);
            mainSlider.value = Mathf.Clamp(mainSlider.value + value, mainSlider.minValue, mainSlider.maxValue);
            StartCoroutine(SliderValueEffect(subSlider, value, animTime));
        }
    }

    //animTime(단위 : 초)에 걸쳐서 슬라이더 값을 val 만큼 증가/감소
    IEnumerator SliderValueEffect(Slider slider, float val, float animTime) {
        float start = slider.value;
        float target = start + val;
        float elapsed = 0f;

        while (elapsed < animTime) {
            elapsed += Time.deltaTime;
            statValue.text = ((int)slider.value).ToString();
            slider.value = Mathf.Lerp(start, target, elapsed / animTime);
            yield return null;
        }
        // 최종값 보정
        slider.value = target;
        statValue.text = mainSlider.value.ToString();
    }
    #endregion
}
