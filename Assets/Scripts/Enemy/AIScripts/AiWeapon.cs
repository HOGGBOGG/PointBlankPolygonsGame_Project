using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;


[RequireComponent(typeof(AudioSource))]
public class AiWeapon : MonoBehaviour
{
    // public GameObject TrailsDisabler;
    public AiStateMachine StateMachine;
    public AiAgent Agent;

    public ShootConfigScriptableObject ShootConfig;
    public TrailConfigScriptableObject TrailConfig;
    public AudioConfigScriptableObject AudioConfig;
    public DamageConfigScriptableObject DamageConfig;

    protected ParticleSystem ShootSystem;
    protected AudioSource ShootingAudioSource;
    public Transform Anchor;

    public float TimeIntervalsLook = 0.5f;
    public float innacuraccyLook = 1f;
    public float DamageRadius = 1f;


    // add burst to weapon
    [SerializeField]
    protected bool isBurst = false;
    public bool isPlayerWeapon = false;
    public float burstInterval = 2f;
    public int burstCount = 3;

    protected int _burstCount;
    protected float burstTimer = 0f;
    private float timer = 0f;
    protected float LastShootTime = 0f;
    protected ObjectPool<TrailRenderer> TrailPool;
    protected readonly List<TrailRenderer> allTrails = new List<TrailRenderer>();


    protected void InitialiseCommonAspects()
    {
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        StateMachine = Agent.stateMachine;
        ShootSystem = GetComponentInChildren<ParticleSystem>();
        ShootingAudioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        InitialiseCommonAspects();
    }

    // Update is called once per frame
    void Update()
    {
        //LookAtTarget();
        if (isBurst)
        {
            ShootBurst();
        }
        else
        {
            Shoot();
        }
    }

