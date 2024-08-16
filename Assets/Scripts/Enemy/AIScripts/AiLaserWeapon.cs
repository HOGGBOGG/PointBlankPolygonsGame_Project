using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AiLaserWeapon : AiWeapon
{
    public LineRenderer lineRenderer;
    private bool isShooting = false;
    private float clipTimer = 2f;
    public float initialCut = 1f;
    public float finalCut = 1.8f;
    public float pierceDistance = 1f;
    public float LaserRange = 1000f;
    private IDamageable damageablehit = null;

    private void OnEnable()
    {
        lineRenderer.enabled = true;
    }
    void Start()
    { 
        InitialiseCommonAspects();
        InitializeBurst();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    private void InitializeBurst()
    {
        burstInterval = ShootingAudioSource.clip.length;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Agent.targeting.HasTarget)
        //    LookAtTarget();
        ////Shoot();
        //ShootBurst();
    }

    protected override void ShootBurst()
    {
        if (isShooting == false && Agent.targeting.HasTarget)
        {
            ShootingAudioSource.Play();
            clipTimer = 3f;
            isShooting = true;
            burstTimer = burstInterval;
            return;
        }
        else if (!Agent.targeting.HasTarget)
        {
            isShooting = false;
            if (ShootingAudioSource.time > initialCut && ShootingAudioSource.time < burstInterval - finalCut)
                Shoot();
            clipTimer = 3f;
            return;
        }
        burstTimer -= Time.deltaTime;
        if (burstTimer > 0)
        {
            if ( ShootingAudioSource.isPlaying && ShootingAudioSource.time > initialCut && ShootingAudioSource.time < burstInterval - finalCut)
                Shoot();
            else
            {
                lineRenderer.SetPosition(0, ShootSystem.transform.position);
                lineRenderer.SetPosition(1, ShootSystem.transform.position);
            }
        }
        else // burst timer < 0
        {
            lineRenderer.SetPosition(0, ShootSystem.transform.position); // Changes
            lineRenderer.SetPosition(1, ShootSystem.transform.position); // Changes
            clipTimer -= Time.deltaTime;
            if (clipTimer < 0)
            {   
                isShooting = false;
                clipTimer = 3f;
                burstTimer = burstInterval;
            }
        }
    }


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

    private void LateUpdate()
    {
        if (Agent.targeting.HasTarget)
        {
            if (LookCoroutine == null)
            {
                LookCoroutine = StartCoroutine(LookAt());
            }
        }
        else
        {
            if (LookCoroutine != null)
            {
                StopCoroutine(LookCoroutine);
                LookCoroutine = null;
            }
        }
        ShootBurst();
        if (!Agent.targeting.HasTarget)
        {
            ShootingAudioSource.Stop(); // CHANGES
            lineRenderer.SetPosition(0, ShootSystem.transform.position);
            lineRenderer.SetPosition(1, ShootSystem.transform.position);
            damageablehit = null;
        }
    }
    protected override void Shoot()
    {
        if (!Agent.targeting.HasTarget) return; // new CHANGE EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
        if (Physics.Raycast(
                        ShootSystem.transform.position,
                        ShootSystem.transform.forward,
                        out RaycastHit hit,
                        1000f,//float.MaxValue,
                        ShootConfig.HitMask
                    ))
        {
            lineRenderer.SetPosition(0, ShootSystem.transform.position);
            lineRenderer.SetPosition(1, hit.point + ShootSystem.transform.forward * pierceDistance);
            if (hit.collider != null)
            {
                //if (damageablehit == null)
                //{
                    if (hit.collider.TryGetComponent<IDamageable>(out IDamageable damageable))
                    {
                        damageablehit = damageable;
                        damageable.TakeDamage(DamageConfig.GetDamage(5) * Time.deltaTime);
                    }
                }
                //else
                //{
                //    damageablehit.TakeDamage(DamageConfig.GetDamage(5) * Time.deltaTime);
                //}
            //}
        }
        else
        {
            //ShootingAudioSource.Stop();
            //damageablehit = null;
            lineRenderer.SetPosition(0, ShootSystem.transform.position);
            lineRenderer.SetPosition(1, ShootSystem.transform.position + ShootSystem.transform.forward * 1000f);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if(ShootingAudioSource != null)
        ShootingAudioSource.Stop();
        lineRenderer.enabled = false;
    }
}
