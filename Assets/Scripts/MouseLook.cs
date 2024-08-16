using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    private AudioSource m_AudioSource;
    public AudioClip ESC_AudioClip;

    public Transform playerBody;

    float xRotation = 0f;

    public GameObject UICanvas;
    public GameObject SoundSettingsButton;
    public GameObject SoundSettingsPanel;
    public bool PlayerWantsToRespawn = false;
    //public float minRotation = 25f;
    //public float maxRotation = 50f;

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        //xRotation = Mathf.Clamp(xRotation, minRotation, maxRotation);

        //transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up, mouseX);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (PlayerWantsToRespawn) return;
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            m_AudioSource.PlayOneShot(ESC_AudioClip);
            UICanvas.SetActive(!UICanvas.activeSelf);
            SoundSettingsButton.SetActive(UICanvas.activeSelf);
            SoundSettingsPanel.SetActive(false);
        }
    }
}
