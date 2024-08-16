using UnityEngine;
using static UnityEngine.ParticleSystem;

[CreateAssetMenu(fileName = "Damage Config", menuName = "Guns/Damage Config", order = 1)]
public class DamageConfigScriptableObject : ScriptableObject
{
    public AnimationCurve DamageCurve;
    public float GetDamage(float Distance = 0)
    {
        return Mathf.CeilToInt(DamageCurve.Evaluate(Distance));
    }
}