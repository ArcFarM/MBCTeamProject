using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MainGame.UI;

public class CardPool : MonoBehaviour
{
    [Header("참조할 카드 데이터베이스")]
    [SerializeField]
    private CardDatabase cardDatabase; // 단 하나의 참조!

    [Header("이벤트 상태")]
    public bool isCrisisEventActive = false;
    public bool isCrisisExclusive = true;

    private float normalTotalWeight;

    void Awake()
    {
        if (cardDatabase == null) {
            Debug.LogError("CardDatabase가 할당되지 않았습니다!");
            return;
        }
        // 데이터베이스로부터 가중치 합계를 계산
        normalTotalWeight = cardDatabase.gradePools.Sum(pool => pool.probabilityWeight);
    }

    public PolicyCard GetCard()
    {
        if (cardDatabase == null) return null;

        // 로직은 이전과 거의 동일하지만, 데이터를 내부가 아닌 cardDatabase에서 가져옵니다.
        List<CardGradeData> currentPools = new List<CardGradeData>();
        float currentTotalWeight = 0f;

        if (isCrisisEventActive)
        {
            if (isCrisisExclusive)
            {
                currentPools.Add(cardDatabase.crisisPool);
                currentTotalWeight = cardDatabase.crisisPool.probabilityWeight;
            }
            else
            {
                currentPools.AddRange(cardDatabase.gradePools);
                currentPools.Add(cardDatabase.crisisPool);
                currentTotalWeight = normalTotalWeight + cardDatabase.crisisPool.probabilityWeight;
            }
        }
        else
        {
            currentPools.AddRange(cardDatabase.gradePools);
            currentTotalWeight = normalTotalWeight;
        }

        if (currentTotalWeight <= 0) return null;

        float randomValue = Random.Range(0, currentTotalWeight);
        float cumulativeWeight = 0f;
        CardGradeData selectedGrade = null;

        foreach (var pool in currentPools)
        {
            cumulativeWeight += pool.probabilityWeight;
            if (randomValue <= cumulativeWeight) {
                selectedGrade = pool;
                break;
            }
        }
        
        if (selectedGrade != null && selectedGrade.cards.Count > 0)
        {
            // 선택된 등급에서 무작위 CardData를 뽑음
            int rndIndex = Random.Range(0, selectedGrade.cards.Count);
            CardData selectedCardData = selectedGrade.cards[rndIndex];

            // CardData에 저장된 프리팹을 사용해 인스턴스 생성
            GameObject cardPrefab = selectedCardData.cardPrefab;
            GameObject newCardInstance = Instantiate(cardPrefab, this.transform);
            PolicyCard_new pc = newCardInstance.GetComponent<PolicyCard_new>();

            if(pc != null) {
                pc.Initialize();
            }
        }

        return null;
    }
}