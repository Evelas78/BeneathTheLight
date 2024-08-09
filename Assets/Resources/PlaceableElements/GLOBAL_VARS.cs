using UnityEngine;
public static class GLOBAL_VARS
{
    public delegate void levelPrimer(int primeObjectType);
    public delegate void sceneLoad(int transitionType);
    public delegate void slowGameSignal(float numChange, float lerpFactor);
    public delegate void throwHatSignal(Vector2 _throwVec, Vector2 positionVec);
    public delegate void entityActiveChangeSignal(GameObject activatedObject, bool isMainChar, bool isNewCamTarget);
    public delegate void camChangeSignal(GameObject followObject, bool isActivation);

    public const float GRAVITY_MAG = 10.0f;
    public struct Layers
    {
        public const int main = 1 << 7;
    }
    public struct ControllerType
    {
        public const int levelController = 0;
        public const int entityController = 1;
        public const int camController = 2;
    }
    public struct Direction
    {
        public const int down = 1;
        public const int up = 2;
        public const int right = 3;
        public const int left = 4;
    }
    public struct ObjectType
    {
        public const int isPlayer = 0;
        public const int isEnemy = 1;
        public const int isObject = 2;
        public const int isWall= 3;
        public const int isMenuObj = 4;
        public const int isController = 5;
    }

    public struct MenuObjTarget
    {
        public const int changeScene = 0;
        public const int changeSettingValue = 1;
    }
    public struct transitionType
    {
        public const int basicFallBlack = 0;
        public const int basicFallMenu = 1;
        public const int basicRiseBlack = 2;
        public const int basicRiseMenu = 3;
    }

    public struct LevelStates
    {
        public const int levelLoading = 0;
        public const int levelFinalLoad = 1;
        public const int levelRunning = 2;
        public const int levelEnding = 3;
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

        public const int Jumping1 = 10;
        public const int Jumping2 = 11;
        public const int Jumping3 = 12;
        public const int FreeFalling1 = 13;
        public const int FreeFalling2 = 14;
        public const int FreeFalling3 = 15;

        public const int prepAbility1 = 30;
        public const int prepAbility2 = 31;
        public const int prepAbility3 = 32;
        public const int ActiveAbility1 = 33;
        public const int ActiveAbility2 = 34;
        
        public const int Hurt = 20;
        public const int Bonk = 21;
        public const int Dead = 22;
    }

    public struct sceneType
    {
        public const int menu = 0;
        public const int level = 1;
    }
}


