using System;
using MainGame.Enum;

// pair를 인스펙터에서 사용하기 위한 구조체
[Serializable]
public struct StatStruct {
    public Stats stat;
    public int value;
}