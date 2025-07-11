using UnityEngine;
using TMPro;
using JiHoon;

public class WaveUIUpdater : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI waveText;  // Wave UI 텍스트

    void OnEnable()
    {
        // 이벤트 구독
        WaveController.OnWaveChanged += UpdateWaveUI;
        WaveController.OnWaveStarted += OnWaveStart;
        WaveController.OnWaveCompleted += OnWaveComplete;

        // 초기 표시 (약간의 딜레이 후)
        Invoke(nameof(InitialUpdate), 0.1f);
    }

    void OnDisable()
    {
        // 이벤트 구독 해제
        WaveController.OnWaveChanged -= UpdateWaveUI;
        WaveController.OnWaveStarted -= OnWaveStart;
        WaveController.OnWaveCompleted -= OnWaveComplete;
    }

    void InitialUpdate()
    {
        var waveController = WaveController.Instance;
        if (waveController != null)
        {
            UpdateWaveUI(waveController.CurrentWaveNumber);
        }
    }

    void UpdateWaveUI(int waveNumber)
    {
        if (waveText != null)
        {
            var waveController = WaveController.Instance;
            if (waveController != null)
            {
                waveText.text = $"Wave {waveNumber} / {waveController.TotalWaves:D2}";
            }
        }
    }

    void OnWaveStart(int waveNumber)
    {
        // 웨이브 시작 시 애니메이션 효과 (선택사항)
        Debug.Log($"Wave {waveNumber} Started!");

        // 예: 텍스트 색상 변경
        if (waveText != null)
        {
            waveText.color = Color.yellow;
            Invoke(nameof(ResetTextColor), 1f);
        }
    }

    void OnWaveComplete(int completedWaveIndex)
    {
        // 웨이브 완료 시 효과 (선택사항)
        Debug.Log($"Wave {completedWaveIndex + 1} Completed!");
    }

    void ResetTextColor()
    {
        if (waveText != null)
        {
            waveText.color = Color.white;
        }
    }
}