    protected void LookAtTarget()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)// loop to rotate the anchor towards the target
        {
            if (!Agent.targeting.HasTarget) return;
            Vector3 targetPosition = Agent.TargetTransform.position;
            targetPosition += Random.insideUnitSphere * innacuraccyLook;
            targetPosition.y = Anchor.position.y;
            Anchor.LookAt(targetPosition);
            timer = TimeIntervalsLook;
        }
    }

    public float Speed = 1f;

    private Coroutine LookCoroutine;
    private IEnumerator LookAt()
    {
        Quaternion lookRotation = Quaternion.LookRotation(Agent.TargetTransform.position - Anchor.position);

        float time = 0;

        Quaternion initialRotation = Anchor.rotation;
        while (time < 1)
        {
            Anchor.rotation = Quaternion.Slerp(initialRotation, lookRotation, time);
            time += Time.deltaTime * Speed;

            yield return null;
        }
        while (Agent.targeting.HasTarget)
        {
            lookRotation = Quaternion.LookRotation(Agent.TargetTransform.position - Anchor.position);
            Anchor.rotation = Quaternion.Slerp(initialRotation, lookRotation, 0.99f);
            yield return null;
        }
    }
    protected virtual void ShootBurst() // override just for laser weapons
    {
        if (burstTimer < 0f)// can now shoot in burst
        {
            if (_burstCount < burstCount)
            {
                Shoot();
            }
            else
            {
                _burstCount = 0;
                burstTimer = burstInterval;
            }
        }
        burstTimer -= Time.deltaTime;
    }
    public bool IsChasing()
    {
        if(StateMachine.currentStateID == AiStateID.ChaseTarget)
        {
            return true;
        }
        return false;
    }
    public bool IsInRangeAndAngle()
    {
        if (!Agent.targeting.HasTarget ) return false;
        //Vector3 targetDirection = Agent.TargetTransform.position - Agent.transform.position;
        //targetDirection.Normalize();

        return true; // CHANGES

        ////float Dot = Vector3.Dot(targetDirection, ShootSystem.transform.forward); // Will anyways look at the target in the update loop
        //float AbsDot = Mathf.Abs(Dot);
        ////Debug.Log(Dot);
        //if (AbsDot > 0.6f)
        //{
        //    return true;
        //}
        //return false;
    }
    protected virtual void Shoot()
    {
        if (!IsChasing() || !IsInRangeAndAngle()) 
        {
            if (LookCoroutine != null)
            {
                StopCoroutine(LookCoroutine);
                LookCoroutine = null;
            }
            return; 
        }
        if (LookCoroutine == null)
        {
            LookCoroutine = StartCoroutine(LookAt());
        }

        if (Time.time > ShootConfig.FireRate + LastShootTime)
        {
            LastShootTime = Time.time;
            ShootSystem.Play(); //CHANGES
            //AudioConfig.PlayShootingClip(ShootingAudioSource, false);
            if (isPlayerWeapon && _burstCount == 0)
            {
                AudioConfig.PlayShootingClip(ShootingAudioSource, false);
            }
            else if(!isPlayerWeapon)
            {
                AudioConfig.PlayShootingClip(ShootingAudioSource, false);
            }

            Vector3 shootDirection = ShootSystem.transform.forward
                + new Vector3(
                    Random.Range(
                        -ShootConfig.Spread.x,
                        ShootConfig.Spread.x
                    ),
                    Random.Range(
                        -ShootConfig.Spread.y,
                        ShootConfig.Spread.y
                    ),
                    Random.Range(
                        -ShootConfig.Spread.z,
                        ShootConfig.Spread.z
                    )
                );
            shootDirection.Normalize();
            DoHitscanShoot(shootDirection);
            _burstCount++;
        }
    }

    // LOGIC FOR HITSCAN GUNS

    private void DoHitscanShoot(Vector3 shootDirection)
    {
        if (Physics.Raycast(
                        ShootSystem.transform.position,
                        shootDirection,
                        out RaycastHit hit,
                        float.MaxValue,
                        ShootConfig.HitMask
                    ))
        {
            StartCoroutine(
                PlayTrail(
                    ShootSystem.transform.position,
                    hit.point,
                    hit,
                    true
                )
            );
        }
        else
        {
            StartCoroutine(
                PlayTrail(
                    ShootSystem.transform.position,
                    ShootSystem.transform.position + (shootDirection * TrailConfig.MissDistance),
                    new RaycastHit(),
                    false
                )
            );
        }
    }
    private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit Hit,bool hitSomething) // Endpoint can be varying so need changes in the code
    {

        TrailRenderer instance = TrailPool.Get();
        allTrails.Add(instance); // for releasing when the Unit dies.
        instance.gameObject.SetActive(true);
        instance.transform.position = StartPoint;

        yield return null; // avoid position carry-over from last frame if reused

        instance.emitting = true;

        float distance = Vector3.Distance(StartPoint, EndPoint);
        float remainingDistance = distance;
        while (remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(
                StartPoint,
                EndPoint,
                Mathf.Clamp01(1 - (remainingDistance / distance))
            );
            remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = EndPoint;

        if(hitSomething == true)
        {
            float Magnitude = (Hit.collider.transform.position - (EndPoint - Hit.normal*1f)).magnitude;
            if (Magnitude < DamageRadius)
            {
                if (Hit.collider.TryGetComponent<IDamageable>(out IDamageable damageable))
                {
                    damageable.TakeDamage(DamageConfig.GetDamage(distance));
                }
            }
            else
            {
                // logic for missing shots
            }
        }


        if (Hit.collider != null)
        {
            //if (Hit.collider.TryGetComponent<IDamageable>(out IDamageable damageable))
            //{
            //    damageable.TakeDamage(DamageConfig.GetDamage(distance));
            //}
        }

        yield return new WaitForSeconds(TrailConfig.Duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        TrailPool.Release(instance);
    }

    // LOGIC FOR LASER GUNS

    // REMAINING LOGIC
    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = TrailConfig.Color;
        trail.material = TrailConfig.Material;
        trail.widthCurve = TrailConfig.WidthCurve;
        trail.time = TrailConfig.Duration;
        trail.minVertexDistance = TrailConfig.MinVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }
    protected virtual void OnDisable() 
    {
        foreach (TrailRenderer trail in allTrails)
        {
            if (trail.gameObject.activeSelf)
            {
                trail.emitting = false;
                trail.gameObject.SetActive(false);
                TrailPool.Release(trail);
            }
        }
    }
}
