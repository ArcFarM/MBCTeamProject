using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하기 위해 추가
using MainGame.Manager;
using MainGame.Card;

namespace MainGame.UI {
    public class PolicyCard_new : MonoBehaviour {
        #region Variables
        
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI nameText; //카드 이름
        [SerializeField] private TextMeshProUGUI descriptionText; //카드 설명
        [SerializeField] private Image cardIconImage; //카드 이미지
        [SerializeField] private Button acceptButton; //선택 버튼
        [SerializeField] private Image cardFrame; //카드 틀
        
        // 이 카드가 표시할 데이터 원본 (ScriptableObject)
        private CardData _cardData;
        #endregion

        #region Properties
        public CardData GetCardData {
            get { return _cardData; }
            private set { _cardData = value; }
        }
        #endregion

        #region Unity Event Method
        private void Awake() {
            acceptButton.onClick.AddListener(OnPolicySelected);
        }
        #endregion

        #region Custom Method
        public void Initialize(CardData data) {
            _cardData = data;
            UpdateUI();
        }

        private void UpdateUI() {
            if (_cardData == null) {
                Debug.LogError("CardData가 없습니다! Initialize가 호출되었는지 확인하세요.");
                return;
            }

            // ScriptableObject의 데이터로 UI 채우기
            nameText.text = _cardData.cardName;
            descriptionText.text = _cardData.description;
            cardIconImage.sprite = _cardData.cardIcon;
            cardFrame.sprite = _cardData.cardFrame;
        }

        //버튼을 눌러서 카드를 골랐을 때
        void OnPolicySelected() {
            if (_cardData == null) return;

            // CardManager에 MonoBehaviour(this) 대신 순수 데이터 객체(_cardData)를 전달합니다.
            CardManager.Instance.ApplyEffect(_cardData);

            //전달 후 이 객체 비활성화
            gameObject.SetActive(false);
        }
        #endregion
    }
}