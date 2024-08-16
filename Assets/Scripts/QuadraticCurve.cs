using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadraticCurve
{
    //use for projectile
    public Vector3 Origin;
    public Vector3 End;
    public Vector3 ControlPoint;

    // use for projectile
    public Vector3 EvaluatePosition(float t)
    {
        Vector3 ac = Vector3.Lerp(Origin, ControlPoint, t);
        Vector3 cb = Vector3.Lerp(ControlPoint, End, t);
        return Vector3.Lerp(ac, cb, t);
    }
}
