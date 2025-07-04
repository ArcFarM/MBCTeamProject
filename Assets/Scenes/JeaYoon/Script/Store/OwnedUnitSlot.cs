using UnityEngine;
using UnityEngine.UI;       // ) Image 활성화.
using System.Collections;       // ) IEnumerator 활성화.
using JiHoon;

/* [0] 개요 : OwnedUnitSlot
		- 
*/

namespace JeaYoon
{
	public class OwnedUnitSlot : MonoBehaviour
	{

        // [1] Variable.
        #region ▼▼▼▼▼ Variable ▼▼▼▼▼
        // [◆] - ▶▶▶ 정의.
        public Image unitIcon;
        private UnitData unitData;
        private Sprite illustrationSprite;      // ) 2D 이미지를 표현하기 위하여 PNG나 JPG를 자동적으로 Sprite로 바꿔줌.
        #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲





        // [2] Custom Method.
        #region ▼▼▼▼▼ Custom Method ▼▼▼▼▼
        // [◆] - ▶▶▶ IsIccupied.
        public bool IsOccupied
        {
            get
            {
                return unitData != null;
            }
        }


        // [◆] - ▶▶▶ SetUnit.
        public void SetUnit(UnitData newUnit)
        {
            unitData = newUnit;                      // ) 유닛 정보 설정.
            unitIcon.sprite = newUnit.icon;         // ) 유닛 아이콘 설정.
            unitIcon.enabled = true;
        }


        // [◆] - ▶▶▶ ClearUnit.
        public void ClearUnit()
        {
            unitData = null;
            unitIcon.sprite = null;
            unitIcon.enabled = false;
        }


        // [◆] - ▶▶▶ SetIllustration → 아이템 일러스트를 보여주는 기능 추가.
        public void SetIllustration(Sprite illustration)
        {
            if (illustration == null)
            {
                Debug.LogWarning("일러스트가 null입니다!");
                return;
            }

            // [◇] - [◆] - ) .
            illustrationSprite = illustration;
            unitIcon.sprite = illustration;
            unitIcon.color = Color.white;
            unitIcon.enabled = true;
            // [◇] - [◆] - ) Coroutine 시작 → PlayPopAnimation.
            StartCoroutine(PlayPopAnimation());
        }


        // [◆] - ▶▶▶ PlayPopAnimation → 하단 슬롯에 유닛이 추가되었을 때 팝업효과.
        private IEnumerator PlayPopAnimation()
        {
            // [◇] - [◆] - ) .
            float duration = 0.2f;
            Vector3 originalScale = transform.localScale;
            Vector3 enlargedScale = originalScale * 1.2f;
            float time = 0f;
            // [◇] - [◆] - ) 커졌다가.
            while (time < duration)
            {
                transform.localScale = Vector3.Lerp(originalScale, enlargedScale, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            // [◇] - [◆] - ) 원래 크기로 복귀.
            time = 0f;
            while (time < duration)
            {
                transform.localScale = Vector3.Lerp(enlargedScale, originalScale, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            transform.localScale = originalScale;
        }
        #endregion ▲▲▲▲▲ Custom Method ▲▲▲▲▲
    }
}
