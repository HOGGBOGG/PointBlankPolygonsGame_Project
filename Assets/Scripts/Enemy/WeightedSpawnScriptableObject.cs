using UnityEngine;

[CreateAssetMenu(fileName = "Weighted Spawn Config", menuName = "Weighted Spawn Config")]
public class WeightedSpawnScriptableObject : ScriptableObject
{
    public AiUnitScriptableObject Enemy;
    [Range(0, 1)]
    public float MinWeight;
    [Range(0, 1)]
    public float MaxWeight;

    public float GetWeight()
    {
        return Random.Range(MinWeight, MaxWeight);
    }
}