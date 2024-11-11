using System;
using UnityEngine;
public static class GLOBAL_VARS
{

    public delegate void gcBeginSignal();
    public delegate void controllerResponseSignal(bool successful);
    public delegate void sceneLoad();
    public delegate void buttonMovementSignal(Vector2 newPos, float time, int lerpType);
    public delegate void buttonTransitionSignal(int startTransitionType, int endTransitionType, int startLerpType, int endLerpType, float startTransitonTime, float endTransitionTime, string targetRoom, bool isLevel);
    public delegate void gcTransitionSignals(int transitionType, int lerpType, float transitionTime, bool isClose);
    public delegate void transitionCompleted();
    public delegate void levelPause(bool isPause);
    public delegate void sceneChangeButtonSignal(int startTransType, int endTransType, float startTransTime, float endTransTime, string sceneTarget);
    public delegate void slowGameSignal(float numChange, float lerpFactor);
    public delegate void throwHatSignal(Vector2 throwVec, Vector2 positionVec);
    public delegate void entityActiveChangeSignal(GameObject activatedObject, bool isMainChar, bool isNewCamTarget);
    public delegate void camChangeSignal(GameObject followObject, bool isActivation, int lerpType, float timeToReach);

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
    public struct ObjectType
    {
        public const int isPlayer = 0;
        public const int isEnemy = 1;
        public const int isObject = 2;
        public const int isWall= 3;
        public const int isMenuObj = 4;
        public const int isController = 5;
    }

    //Open means it reveals the vision of the scene
    //Close means it hides the vision of the scene.
    public struct transitionType
    {
        public const int basicCloseFade = 0;
        public const int basicOpenFade = 1;
        public const int basicCloseRise = 2;
        public const int basicOpenRise = 3;
        public const int basicCloseFall = 4;
        public const int baseOpenFall = 5;
    }

    public struct GameStates
    {
        public const int gamePreparing = 0; //Initial Load, means the the game controller is the first thing working before anything else
        public const int gameLoading = 1; //Regular loads, whether this be transitioning or just moving where you can interact with the game
        public const int menuState = 2; //We're in a menu, this basically just means, we can interact with buttons on the screen and means the level youre on is frozen
        public const int levelState = 3; //We're in a level and yeah, we control our characters.
        public const int levelEnding = 4; //Ending the level, so somethign important is probably happening here, gotta check if like. This will always go into a transition phase

        public const int sceneLeaving = 7;
        public const int sceneEntering = 8;
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

    public struct SceneLoadType
    {
        public const int menu = 0;
        public const int level = 1;
        public const int unset = 2;
    }

    public struct ButtonType
    {
        public const int transitionScene = 0;
        public const int valueChange = 1;
        public const int moveScreen = 2;
    }

    public struct LerpType
    {
        public const int linear = 0;
        public const int exponential = 1;
        public const int decay = 2;
        public const int SFS = 3;
        public const int FSF = 4;
    }

    public static float lerpWorker(int _lerpType, float a, float b, float curr, float max)
    {
        float result = 0;
        switch(_lerpType)
        {
            case GLOBAL_VARS.LerpType.linear:
                result = GLOBAL_VARS.linearLerp(a, b, curr, max);
                break;
            case GLOBAL_VARS.LerpType.decay:
                result = GLOBAL_VARS.decayLerp(a, b, curr, max);
                break;
            case GLOBAL_VARS.LerpType.exponential:
                result = GLOBAL_VARS.expoLerp(a, b, curr, max);
                break;
            case GLOBAL_VARS.LerpType.FSF:
                result = GLOBAL_VARS.FSFLerp(a, b, curr, max);
                break;
            case GLOBAL_VARS.LerpType.SFS:
                result = GLOBAL_VARS.SFSLerp(a, b, curr, max);
                break;
            default:
                Debug.LogError("Lerp type doesnt exist: " + _lerpType);
                break;
        }
        return result;
    }
    //made for uniformity, next ones are what matters
    public static float linearLerp(float a, float b, float curr, float max)
    {   
        return Mathf.Lerp(a,b,curr/max); 
    }

    public static float decayLerp(float a, float b, float curr, float max)
    {
        return Mathf.Lerp(a, b, decayDown(curr/max));
    }
    public static float expoLerp(float a, float b, float curr, float max)
    {
        return Mathf.Lerp(a, b, expoUp(curr/max));
    }
    public static float SFSLerp(float a, float b, float curr, float max)
    {
        float currCounter = Mathf.Lerp(expoUp(curr/max), decayDown(curr/max), curr/max);
        return Mathf.Lerp(a, b, currCounter);
    }
    public static float FSFLerp(float a, float b, float curr, float max)
    {
        float currCounter = Mathf.Lerp(decayDown(curr/max), expoUp(curr/max), curr/max);
        return Mathf.Lerp(a, b, currCounter);
    }
    public static float squareVal(float input)
    {
        return input * input;
    }
    public static float flipFraction(float input)
    {
        return 1 - input;
    }
    public static float expoUp(float input)
    {
        return squareVal(input);
    }
    public static float decayDown(float input)
    {
        //flip once, to get the reverse speed, square it, then flip again so its at the right spot
        return flipFraction(squareVal(flipFraction(input)));
    }
}


