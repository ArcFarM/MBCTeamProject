using UnityEngine;
using MainGame.Units;

namespace JeaYoon
{
    public class UN001 : MainGame.Units.UnitBase
    {
        protected override void Start()
        {
            // [◆] - ▶▶▶ 정의.
            unitID = "UN_001";                         // ) 항목 ID.
            unitName = "용사";                        // ) 항목명.
            faction = UnitFaction.Enemy;             // ) 아군/적군.

            // [◆] - ▶▶▶ 공격.
            baseDamage = 100f;                      // ) 공격력.
            baseAtkSpd = 0.5f;                        // ) 공격 대기 시간.
            baseSplash = 2f;                           // ) 방사형 공격범위.
            baseRange = 2f;                           // ) 사거리.

            // [◆] - ▶▶▶ 크기.
            baseRawSize = 1;                           // ) 배치타일 행 크기.
            baseColSize = 1;                            // ) 배치타일 열 크기.

            // [◆] - ▶▶▶ HP.
            baseHealth = 1000f;                        // ) HP.

            // [◆] - ▶▶▶ 이동속도.
            baseSpd = 1f;                               // ) 이동속도.

            // [◆] - ▶▶▶ ETC.

            base.Start();
        }
    }
}