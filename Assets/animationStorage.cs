using UnityEngine;

[CreateAssetMenu]
public class animationStorage : ScriptableObject
{
    public string prefabName;

    public int numberOfPrefabsToCreate;
    public Vector3[] spawnPoints;
}