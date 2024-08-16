using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
public class GunScriptableObject : ScriptableObject
{
    public float DamageRadius = 2f;

    [Header("Handgun(To be upgraded to shotgun)")]
    public GunType Type;
    public string Name;
    public GameObject ModelPrefab;
    public Vector3 SpawnPoint;
    public Vector3 SpawnRotation;
    public AudioConfigScriptableObject ShotgunAudioConfig;

    [Header("Rifle")]
    public GameObject SniperModelPrefab;
    public Vector3 Sniper1SpawnPoint;
    public Vector3 Sniper1SpawnRotation;
    public Vector3 Sniper2SpawnPoint;
    public Vector3 Sniper2SpawnRotation;
    private ParticleSystem Sniper1ShootSystem;
    private ParticleSystem Sniper2ShootSystem;
    private AudioSource SniperAudioSource;
    public ShootConfigScriptableObject SniperShootConifg;
    public AudioConfigScriptableObject SniperAudioConfig;
    public DamageConfigScriptableObject SniperDamageConfig;
    private float SniperLastShootTime;
    private ParticleSystem SniperSHOOTsystem;

    [Header("Others")]
    public ShootConfigScriptableObject ShootConfig;
    public TrailConfigScriptableObject TrailConfig;
    public AmmoConfigScriptableObject AmmoConfig;
    public AudioConfigScriptableObject AudioConfig;
    public DamageConfigScriptableObject DamageConfig;

    private MonoBehaviour ActiveMonoBehaviour;
    private GameObject Model;
    private float LastShootTime;
    private ParticleSystem ShootSystem;
    private ObjectPool<TrailRenderer> TrailPool;
    private AudioSource ShootingAudioSource;


