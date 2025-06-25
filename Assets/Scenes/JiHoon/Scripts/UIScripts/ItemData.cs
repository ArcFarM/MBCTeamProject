using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Shop/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;     // ������ �̸�
    [TextArea]public string description;  // ������ ����
    public Sprite icon;         // ������ ������
    public int price;           // ������ ����

    public Sprite illustration; // ������ �Ϸ���Ʈ
}
