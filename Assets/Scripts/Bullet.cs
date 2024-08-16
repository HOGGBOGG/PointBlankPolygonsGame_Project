using System.Collections;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    private Rigidbody Rigidbody;
    [field: SerializeField]

    private QuadraticCurve Curve = new QuadraticCurve();
    public Vector3 SpawnLocation
    {
        get; private set;
    }

    public delegate void CollisionEvent(Bullet Bullet, Collision Collision);
    public event CollisionEvent OnCollsion;

    private void Awake()
    {
        //Rigidbody = GetComponent<Rigidbody>();
    }

    public void Spawn(Vector3 Startpoint,Vector3 ControlPoint,Vector3 EndPoint,float reachTime)
    {
        Curve.Origin = Startpoint;
        Curve.ControlPoint = ControlPoint;
        Curve.End = EndPoint;
        StartCoroutine(QuadraticTrajectory(1f / reachTime));
        StartCoroutine(DelayedDisable(reachTime));
    }

    private IEnumerator QuadraticTrajectory(float speed)
    {
        float timer = 0f;
        while(timer < 1)
        {
            timer += Time.deltaTime * speed;
            transform.position = Curve.EvaluatePosition(timer);
            yield return null;
        }
    }
    private IEnumerator DelayedDisable(float Time)
    {
        yield return new WaitForSeconds(Time);
        OnCollisionEnter(null);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollsion?.Invoke(this, collision);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        //if(Rigidbody != null) 
        //Rigidbody.velocity = Vector3.zero;
        //Rigidbody.angularVelocity = Vector3.zero;
        OnCollsion = null;
    }
}