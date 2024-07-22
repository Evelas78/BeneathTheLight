
using UnityEditor.SearchService;
using UnityEngine;

public class IDScript : MonoBehaviour
{
    public bool isActive = false;
    public int objectType;
    public int menuObjTarget;
    //Jumpable for enemies and platforms means if you press space while on it, you'll jump.
    //For enemies, itll mean you gotta time the jump
    public int damage_level = 0;
    public bool hasEffectScript = false;
    public bool passThroughDown = false;
    public bool passThroughUp = false;
    public bool passThroughLeft = false;
    public bool passThroughRight = false;
    public string targetScene;
}
