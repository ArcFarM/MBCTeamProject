using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하기 위해 추가
using MainGame.Manager;

namespace MainGame.UI {
    public class PolicyCard_new : MonoBehaviour {
        #region UI 컴포넌트 참조
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image cardIconImage;
        [SerializeField] private Button acceptButton;
        #endregion

        #region 내부 데이터
        // 이 카드가 표시할 데이터 원본 (ScriptableObject)
        private CardData _cardData;
        #endregion

        #region Unity Event Method
        private void Awake() {
            // 버튼 이벤트는 한 번만 등록하는 것이 안전합니다.
            acceptButton.onClick.AddListener(OnPolicySelected);
        }
        #endregion

        #region Custom Method

        /// <summary>
        /// CardPool에서 카드를 생성한 후 호출하여 데이터를 주입하고 UI를 설정합니다.
        /// </summary>
        /// <param name="data">이 카드가 표시할 ScriptableObject 데이터</param>
        public void Initialize(CardData data) {
            _cardData = data;
            UpdateUI();
        }

        /// <summary>
        /// _cardData에 저장된 정보를 바탕으로 UI 요소들을 업데이트합니다.
        /// </summary>
        private void UpdateUI() {
            if (_cardData == null) {
                Debug.LogError("CardData가 없습니다! Initialize가 호출되었는지 확인하세요.");
                return;
            }

            // ScriptableObject의 데이터로 UI 텍스트와 이미지를 채웁니다.
            nameText.text = _cardData.cardName;
            descriptionText.text = _cardData.description;
            cardIconImage.sprite = _cardData.cardIcon;

            // (선택 사항) 카드 등급에 따라 배경색이나 테두리 등을 바꿀 수 있습니다.
            // SetCardAppearanceByGrade(_cardData.cardGrade);
        }

        /// <summary>
        /// 수락 버튼이 클릭되었을 때 호출됩니다.
        /// </summary>
        void OnPolicySelected() {
            if (_cardData == null) return;

            // CardManager에 MonoBehaviour(this) 대신 순수 데이터 객체(_cardData)를 전달합니다.
            //CardManager.Instance.ApplyEffect(_carddata);

            // 카드를 사용한 후, 이 오브젝트를 파괴하거나 풀(Pool)로 반환합니다.
            // 예: Destroy(gameObject);
        }
        #endregion
    }
}