using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public TextMeshProUGUI PointsText;
    public static int MaxEnemiesPerTeam = 15;
    public int CurrentNumberEnemiesAllyTeam = 0;
    public int CurrentNumberEnemiesHostileTeam = 0;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        Destroy(gameObject);
    }

    private void Start()
    {
        UpdatePoints();
    }

    public int points = 0;

    public void AddPoints(int value)
    {
        points += value;
        UpdatePoints();
    }

    public void DeductPoints(int value)
    {
        points -= value;
        UpdatePoints();
    }

    public void UpdatePoints()
    {
        PointsText.text = points.ToString();
    }

    public GameObject HostileBase;
    public GameObject AllyBase;
    public GameObject GameWinPanel;
    public GameObject GameLosePanel;
    public MouseLook mouseLook;
    private void Update()
    {
        if (!HostileBase.activeInHierarchy)
        {
            GameWin();
        }
        if (!AllyBase.activeInHierarchy)
        {
            GameLose();
        }
    }

    private Coroutine EndingGame;
    public void GameWin()
    {
        Cursor.lockState = CursorLockMode.None;
        mouseLook.PlayerWantsToRespawn = true;
        if (EndingGame == null) {
            EndingGame = StartCoroutine(EndGame(GameWinPanel));
        }
    }

    public void GameLose()
    {
        Cursor.lockState = CursorLockMode.None;
        mouseLook.PlayerWantsToRespawn = true;
        if (EndingGame == null)
        {
            EndingGame = StartCoroutine(EndGame(GameLosePanel));
        }
    }

    private IEnumerator EndGame(GameObject gmj)
    {
        yield return new WaitForSeconds(3f);
        gmj.SetActive(true);
    }

    public void Replay()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public GameObject ReloadGameObj;

    public void QuitGame()
    {
        ReloadGameObj.SetActive(true);
        //Application.Quit();
    }
}