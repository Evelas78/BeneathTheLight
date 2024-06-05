
using UnityEngine;

public class IDScript : MonoBehaviour
{
    public int objectType;
    //Jumpable for enemies and platforms means if you press space while on it, you'll jump.
    //For enemies, itll mean you gotta time the jump
    public int damage_level = 0;
    public bool hasEffectScript = false;
}
