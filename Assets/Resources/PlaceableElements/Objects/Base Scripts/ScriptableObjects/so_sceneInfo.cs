using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "Scene Info", menuName = "ScriptableObjects/sceneInfo", order = 1)]
public class SoSceneInfo : ScriptableObject
{
    public int sceneType = -1;
    //Write these in order please, since we'll be using trigger zones to move from camPoints
    public List<Vector3> camPoints = new List<Vector3>();
}