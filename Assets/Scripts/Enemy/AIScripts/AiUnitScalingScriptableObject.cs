using UnityEngine;

[CreateAssetMenu(fileName = "Scaling Configuration", menuName = "AiUnit Scaling")]
public class AiUnitScalingScriptableObject : ScriptableObject
{
    public AnimationCurve HealthCurve;
    public AnimationCurve DamageCurve;
    public AnimationCurve SpeedCurve;
    public AnimationCurve SpawnRateCurve;
    public AnimationCurve SpawnCountCurve;
}