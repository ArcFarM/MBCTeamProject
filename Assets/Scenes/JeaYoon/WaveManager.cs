using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;

/* [0] 개요 : WaveManager
		- Enemy의 스폰 및 웨이브를 관리하는 스크립트.
*/

public class WaveManager : MonoBehaviour
{
    // [1] Variable.
    #region ▼▼▼▼▼ Variable ▼▼▼▼▼
    // [◆] - ▶▶▶ 웨이브.
    public WaveManager[] waves;          // ) 웨이브 데이터 셋팅 : Enemy 프리팹, Enemy 생성 갯수, Enemy 젠 딜레이 타임.
    public float waveTimer = 5f;            // ) 타이머.
    private int waveCount = 0;              // ) 웨이브 카운트.
    public GameObject startButton;        // ) 웨이브 스타트 버튼.
    public GameObject waveInfo;           // ) 웨이브 정보.
    private int enemyCount;                 // ) 웨이브에서 생성할 Enemy 갯수.


    // [◆] - ▶▶▶ ETC.
    public static int enemyAlive = 0;       // ) 현재 게임화면에서 살아있는 Enemy의 숫자.
    public Transform startPoint;            // ) Enemy 스폰 위치.
    private float countdown = 0f;
    public TextMeshProUGUI countText;       // ) Text.
    public GameManager gameManager;         // ) 게임매니저.
    #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲





    // [2] Unity Event Method.
    #region ▼▼▼▼▼ Unity Event Method ▼▼▼▼▼
    // [◆] - ▶▶▶ Start.
    private void Start()
    {
        // [◇] - [◆] - ) 게임을 시작 할 때 모든 변수를 초기화해서 오류없이 첫 웨이브부터 정상적으로 작동할 수 있게 함.
        countdown = 3f;
        waveCount = 0;
        enemyAlive = 0;
        enemyCount = 0;
    }


    // [◆] - ▶▶▶ Update.
    private void Update()
    {
        // [◇] - [◆] - ) 현재 맵에 Enemy가 남아있는지 여부를 체크하여 젠 및 카운트다운을 막음.
        if (enemyAlive > 0)
        {
            // [◇] - [◇] - [◆] ) 147.

        }
    }

    // [◇] - [◆] - ) 789.
    // [◇] - [◇] - [◆] ) 147.
    // [◇] - [◇] - [◇] - [◆] ) 258.
    // [◇] - [◇] - [◇] - [◇] - [◆] ) 369.
    #endregion ▲▲▲▲▲ Unity Event Method ▲▲▲▲▲





    // [4] Custom Method.
    #region ▼▼▼▼▼ Custom Method ▼▼▼▼▼
    // [◆] - ▶▶▶ 123.


    // [◆] - ▶▶▶ 456.


    // [◇] - [◆] - ) 789.
    // [◇] - [◇] - [◆] ) 147.
    // [◇] - [◇] - [◇] - [◆] ) 258.
    // [◇] - [◇] - [◇] - [◇] - [◆] ) 369.
    #endregion ▲▲▲▲▲ Custom Method ▲▲▲▲▲
}
