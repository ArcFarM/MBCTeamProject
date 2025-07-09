using TMPro;
using UnityEngine;

// ▶ 슬롯 1개의 텍스트 제어 스크립트
public class RouletteSlot : MonoBehaviour
{
    // ▶ 슬롯 내부 텍스트 (TextMeshPro 연결)
    public TextMeshProUGUI slotLabel;

    // ▶ 텍스트 설정 함수
    public void SetText(string text)
    {
        if (slotLabel != null)
        {
            slotLabel.text = text;
        }
    }
}
