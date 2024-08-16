using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(0)]
[RequireComponent(typeof(AiSensor))]
[RequireComponent(typeof(AiTargetingSystem))]
[RequireComponent (typeof(NavMeshAgent))]
public class AiAgent : PoolableObject
{
    public int Points = 0;
    [HideInInspector]
    public AiAgent EnemyBase; // should only be filled by the scriptableobject, 
    // the isStationery flag
    public AiAgent[] EnemyBases = new AiAgent[3];

    public AiUnitScriptableObject AiConfig;
    public AiStateMachine stateMachine;
    public AiStateID initialState;

    [HideInInspector]
    public NavMeshAgent navMeshAgent;
    [HideInInspector]
    public AiSensor sensor;
    [HideInInspector]
    public AiTargetingSystem targeting;

    public Health health; // should have a collider on the same GameObject as the Health script
    //How frequentlY the current state will update
    public float AIStateUpdateInterval = 0.16f;
    private float timer = 0f;

    public List<GameObject> WeaponAnchors = new List<GameObject>();

    // Only for bases and stationary turrets..
    public bool isStationary = false;
    public Transform TargetTransform
    {
        get
        {
            return targeting.Target.transform;
        }
    }
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        targeting = GetComponent<AiTargetingSystem>();
        sensor = GetComponent<AiSensor>();
        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiChaseTargetState());
        stateMachine.RegisterState(new AiAttackBaseState());
    }
    void Start()
    {
        health.OnDeath += Die;
        health.OnTakeDamage += HealthDecrease;
        AiConfig.SetupAiUnit(this);
        sensor.sphereCollider = GetComponent<SphereCollider>();
        sensor.sphereCollider.radius = sensor.distance;
        stateMachine.ChangeState(initialState);
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            stateMachine.Update();
            timer += AIStateUpdateInterval;
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        navMeshAgent.enabled = false;
        sensor.ResetPotentialTargets();
    }

    void Die(Vector3 pos)
    {
        GameManager.Instance.AddPoints(Points);
        gameObject.SetActive(false); // return object to pool
    }

    void HealthDecrease(float dmg)
    {
        //Debug.Log(gameObject + $" took {dmg} damage...");
    }
}