    public void Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour)
    {
        this.ActiveMonoBehaviour = ActiveMonoBehaviour;
        LastShootTime = 0; // in editor this will not be properly reset, in build it's fine
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        Model = Instantiate(ModelPrefab);
        Model.transform.SetParent(Parent, false);
        Model.transform.localPosition = SpawnPoint;
        Model.transform.localRotation = Quaternion.Euler(SpawnRotation);

        ShootSystem = Model.GetComponentInChildren<ParticleSystem>(); // Should be present in the prefab
        ShootingAudioSource = Model.GetComponentInChildren<AudioSource>(); // Should be present in the prefab

        //For Snipers

        SniperLastShootTime = 0;

        GameObject SniperModel1 = Instantiate(SniperModelPrefab);
        GameObject SniperModel2 = Instantiate(SniperModelPrefab);
        SniperModel1.transform.SetParent(Parent, false);
        SniperModel2.transform.SetParent(Parent, false);
        SniperModel1.transform.localPosition = Sniper1SpawnPoint;
        SniperModel2.transform.localPosition = Sniper2SpawnPoint;
        SniperModel1.transform.localRotation = Quaternion.Euler(Sniper1SpawnRotation);
        SniperModel2.transform.localRotation = Quaternion.Euler(Sniper2SpawnRotation);

        Sniper1ShootSystem = SniperModel1.GetComponentInChildren<ParticleSystem>();
        Sniper2ShootSystem = SniperModel2.GetComponentInChildren<ParticleSystem>();
        SniperAudioSource = SniperModel1.GetComponentInChildren<AudioSource>();

    }

    public void Shoot()
    {
        if (Time.time > ShootConfig.FireRate + LastShootTime)
        {
            if (AmmoConfig.CurrentClipAmmo == 0)
            {
                AudioConfig.PlayOutOfAmmoClip(ShootingAudioSource);
                return;
            }
                LastShootTime = Time.time;
            ShootSystem.Play();
            AudioConfig.PlayShootingClip(ShootingAudioSource, AmmoConfig.CurrentClipAmmo == 1);
            AmmoConfig.CurrentClipAmmo--;
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

            if (Physics.Raycast(
                    ShootSystem.transform.position,
                    shootDirection,
                    out RaycastHit hit,
                    float.MaxValue,
                    ShootConfig.HitMask
                ))
            {
                ActiveMonoBehaviour.StartCoroutine(
                    PlayTrail(
                        ShootSystem.transform.position,
                        hit.point,
                        hit,
                        false
                    )
                );
            }
            else
            {
                ActiveMonoBehaviour.StartCoroutine(
                    PlayTrail(
                        ShootSystem.transform.position,
                        ShootSystem.transform.position + (shootDirection * TrailConfig.MissDistance),
                        new RaycastHit(),
                        false
                    )
                );
            }
        }
    }

    public void SniperShoot()
    {
        if (Time.time > SniperShootConifg.FireRate + SniperLastShootTime)
        {
            if (AmmoConfig.CurrentClipAmmo == 0)
            {
                AudioConfig.PlayOutOfAmmoClip(ShootingAudioSource);
                return;
            }
            SniperLastShootTime = Time.time;
            SniperAudioConfig.PlayShootingClip(ShootingAudioSource, AmmoConfig.CurrentClipAmmo == 1);
            for (int i = 0; i < 2; i++) {
                
                Vector3 shootDirection;
                Vector3 Origin;
                if (i == 0)
                {
                    SniperSHOOTsystem = Sniper1ShootSystem;
                    Origin = Sniper1ShootSystem.transform.position;
                    shootDirection = Sniper1ShootSystem.transform.forward;
                }
                else
                {
                    SniperSHOOTsystem = Sniper2ShootSystem;
                    Origin = Sniper2ShootSystem.transform.position;
                    shootDirection = Sniper2ShootSystem.transform.forward;
                }

                SniperSHOOTsystem.Play();
                shootDirection.Normalize();

                if (Physics.Raycast(
                        Origin,
                        shootDirection,
                        out RaycastHit hit,
                        float.MaxValue,
                        ShootConfig.HitMask
                    ))
                {
                    ActiveMonoBehaviour.StartCoroutine(
                        PlayTrail(
                            Origin,
                            hit.point,
                            hit,
                            true
                        )
                    );
                }
                else
                {
                    ActiveMonoBehaviour.StartCoroutine(
                        PlayTrail(
                            Origin,
                            Origin + (shootDirection * TrailConfig.MissDistance),
                            new RaycastHit(),
                            true
                        )
                    );
                }
            }
        }
    }

    public void ShotGunShoot(int pelets,float ShotgunSpread)
    {
        if (Time.time > ShootConfig.FireRate + LastShootTime)
        {
            LastShootTime = Time.time;
            ShootSystem.Play();
            ShotgunAudioConfig.PlayShootingClip(ShootingAudioSource, AmmoConfig.CurrentClipAmmo == 1);
            AmmoConfig.CurrentClipAmmo--;
            for (int i = -pelets / 2; i <= pelets / 2; i++)
            {
                float angle1 = (ShotgunSpread / pelets) * i;
                Vector3 trailDirection = Quaternion.Euler(0f, angle1, 0f) * ShootSystem.transform.forward;
       
                trailDirection.Normalize();

                if (Physics.Raycast(
                        ShootSystem.transform.position,
                        trailDirection,
                        out RaycastHit hit,
                        float.MaxValue,
                        ShootConfig.HitMask
                    ))
                {
                    ActiveMonoBehaviour.StartCoroutine(
                        PlayTrail(
                            ShootSystem.transform.position,
                            hit.point,
                            hit,
                            false
                        )
                    );
                }
                else
                {
                    ActiveMonoBehaviour.StartCoroutine(
                        PlayTrail(
                            ShootSystem.transform.position,
                            ShootSystem.transform.position + (trailDirection * TrailConfig.MissDistance),
                            new RaycastHit(),
                            false
                        )
                    );
                }
        }
            
        }
    }

    private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit Hit,bool SniperShot)
    {
        TrailRenderer instance = TrailPool.Get();
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

        if(Hit.collider != null)
        {
            if (Hit.collider.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                if (SniperShot)
                {
                    damageable.TakeDamage(SniperDamageConfig.GetDamage(distance));
                }
                else
                {
                    damageable.TakeDamage(DamageConfig.GetDamage(distance));
                }
            }
        }

        yield return new WaitForSeconds(TrailConfig.Duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        TrailPool.Release(instance);
    }

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

    public bool CanReload()
    {
        return AmmoConfig.CanReload();
    }

    public void EndReload()
    {
        AmmoConfig.Reload();
    }
    public void StartReloading()
    {
        AudioConfig.PlayReloadClip(ShootingAudioSource);
    }
}