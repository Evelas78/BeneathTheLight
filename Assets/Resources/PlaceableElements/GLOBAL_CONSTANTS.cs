using UnityEngine;
public static class GLOBAL_CONSTANTS 
{
    public const float GRAVITY_MAG = 10.0f;
    public struct Layers
    {
        public const int main = 1 << 7;
    }
    public struct Direction
    {
        public const int down = 1;
        public const int up = 2;
        public const int right = 3;
        public const int left = 4;
    }
    public struct objectType
    {
        public const int isPlayer = 0;
        public const int isEnemy = 1;
        public const int isObject = 2;
        public const int isSolid = 3;
        public const int isPlatform = 4;
    }

    public struct AirStates
    {
        public const int Grounded = 1;
        public const int Falling = 2;
        public const int activeAir = 3;
    }

    public struct CharacterStates
    {
        public const int Walking = 0;
        public const int Running = 1;
        public const int Idle = 2;
        public const int Jumping = 2;
        public const int Hurt = 3;
        public const int Bonk = 4;
        public const int ActiveAbility = 5;
        public const int Dead = 6;
    }
}


