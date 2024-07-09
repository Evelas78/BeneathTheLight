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
        public const int isWall= 3;
    }

    public struct AirStates
    {
        public const int Grounded = 1;
        public const int Falling = 2;
        public const int activeAir = 3;
    }

    public struct CharacterStates
    {
        public const int Idle = 0;

        public const int Walking = 1;
        public const int Running = 2;
        public const int Sprinting = 3;
        public const int Skidding = 4;

        public const int Jumping = 10;
        public const int FreeFalling1 = 11;
        public const int FreeFalling2 = 12;

        public const int ActiveAbility = 30;
        
        public const int Hurt = 20;
        public const int Bonk = 21;
        public const int Dead = 22;
    }
}


