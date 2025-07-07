using TMPro;
using UnityEngine;

// ▶ 슬롯 하나를 제어하는 컴포넌트
public class RouletteSlot : MonoBehaviour
{
    // ▶ 슬롯 안에 표시될 텍스트 (TextMeshPro 컴포넌트 연결 필드)
    public TextMeshProUGUI slotLabel;

    // ▶ 슬롯의 텍스트를 설정하는 함수
    public void SetText(string text)
    {
        // ▶ slotLabel이 연결되어 있다면 텍스트 변경
        if (slotLabel != null)
            slotLabel.text = text;
    }
}
