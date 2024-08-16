using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private PlayerGunSelector GunSelector;
    [SerializeField]
    private bool AutoReload = false;
    [SerializeField]
    private bool IsReloading = false;
    [SerializeField]
    public float timeToReload = 1.5f;
    //[SerializeField]
    //private Animator PlayerAnimator;
    private Coroutine ReloadCoroutine = null;

    public bool useShotgun = false;
    public int pelets = 5;
    public float ShotgunSpread = 60f;
    public bool useSniper = false;


    private void Update()
    {
        if (!IsReloading && Input.GetMouseButton(0)
            && GunSelector.ActiveGun != null)
        {
            if (useShotgun)
                GunSelector.ActiveGun.ShotGunShoot(pelets,ShotgunSpread);
            else
                GunSelector.ActiveGun.Shoot();
            if (useSniper)
                GunSelector.ActiveGun.SniperShoot();
            
        }
        if (ShouldManualReload() || ShouldAutoReload())
        {
            if (ReloadCoroutine == null)
            {
                IsReloading = true;
                GunSelector.ActiveGun.StartReloading();
                ReloadCoroutine = StartCoroutine(EndReload());
            }
            else
            {
                Debug.Log("Reload Coroutine is not null");
            }
        }
    }

    private bool ShouldManualReload()
    {
        return !IsReloading
            && Input.GetKeyUp(KeyCode.R)
            && GunSelector.ActiveGun.CanReload();
    }

    private bool ShouldAutoReload()
    {
        return !IsReloading
            && AutoReload
            && GunSelector.ActiveGun.AmmoConfig.CurrentClipAmmo == 0
            && GunSelector.ActiveGun.CanReload();
    }

    private IEnumerator EndReload()
    {
        yield return new WaitForSeconds(timeToReload);
        GunSelector.ActiveGun.EndReload();
        IsReloading = false;
        ReloadCoroutine = null;
    }

    //private void EndReload()
    //{
    //    GunSelector.ActiveGun.EndReload();
    //    IsReloading = false;
    //}
}