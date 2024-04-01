using UnityEngine;
public static class GLOBAL_CONSTANTS 
{
    public const float GRAVITY_MAG = 10.0f;
    public struct objectType
    {
        public const int isEntity = 0;
        public const int isFloor = 1;
    }
    public struct entityType
    {
        public const int isPlayer = 0;
        public const int isEnemy = 1;
        public const int isFriendly = 2;
        public const int isInanimate = 3;
    }

    public struct floorType
    {
        public const int solid = 0;
        public const int platform = 1;
        public const int ceiling = 2;
    }

    public struct CharacterStates
    {
        public const int Running = 1;
        public const int Falling = 2;
        public const int activeAir = 3;
    }
}


