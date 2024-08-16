using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AiMissileWeapon : AiWeapon
{
    private Collider[] colliders = new Collider[15];
    private ObjectPool<Bullet> BulletPool;
    public float timeToReach = 2f;
    public float ExplosionDuration = 2f;
    public float ExplosionRadius = 10f;
    public float ExplosionDamage = 20f;
    public Bullet BulletPrefab;
    public float ControlSensitivity = 2f;
    [Tooltip("Explosion layers to be taken in consideration")]
    public LayerMask ExplosionLayers;
    public float XZoffset = 10f;
    public float Yoffset = 10f;
    public float maxRange = 300f;
    // NO NEED TO CALL FOR LOOKAT TARGETS
    [SerializeField]
    private ImpactType ImpactType;

    protected void Start()
    {
        InitialiseCommonAspects();
        BulletPool = new ObjectPool<Bullet>(CreateBullet);
    }
    private Bullet CreateBullet()
    {
        return Instantiate(BulletPrefab);
    }

    public void Update()
    {
        LookAtTarget();
        if (isBurst)
        {
            ShootBurst();
        }
        else
        {
            Shoot();
        }
    }

    protected override void Shoot()
    {
        if (!IsChasing() || !Agent.targeting.HasTarget)
        {
            return;
        }

        if (Time.time > ShootConfig.FireRate + LastShootTime)
        {
            LastShootTime = Time.time;
            ShootSystem.Play();
            //AudioConfig.PlayShootingClip(ShootingAudioSource, false);

            DoProjectileShoot(ShootSystem.transform.position, CalculateControlPoint(), Agent.targeting.TargetPosition);
            _burstCount++;
        }
    }
    private void DoProjectileShoot(Vector3 StartPosition, Vector3 ControlPoint, Vector3 EndPosition)
    {
        Bullet bullet = BulletPool.Get();
        bullet.gameObject.SetActive(true);
        bullet.OnCollsion += HandleBulletCollision;
        bullet.transform.position = ShootSystem.transform.position;
        bullet.Spawn(StartPosition, ControlPoint, EndPosition, timeToReach);

        TrailRenderer trail = TrailPool.Get();
        allTrails.Add(trail); // for removing all trails when unit dies
        if (trail != null)
        {
            trail.transform.SetParent(bullet.transform, false);
            trail.transform.localPosition = Vector3.zero;
            trail.emitting = true;
            trail.gameObject.SetActive(true);
        }
    }
    private void HandleBulletCollision(Bullet Bullet, Collision Collision)
    {
        Vector3 lastBulletPosition = Bullet.transform.position;
        TrailRenderer trail = Bullet.GetComponentInChildren<TrailRenderer>();
        if (trail != null)
        {
            trail.transform.SetParent(null, true);
            StartCoroutine(DelayedDisableTrail(trail));
        }
        Vector3 bulletLastPos = Bullet.transform.position; // for SurfaceManager only
        Bullet.OnCollsion -= HandleBulletCollision;// change
        Bullet.gameObject.SetActive(false);
        BulletPool.Release(Bullet);
        if (Collision != null)
        {
            if (Collision.collider.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(DamageConfig.GetDamage(10));
            }
        }
        if (Physics.Raycast(lastBulletPosition, -Vector3.up, out RaycastHit hit, 10f, -1))
        {
            SurfaceManager.Instance.PlayEffectAtPosition(bulletLastPos, ImpactType, ExplosionDuration);
        }
        Explosion(lastBulletPosition);
        
    }
    private void Explosion(Vector3 position)
    {
        int hits = Physics.OverlapSphereNonAlloc(position, ExplosionRadius, colliders, ExplosionLayers);
        if (hits < 1) return;
        for (int i = 0; i < hits; i++)
        {
            if (colliders[i].TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(ExplosionDamage);
            }
        }
    }
    private IEnumerator DelayedDisableTrail(TrailRenderer Trail)
    {
        yield return new WaitForSeconds(TrailConfig.Duration);
        yield return null;
        Trail.emitting = false;
        Trail.gameObject.SetActive(false);
        TrailPool.Release(Trail);
    }
    private Vector3 CalculateControlPoint()
    {
        float Distance = (Agent.targeting.TargetPosition - ShootSystem.transform.position).magnitude;
        float MaxRange = maxRange;
        float RandomXZ = Random.Range(-XZoffset, XZoffset);
        float RandomY = Random.Range(0f, Yoffset);
        Mathf.Clamp(RandomXZ, -(Distance / maxRange) * ControlSensitivity, (Distance / MaxRange) * ControlSensitivity);
        Mathf.Clamp(RandomY, 3f, (Distance / maxRange) * ControlSensitivity / 2);
        Vector3 ControlPoint = transform.position + new Vector3(RandomXZ, RandomY, RandomXZ);
        return ControlPoint;
    }

}
