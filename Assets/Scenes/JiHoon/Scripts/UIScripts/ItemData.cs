using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Shop/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;     // 아이템 이름
    [TextArea]public string description;  // 아이템 설명
    public Sprite icon;         // 아이템 아이콘
    public int price;           // 아이템 가격

    public Sprite illustration; // 아이템 일러스트
}
