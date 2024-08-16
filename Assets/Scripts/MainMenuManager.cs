using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string SceneName;
    public GameObject MainMenu;
    public GameObject ControlsPanelGameObject;

    public void StartGame()
    {
        SceneManager.LoadScene(SceneName);
    }


    public void ControlsClick()
    {
        ControlsPanelGameObject.SetActive(true);
        MainMenu.SetActive(false);
    }

    public void BackButton()
    {
        ControlsPanelGameObject.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
