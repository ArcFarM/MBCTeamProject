using UnityEngine;

public class UN002 : MainGame.Units.UnitBase
{
    protected override void Start()
    {
        // [◆] - ▶▶▶ 정의.
        unitID = "UN_002";                         // ) 항목 ID.
        unitName = "기사단";                        // ) 항목명.
        faction = UnitFaction.Enemy;             // ) 아군/적군.

        // [◆] - ▶▶▶ 공격.
        baseDamage = 120f;                      // ) 공격력.
        baseAtkSpd = 0.25f;                        // ) 공격 대기 시간.
        baseSplash = 1f;                           // ) 방사형 공격범위.
        baseRange = 1f;                           // ) 사거리.

        // [◆] - ▶▶▶ 크기.
        baseRawSize = 2;                           // ) 배치타일 행 크기.
        baseColSize = 2;                            // ) 배치타일 열 크기.

        // [◆] - ▶▶▶ HP.
        baseHealth = 500f;                        // ) HP.

        // [◆] - ▶▶▶ 이동속도.
        baseSpd = 1f;                               // ) 이동속도.

        // [◆] - ▶▶▶ ETC.

        base.Start();
    }
}
