using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class scr_Menu : MonoBehaviour
{
    [SerializeField] protected int[] button_Array;
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    public abstract void menuUpdate();
}
