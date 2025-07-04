using JiHoon;
using UnityEngine;

public enum ItemType
{ 
Unit,                   // ) 유닛.
Equipment,           // ) 장비.
Consumable,         // ) 소모품.
Etc                     // ) 기타.
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Shop/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;     // 아이템 이름
    [TextArea]public string description;  // 아이템 설명
    public Sprite icon;         // 아이템 아이콘
    public int discontent;      // ) 구매시 소모되는 아이템의 불만 수치.
    public int price;           // 아이템 가격 = 재정 수치
    public int dominace;        // ) 구매시 소모되는 아이템의 지배 수치.
    public int chaos;        // ) 구매시 소모되는 아이템의 혼돈 수치.


    public Sprite illustration; // 아이템 일러스트

    public ItemType itemType;       // ) 아이템 타입
                                    // .
    [Header("유닛 프리팹 (UnitBase 스크립트가 붙은)")]
    public GameObject unitPrefab;
}
