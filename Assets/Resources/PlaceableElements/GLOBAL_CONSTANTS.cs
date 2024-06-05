using UnityEngine;
public static class GLOBAL_CONSTANTS 
{
    public const float GRAVITY_MAG = 10.0f;
    public struct layers
    {
        public const int main = 1 << 7;
    }
    public struct objectType
    {
        public const int isPlayer = 0;
        public const int isEnemy = 1;
        public const int isObject = 2;
        public const int isSolidFloor = 3;
        public const int isPlatformFloor = 4;
    }

    public struct AirStates
    {
        public const int Grounded = 1;
        public const int Falling = 2;
        public const int activeAir = 3;
    }

    public struct ActionStates
    {
        public const int Jumping = 1;
        public const int Falling = 2;
        public const int SpecialFall = 3;
        public const int Idle = 4;
        public const int Walking = 5;
        public const int Running = 6;
        public const int Rolling = 7;
        public const int Attack1 = 8;
        public const int Attack2 = 9;
        public const int Attack3 = 10;
        public const int Hurt = 11;
        public const int Bonk = 12;
    }
}


