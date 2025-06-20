using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MainGame.UI {
    public class ShowStatInfo : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler {
        #region Variables
        //능력치 설명문
        [SerializeField] GameObject statInfo;
        //능력치 설명문을 instantiate하고 파괴할 임시 오브젝트
        GameObject tmp;
        RectTransform tmpRect;
        //설명문을 표시할 위치
        [SerializeField] Vector3 Offset; //주의 : 반드시 설명문 크기보다 크게 잡아야 함
        #endregion

        #region Properties
        #endregion

        #region Unity Event Method
        private void Start() {
            tmp = null;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            //Debug.Log("마우스 들어옴 (UI)");
            if (tmp == null) {
                // 캔버스 하위로 설명문 생성
                Canvas parentCanvas = GetComponentInParent<Canvas>();
                tmp = Instantiate(statInfo, parentCanvas.transform);
                tmpRect = tmp.GetComponent<RectTransform>();
                if (tmpRect != null) tmpRect.position = Input.mousePosition + Offset;
            }
        }

        public void OnPointerMove(PointerEventData eventData) {
            if(tmp != null && tmpRect != null) {
                tmpRect.position = Input.mousePosition + Offset;
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            //Debug.Log("마우스 나감 (UI)");
            if (tmp != null) {
                Destroy(tmp);
                tmp = null;
            }
        }
        #endregion

        #region Custom Method
        #endregion
    }

}

