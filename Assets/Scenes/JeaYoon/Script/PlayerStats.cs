using UnityEngine;

/* [0] 개요 : PlayerStats
		- 플레이어의 속성(데이터)을 관리하는 클래스.
*/

namespace JeaYoon
{
    public class PlayerStats : MonoBehaviour
    {

        // [1] Variable.
        #region ▼▼▼▼▼ Variable ▼▼▼▼▼
        // [◆] - ▶▶▶ 정의.
        private static int unrest;                                // ) 불만.
        [SerializeField] private int startunrest = 50;          // ) 게임을 시작할 때 정해지는 불만.

        private static int finances;                                   // ) 재정.
        [SerializeField] private int startfinances = 1000;          // ) 게임을 시작할 때 정해지는 재정.

        private static int dominance;                                // ) 지배.
        [SerializeField] private int startdominance = 50;         // ) 게임을 시작할 때 정해지는 지배.

        private static int manpower;                                      // ) 혼돈.
        [SerializeField] private int startManpower = 50;               // ) 게임을 시작할 때 정해지는 혼돈.
        #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲


        


        // [2] Property.
        #region ▼▼▼▼▼ Property ▼▼▼▼▼
        // [◆] - ▶▶▶ 불만 읽기전용 속성.
        public static int Unrest
        {
            get { return unrest; }
        }


        // [◆] - ▶▶▶ 재정 읽기전용 속성.
        public static int Finances
        {
            get { return finances; }
        }


        // [◆] - ▶▶▶ 지배 읽기전용 속성.
        public static int Dominance
        {
            get { return dominance; }
        }


        // [◆] - ▶▶▶ 혼돈 읽기전용 속성.
        public static int Manpower
        {
            get { return manpower; }
        }


        // [◆] - ▶▶▶ 웨이브 카운트.
        public static int Waves { get; set; }
        #endregion ▲▲▲▲▲ Property ▲▲▲▲▲





        // [3] Unity Event Method.
        #region ▼▼▼▼▼ Unity Event Method ▼▼▼▼▼
        // [◆] - ▶▶▶ Start.
        private void Start()
        {
            // [◇] - [◆] - ) 초기화.
            unrest = startunrest;           // ) 불만.
            finances = startfinances;                // ) 재정.
            dominance = startdominance;         // ) 지배.
            manpower = startManpower;                      // ) 혼돈.
            Waves = 0;                                // ) 웨이브.
        }
        #endregion ▲▲▲▲▲ Unity Event Method ▲▲▲▲▲





        // [4-1] Custom Method(불만도).
        #region ▼▼▼▼▼ Custom Method(불만도) ▼▼▼▼▼
        // [◆] - ▶▶▶ Addunrest → 불만도 추가.
        public static void Addunrest(int amount)
        {
            unrest += amount;
        }


        // [◆] - ▶▶▶ Useunrest → 불만도 사용.
        public static bool Useunrest(int amount)
        {
            // [◇] - [◆] - ) .
            if (unrest < amount)
            {
                Debug.Log("불만도가 부족합니다.");
                return false;
            }
            unrest -= amount;
            return true;
        }


        // [◆] - ▶▶▶ Hasunrest → 보유하고 있는 불만도.
        public static bool Hasunrest(int amount)
        {
            return unrest >= amount;
        }
        #endregion ▲▲▲▲▲ Custom Method(불만도) ▲▲▲▲▲





        // [4-2] Custom Method(재정).
        #region ▼▼▼▼▼ Custom Method(재정) ▼▼▼▼▼
        // [◆] - ▶▶▶ AddFinances → 재정 추가.
        public static void AddFinances(int amount)
        {
            finances += amount;
        }


        // [◆] - ▶▶▶ Useunrest → 재정 사용.
        public static bool UseFinances(int amount)
        {
            // [◇] - [◆] - ) .
            if (finances < amount)
            {
                Debug.Log("재정이 부족합니다.");
                return false;
            }
            finances -= amount;
            return true;
        }


        // [◆] - ▶▶▶ HasFinances → 보유하고 있는 재정.
        public static bool HasFinances(int amount)
        {
            return finances >= amount;
        }
        #endregion ▲▲▲▲▲ Custom Method(재정) ▲▲▲▲▲





        // [4-3] Custom Method(지배).
        #region ▼▼▼▼▼ Custom Method(지배) ▼▼▼▼▼
        // [◆] - ▶▶▶ AddDominance → 지배 추가.
        public static void AddDominance(int amount)
        {
            dominance += amount;
        }


        // [◆] - ▶▶▶ UseDominance → 지배 사용.
        public static bool UseDominance(int amount)
        {
            // [◇] - [◆] - ) .
            if (dominance < amount)
            {
                Debug.Log("지배가 부족합니다.");
                return false;
            }
            dominance -= amount;
            return true;
        }


        // [◆] - ▶▶▶ HasDominance → 보유하고 있는 지배.
        public static bool HasDominance(int amount)
        {
            return dominance >= amount;
        }
        #endregion ▲▲▲▲▲ Custom Method(지배) ▲▲▲▲▲





        // [4-4] Custom Method(혼돈).
        #region ▼▼▼▼▼ Custom Method(혼돈) ▼▼▼▼▼
        // [◆] - ▶▶▶ AddManpower → 혼돈 추가.
        public static void AddManpower(int amount)
        {
            manpower += amount;
        }


        // [◆] - ▶▶▶ UseManpower → 혼돈 사용.
        public static bool UseManpower(int amount)
        {
            // [◇] - [◆] - ) .
            if (manpower < amount)
            {
                Debug.Log("혼돈이 부족합니다.");
                return false;
            }
            manpower -= amount;
            return true;
        }


        // [◆] - ▶▶▶ HasManpower → 보유하고 있는 혼돈.
        public static bool HasManpower(int amount)
        {
            return manpower >= amount;
        }
        #endregion ▲▲▲▲▲ Custom Method(혼돈) ▲▲▲▲▲
    }
}
