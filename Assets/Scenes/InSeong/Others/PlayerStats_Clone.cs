/*using UnityEngine;

public class PlayerStats : MonoBehaviour
{







    //Life
    private static int lives;

    //소지금
    private static int money;

    //벌기, 쓰기, 소지금 확인 함수 만들기
    public static void AddMoney(int amount)
    {
        money += amount;
    }

    public static bool UseMoney(int amount)
    {
        //소지금 체크
        if (money < amount)
        {
            Debug.Log("소지금이 부족합니다");
            return false;
        }

        money -= amount;
        return true;
    }

    //생명 사용하기, 소모
    public static void UseLife(int amount)
    {
        lives -= amount;

        if (lives <= 0)
        {
            lives = 0;
        }
    }


}
*/