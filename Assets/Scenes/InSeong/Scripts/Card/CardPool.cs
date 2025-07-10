/*using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MainGame.Enum;
using MainGame.UI;

// CardGradePool 클래스는 이전과 동일합니다.
[System.Serializable]
public class CardGradePool {
    public string name;
    public CardGrade grade;
    public List<GameObject> cardPrefabs;
    public float probabilityWeight;
}

public class CardPool : MonoBehaviour {
    #region Variables
    [Header("일반 카드 등급 설정")]
    [SerializeField]
    private List<CardGradePool> gradePools;

    [Header("위기 카드 설정")]
    [SerializeField]
    private CardGradePool crisisPool;

    [Header("이벤트 상태")]
    public bool isCrisisEventActive = false; // 위기 이벤트 발생 여부

    // 이 변수가 true이면 위기 카드만, false이면 기존 풀에 위기 카드가 추가됨
    public bool isCrisisExclusive = true;

    private float normalTotalWeight;
    #endregion

    #region Unity Event Methods
    void Awake() {
        // 일반 카드들의 전체 확률 가중치 합을 미리 계산해둡니다.
        normalTotalWeight = gradePools.Sum(pool => pool.probabilityWeight);
    }
    #endregion

    #region Custom Methods
    /// <summary>
    /// 설정된 확률에 따라 무작위 카드 프리팹의 복사본(인스턴스)을 반환합니다.
    /// 위기 이벤트의 종류(독점적/포함)를 고려합니다.
    /// </summary>
    /// <returns>생성된 카드의 PolicyCard 컴포넌트, 실패 시 null</returns>
    public PolicyCard GetCard() {
        // 1. 현재 상태에 맞는 유효 카드 풀 리스트와 전체 가중치를 결정합니다.
        List<CardGradePool> currentPools = new List<CardGradePool>();
        float currentTotalWeight = 0f;

        if (isCrisisEventActive) {
            if (isCrisisExclusive) {
                // "독점적" 위기: 위기 카드 풀만 사용합니다.
                currentPools.Add(crisisPool);
                currentTotalWeight = crisisPool.probabilityWeight;
            } else {
                // "포함" 위기: 일반 풀 + 위기 풀을 모두 사용합니다.
                currentPools.AddRange(gradePools);
                currentPools.Add(crisisPool);
                currentTotalWeight = normalTotalWeight + crisisPool.probabilityWeight;
            }
        } else {
            // 평상시: 일반 카드 풀만 사용합니다.
            currentPools.AddRange(gradePools);
            currentTotalWeight = normalTotalWeight;
        }

        // 2. 결정된 카드 풀과 가중치를 기반으로 카드를 뽑습니다.
        if (currentTotalWeight <= 0) {
            Debug.LogWarning("유효한 카드가 없거나 확률 가중치의 합이 0입니다.");
            return null;
        }

        float randomValue = Random.Range(0, currentTotalWeight);
        float cumulativeWeight = 0f;
        CardGradePool selectedPool = null;

        foreach (var pool in currentPools) {
            cumulativeWeight += pool.probabilityWeight;
            if (randomValue <= cumulativeWeight) {
                selectedPool = pool;
                break;
            }
        }

        // 3. 선택된 풀에서 카드를 생성하여 반환합니다.
        if (selectedPool != null && selectedPool.cardPrefabs.Count > 0) {
            int rndIndex = Random.Range(0, selectedPool.cardPrefabs.Count);
            GameObject cardPrefab = selectedPool.cardPrefabs[rndIndex];

            GameObject newCardInstance = Instantiate(cardPrefab, this.transform);
            return newCardInstance.GetComponent<PolicyCard>();
        }

        Debug.LogWarning("카드를 뽑을 수 없습니다. CardPool 설정을 확인하세요.");
        return null;
    }
    #endregion
}
*/