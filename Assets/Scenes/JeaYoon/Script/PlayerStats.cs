using UnityEngine;

public class PlayerStats : MonoBehaviour
{







    //Life
    private static int lives;

    //������
    private static int money;

    //����, ����, ������ Ȯ�� �Լ� �����
    public static void AddMoney(int amount)
    {
        money += amount;
    }

    public static bool UseMoney(int amount)
    {
        //������ üũ
        if (money < amount)
        {
            Debug.Log("�������� �����մϴ�");
            return false;
        }

        money -= amount;
        return true;
    }

    //���� ����ϱ�, �Ҹ�
    public static void UseLife(int amount)
    {
        lives -= amount;

        if (lives <= 0)
        {
            lives = 0;
        }
    }


}
