using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour
{
    public Health health;
    public GameObject ParentGameObject;
    public Vector3 RespawnPosition;
    public List<GameObject> gmjDisable = new List<GameObject>();
    public GameObject RespawnButton;
    public List<MeshRenderer> DisableMeshRenderers = new List<MeshRenderer>();
    public float RegenAmount = 0f;
    public GameObject ObjectToParentCamera;
    public GameObject Camera;
    public Vector3 InitialCameraPosition = Vector3.zero;
    public MouseLook mouseLook;
    public CharacterController characterController;
    public GameObject aiAgent_ToNotDisable;
    public GameObject ShieldGameObject;
    public AiSensor[] HostileBaseSensor;
    private bool hasDied = false;
    public GameObject MainMenu;
    public GameObject SoundSettings;
    public GameObject SoundButton;
    void Start()
    {
        health = GetComponent<Health>();
        health.OnTakeDamage += TakeDmg;
        health.OnDeath += Die;
        health.InvokeOnTakeDamage();
    }

    // Update is called once per frame
    void Update()
    {
        health.IncreaseCurrentPlayerHP(RegenAmount * Time.deltaTime);
        if (!hasDied)
        {
            if(ParentGameObject.transform.position.y < -5f)
            {
                hasDied = true;
                Die(Vector3.zero);
                StartCoroutine(RespawnPlayer());
            }
        }
    }


    private void TakeDmg(float dmg)
    {
    }


    private void Die(Vector3 pos)
    {
        foreach(MeshRenderer me in DisableMeshRenderers)
        {
            me.enabled = false;
        }
        foreach(GameObject c in gmjDisable)
        {
            c.SetActive(false);
        }
        characterController.enabled = false;
        StartCoroutine(RespawnPlayer());
    }


    private IEnumerator RespawnPlayer()
    {
        foreach(AiSensor AS in HostileBaseSensor)
        {
            AS.RemovePlayerDisabled(this.gameObject);
            AS.RemovePlayerDisabled(ShieldGameObject.gameObject);
        }
        ParentGameObject.transform.position = new Vector3(-1000, 0, 0);
        health.SetCurrentToMax();
        Camera.transform.SetParent(ObjectToParentCamera.transform, false);
        yield return new WaitForSeconds(5f);
        Cursor.lockState = CursorLockMode.None;
        mouseLook.PlayerWantsToRespawn = true;
        RespawnButton.SetActive(true);
    }

    public void Respawn()
    {
        aiAgent_ToNotDisable.SetActive(true);
        mouseLook.PlayerWantsToRespawn = false;
        Cursor.lockState = CursorLockMode.Locked;
        MainMenu.SetActive(false);
        SoundSettings.SetActive(false);
        RespawnButton.SetActive(false);
        SoundButton.SetActive(false);
        Camera.transform.SetParent(transform, false);
        Camera.transform.localPosition = InitialCameraPosition;
        health.SetCurrentToMax();
        health.InvokeOnTakeDamage();
        ParentGameObject.transform.position = RespawnPosition;
        foreach (MeshRenderer me in DisableMeshRenderers)
        {
            me.enabled = true;
        }
        foreach (GameObject c in gmjDisable)
        {
            c.SetActive(true);
        }
        characterController.enabled = true;
        hasDied = false;
    }


}
