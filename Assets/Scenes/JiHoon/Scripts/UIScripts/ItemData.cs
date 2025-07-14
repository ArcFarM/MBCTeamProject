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
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public int unrest;
    public int price;
    public int dominance;
    public int manpower;
    public Sprite illustration;
    public ItemType itemType;

    [Header("유닛 프리팹 (UnitBase 스크립트가 붙은)")]
    public GameObject unitPrefab;

    [Header("카드덱용 데이터 (중요!)")]
    public JiHoon.UnitCardData unitCardData;  // 이 필드 추가!
}
