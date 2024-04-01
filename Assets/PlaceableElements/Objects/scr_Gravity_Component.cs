using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class scr_Gravity_Component : MonoBehaviour
{
   public scr_Basic_Entity characterScript;
   public float gravStrength = 0.1f;
   
   public void GravityUpdate()
   {
      characterScript.changeTo.y -= gravStrength;
   }
}