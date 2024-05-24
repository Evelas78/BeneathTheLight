
using UnityEngine;

public class IDScript : MonoBehaviour
{
    public int objectType;
    public int versionType;

    //Jumpable for enemies and platforms means if you press space while on it, you'll jump.
    //For enemies, itll mean you gotta time the jump
    public int jump_level;
    //Means touching it will damage you, most enemies will have this and some platforms may. 
    //Jumping gets checked first though so if you are in a jumping position, easy game!!!
    public int damage_level = 0;
}
