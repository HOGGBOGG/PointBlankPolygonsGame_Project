using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip TurretUpgradeClip;
    public AudioClip PlayerWeaponUpgradeClip;
    public AudioClip InsufficientFundsClip;
    public AudioClip MaxLevelAchievedClip;
    public AudioClip UpgradeSuccessfulClip;


    [Header("Base Turret")]
    private int _currentTurretLevel = 0;
    public TextMeshProUGUI baseTurretlvlIndicator;
    public TextMeshProUGUI baseTurretCostIndicator;
    public TextMeshProUGUI baseTurretUpgradeIndicator;

    [Header("Lvl1 Turrets")]
    public List<GameObject> lvl1_Turrets = new List<GameObject>();

    [Header("Lvl2 Turrets")]
    public List<GameObject> lvl2_Turrets = new List<GameObject>();

    [Header("Lvl3 Turrets")]
    public List<GameObject> lvl3_Turrets = new List<GameObject>();


    private Dictionary<int,List<GameObject>> mappedTurrets = new Dictionary<int,List<GameObject>>();
    public int[] baseTurretCost = new int[3];

    public GameObject DialogBox;
    private Coroutine DialogCoroutine = null;
    private void InitialiseBaseDefenceAndTurrets()
    {
        // turret defence
        mappedTurrets.Add(1, lvl1_Turrets);
        mappedTurrets.Add(2, lvl2_Turrets);
        mappedTurrets.Add(3, lvl3_Turrets);

        // base defence
        mappedbaseTurrets.Add(1, lvl1_baseTurrets);
        mappedbaseTurrets.Add(2, lvl2_baseTurrets);
        mappedbaseTurrets.Add(3, lvl3_baseTurrets);
    }

    private void InitialiseTexts()
    {
        baseTurretCostIndicator.text = baseTurretCost[0].ToString() + " Points";
        baseShieldLvlIndicator.text = "Current Lv: 0";
        baseShieldCostIndicator.text = baseShieldCost.ToString() + " Points";
        PlayerShieldLvlIndicator.text = "Current Lv: 0";
        PlayerShieldCostIndicator.text = baseShieldCost.ToString() + " Points";
        PlayerWeaponLvlIndicator.text = "Current Lv: 0";
        PlayerWeaponCostIndicator.text = playerWeaponCost[0].ToString() + " Points";
        BaseDefenceLvlIndicator.text = "Current Lv: 0";
        BaseDefenceCostIndicator.text = baseDefenceCost[0].ToString() + " Points";
        PlayerRegenLvlIndicator.text = "Current Lv: 0";
        PlayerRegenCostIndicator.text = PlayerRegenCost[0].ToString() + " Points";
    }

    [Header("Base Shield")]
    public GameObject baseShield;
    public int baseShieldCost = 100;
    public TextMeshProUGUI baseShieldLvlIndicator;
    public TextMeshProUGUI baseShieldCostIndicator;
    private bool alreadyBoughtbaseShield = false;

    [Header("Player Shield")]
    public GameObject playerShield;
    public int playerShieldCost = 100;
    public TextMeshProUGUI PlayerShieldLvlIndicator;
    public TextMeshProUGUI PlayerShieldCostIndicator;
    private bool alreadyBoughtPlayerShield = false;

    [Header("Player Automatic Turrets")]
    private int currentPlayerAutoTurretlvl = 0;
    public GameObject[] playerAutoTurrets = new GameObject[2];
    public int[] playerAutoTurretsCost = new int[2];
    public TextMeshProUGUI PlayerAutoTurretLvlIndicator;
    public TextMeshProUGUI PlayerAutoTurretCostIndicator;

    [Header("Player Weapons")]
    private int currentPlayerWeaponlvl = 0;
    public GameObject PlayerSniperLvl2;
    public int[] peletCountperLvl = new int[5];
    public float[] spreadperLvl = new float[5];
    public int[] playerWeaponCost = new int[5];
    public TextMeshProUGUI PlayerWeaponLvlIndicator;
    public TextMeshProUGUI PlayerWeaponCostIndicator;
    public PlayerAction playerAction;

    [Header("Base Defences")]
    private int currentBaseDefencelvl = 0;
    public int[] baseDefenceCost = new int[3];
    public TextMeshProUGUI BaseDefenceLvlIndicator;
    public TextMeshProUGUI BaseDefenceCostIndicator;
    private Dictionary<int, List<GameObject>> mappedbaseTurrets = new Dictionary<int, List<GameObject>>();

    [Header("Lvl1 BaseDefence")]
    public List<GameObject> lvl1_baseTurrets = new List<GameObject>();

    [Header("Lvl2 BaseDefence")]
    public List<GameObject> lvl2_baseTurrets = new List<GameObject>();


    [Header("Lvl3 BaseDefence")]
    public List<GameObject> lvl3_baseTurrets = new List<GameObject>();


    [Header("Lvl3 PlayerRegen")]
    public Player player;
    public int[] PlayerRegenCost = new int[3];
    public float[] PlayerRegenAmountPerLvl = new float[3];
    public TextMeshProUGUI PlayerRegenLvlIndicator;
    public TextMeshProUGUI PlayerRegenCostIndicator;
    private int _currentPlayerRegenLvl = 0;
    public AudioClip PlayerRegenUpgradeClip;


    void Start()
    {
        InitialiseBaseDefenceAndTurrets();
        InitialiseTexts();
        ChangeVolume(0.5f);
    }

    private void PlayAudioSource(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void BuyBaseShield()
    {
        if (alreadyBoughtbaseShield) { ShowMessage("Maximum level achieved.",InsufficientFundsClip); return; }
        if (GameManager.Instance.points < baseShieldCost)
        {
            ShowMessage("Insufficient points",InsufficientFundsClip);
            return;
        }
        GameManager.Instance.DeductPoints(baseShieldCost);
        baseShield.SetActive(true);
        ShowMessage("Upgrade Successful", UpgradeSuccessfulClip);
        baseShieldCostIndicator.text = "MAX LEVEL";
        baseShieldLvlIndicator.text = "MAX LEVEL";
        alreadyBoughtbaseShield = true;
    }

    public void UpgradePlayerAutoTurrets()
    {
        if (currentPlayerAutoTurretlvl == 2) { ShowMessage("Maximum level achieved.", InsufficientFundsClip); return; }
        if (GameManager.Instance.points < playerAutoTurretsCost[currentPlayerAutoTurretlvl])
        {
            ShowMessage("Insufficient points", InsufficientFundsClip);
            return;
        }
        if(currentPlayerAutoTurretlvl < 1)
        {
            ShowMessage("Upgrade Successful", PlayerWeaponUpgradeClip);
            GameManager.Instance.DeductPoints(playerAutoTurretsCost[currentPlayerAutoTurretlvl]);
            PlayerAutoTurretCostIndicator.text = playerAutoTurretsCost[currentPlayerAutoTurretlvl+1].ToString() + " Points";
            playerAutoTurrets[currentPlayerAutoTurretlvl].SetActive(true);
            currentPlayerAutoTurretlvl++;
            PlayerAutoTurretLvlIndicator.text = "Current Lv: " + currentPlayerAutoTurretlvl.ToString();
        }
        else
        {
            PlayerAutoTurretLvlIndicator.text = "MAX LEVEL";
            PlayerAutoTurretCostIndicator.text = "MAX LEVEL";
            ShowMessage("Maximum level achieved.", MaxLevelAchievedClip);
            GameManager.Instance.DeductPoints(playerAutoTurretsCost[currentPlayerAutoTurretlvl]);
            playerAutoTurrets[currentPlayerAutoTurretlvl].SetActive(true);
            currentPlayerAutoTurretlvl++;
        }
    }

    public void UpgradePlayerWeapon()
    {
        if (currentPlayerWeaponlvl == 5) { ShowMessage("Maximum level achieved.", InsufficientFundsClip); return; }
        if (GameManager.Instance.points < playerWeaponCost[currentPlayerWeaponlvl])
        {
            ShowMessage("Insufficient points", InsufficientFundsClip);
            return;
        }
        if (currentPlayerWeaponlvl < 4)
        {
            ShowMessage("Upgrade Successful", PlayerWeaponUpgradeClip);
            GameManager.Instance.DeductPoints(playerWeaponCost[currentPlayerWeaponlvl]);
            PlayerWeaponCostIndicator.text = playerWeaponCost[currentPlayerWeaponlvl + 1].ToString() + " Points";
            playerAction.pelets = peletCountperLvl[currentPlayerWeaponlvl];
            playerAction.ShotgunSpread = spreadperLvl[currentPlayerWeaponlvl];
            currentPlayerWeaponlvl++;
            PlayerWeaponLvlIndicator.text = "Current Lv: " + currentPlayerWeaponlvl.ToString();
        }
        else
        {
            PlayerWeaponLvlIndicator.text = "MAX LEVEL";
            PlayerWeaponCostIndicator.text = "MAX LEVEL";
            ShowMessage("Maximum level achieved.", MaxLevelAchievedClip);
            GameManager.Instance.DeductPoints(playerWeaponCost[currentPlayerWeaponlvl]);
            PlayerSniperLvl2.SetActive(true);
            playerAction.useSniper = true;
            currentPlayerWeaponlvl++;
        }
    }


    public void BuyPlayerShield()
    {
        if (alreadyBoughtPlayerShield) { ShowMessage("Maximum level achieved.", InsufficientFundsClip); return; }
        if (GameManager.Instance.points < playerShieldCost)
        {
            ShowMessage("Insufficient points", InsufficientFundsClip);
            return;
        }
        GameManager.Instance.DeductPoints(playerShieldCost);
        playerShield.SetActive(true);
        ShowMessage("Bought Player Shield 1000 HP", UpgradeSuccessfulClip);
        PlayerShieldCostIndicator.text = "MAX LEVEL";
        PlayerShieldLvlIndicator.text = "MAX LEVEL";
        alreadyBoughtPlayerShield = true;
    }


    private void ShowMessage(string message,AudioClip audioClip)
    {
        if(DialogCoroutine == null)
        {
            DialogCoroutine = StartCoroutine(ShowDialog(message,audioClip));
        }
        else
        {
            StopCoroutine(DialogCoroutine);
            DialogCoroutine = null;
            DialogCoroutine = StartCoroutine(ShowDialog(message, audioClip));
        }
    }

    private IEnumerator ShowDialog(string message,AudioClip audioClip)
    {
        PlayAudioSource(audioClip);
        DialogBox.SetActive(true);
        DialogBox.GetComponentInChildren<TextMeshProUGUI>().text = message;
        yield return new WaitForSeconds(2f);
        DialogBox.SetActive(false);
        DialogCoroutine = null;
    }


    public void UpgradeBaseTurret()
    {
        if (_currentTurretLevel == 4) { ShowMessage("Maximum level achieved.", InsufficientFundsClip); return; }
        if (GameManager.Instance.points < baseTurretCost[_currentTurretLevel])
        {
            ShowMessage("Insufficient points", InsufficientFundsClip);
            return;
        }
        if (_currentTurretLevel < 3)
        {
            _currentTurretLevel++;
            foreach(GameObject obj in mappedTurrets[_currentTurretLevel])
            {
                obj.SetActive(true);
            }
            GameManager.Instance.DeductPoints(baseTurretCost[_currentTurretLevel - 1]);
            baseTurretlvlIndicator.text = "Current Lv: " + _currentTurretLevel;
            baseTurretCostIndicator.text = baseTurretCost[_currentTurretLevel].ToString() + " Points";
            ShowMessage("Upgrade Successful", TurretUpgradeClip);
            if(_currentTurretLevel == 3)
            {
                baseTurretUpgradeIndicator.text = "Turret Range++";
            }
        }
        else
        {
            _currentTurretLevel++;
            //foreach (GameObject obj in mappedTurrets[_currentTurretLevel])
            //{
            //    obj.SetActive(true);
            //}
            for (int i = 1; i < _currentTurretLevel; i++)
            {
                foreach (GameObject obj in mappedTurrets[i])
                {
                    SphereCollider sphereC = obj.GetComponent<SphereCollider>();
                    sphereC.radius = 1000f;
                }
            }
            GameManager.Instance.DeductPoints(baseTurretCost[_currentTurretLevel - 1]);
            baseTurretlvlIndicator.text = "MAX LEVEL";
            baseTurretCostIndicator.text = "MAX LEVEL";
            ShowMessage("Maximum level achieved.", MaxLevelAchievedClip);
        }
    }

    public void UpgradeBaseDefence()
    {
        if (currentBaseDefencelvl == 3) { ShowMessage("Maximum level achieved.", InsufficientFundsClip); return; }
        if (GameManager.Instance.points < baseDefenceCost[currentBaseDefencelvl])
        {
            ShowMessage("Insufficient points", InsufficientFundsClip);
            return;
        }
        if (currentBaseDefencelvl < 2)
        {
            currentBaseDefencelvl++;
            foreach (GameObject obj in mappedbaseTurrets[currentBaseDefencelvl])
            {
                obj.SetActive(true);
            }
            GameManager.Instance.DeductPoints(baseDefenceCost[currentBaseDefencelvl - 1]);
            BaseDefenceLvlIndicator.text = "Current Lv: " + currentBaseDefencelvl;
            BaseDefenceCostIndicator.text = baseDefenceCost[currentBaseDefencelvl].ToString() + " Points";
            ShowMessage("Upgrade Successful", TurretUpgradeClip);
        }
        else
        {
            currentBaseDefencelvl++;
            foreach (GameObject obj in mappedbaseTurrets[currentBaseDefencelvl])
            {
                obj.SetActive(true);
            }
            GameManager.Instance.DeductPoints(baseDefenceCost[currentBaseDefencelvl - 1]);
            BaseDefenceLvlIndicator.text = "MAX LEVEL";
            BaseDefenceCostIndicator.text = "MAX LEVEL";
            ShowMessage("Maximum level achieved.", MaxLevelAchievedClip);
        }
    }

    public void UpgradePlayerRegen()
    {
        if (_currentPlayerRegenLvl == 3) { ShowMessage("Maximum level achieved.", InsufficientFundsClip); return; } // 0 1 2 3
        if (GameManager.Instance.points < PlayerRegenCost[_currentPlayerRegenLvl])
        {
            ShowMessage("Insufficient points", InsufficientFundsClip);
            return;
        }
        if (_currentPlayerRegenLvl < 2)
        {
            _currentPlayerRegenLvl++;
            GameManager.Instance.DeductPoints(PlayerRegenCost[_currentPlayerRegenLvl - 1]);
            player.RegenAmount = PlayerRegenAmountPerLvl[_currentPlayerRegenLvl - 1];
            PlayerRegenLvlIndicator.text = "Current Lv: " + _currentPlayerRegenLvl;
            PlayerRegenCostIndicator.text = PlayerRegenCost[_currentPlayerRegenLvl].ToString() + " Points";
            ShowMessage("Upgrade Successful", PlayerRegenUpgradeClip);
        }
        else
        {
            _currentPlayerRegenLvl++;
            player.RegenAmount = PlayerRegenAmountPerLvl[_currentPlayerRegenLvl - 1];
            GameManager.Instance.DeductPoints(PlayerRegenCost[_currentPlayerRegenLvl - 1]);
            PlayerRegenLvlIndicator.text = "MAX LEVEL";
            PlayerRegenCostIndicator.text = "MAX LEVEL";
            ShowMessage("Maximum level achieved.", PlayerRegenUpgradeClip);
        }
    }

    public TextMeshProUGUI volume_textValue;
    public GameObject volume_Panel;
    public GameObject volume_btn_GameObject;

    public void ChangeVolume(float volume)
    {
        AudioListener.volume = volume;
        float val = volume * 100f;
        volume_textValue.text = val.ToString("0.0");
    }

    public void OpenVolumeSettings()
    {
        volume_btn_GameObject.SetActive(false);
        volume_Panel.SetActive(true);
    }

    public void CloseVolumeSettings()
    {
        volume_Panel.SetActive(false);
        volume_btn_GameObject.SetActive(true);
    }
}
