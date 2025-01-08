using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenus : MonoBehaviour
{

    [SerializeField] string MainMenu = "Main Menu";
    [SerializeField] string MapScene = "MainScenes";
    [SerializeField] string PlayerScene = "Testing";
    [SerializeField] string NPCScene = "EnemyAgent_Test";

    //[SerializeField] GameObject WonGame;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject CaughtMenu;
    [SerializeField] float RespawnTime = 3f;
    private bool Caught = false;
    //private bool WonTrue = false;


    void Start()
    {
        //too be an audio function call
        if (CaughtMenu != null)
        {
            CaughtMenu.SetActive(false);
        }

        if (PauseMenu != null)
        {
            PauseMenu.SetActive(false);
        }
        //WonGame.SetActive(false);
    }

    private void Update()
    {
        /*if (WonTrue == true)
        {
            EndTime -= Time.deltaTime;
            if (EndTime <= 0)
            {
                SceneManager.LoadScene("MainMenu");
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }*/
        if (CaughtMenu != null && Caught == true)
        {
            CaughtMenu.SetActive(true);
            RespawnTime -= Time.deltaTime;

            if (RespawnTime <= 0)
            {
                CaughtMenu.SetActive(false);
                RespawnTime = 3f;
                Caught = false;
            }
        }
    }

    public void CaughtScreen()
    {
        Debug.Log("Player was caught: UI Script");
        Caught = true;
    }
    public void Pause()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void Continue()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void LoadMainMenu(string MainMenu)
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void LoadPlayerScene(string Testing)
    {
        SceneManager.LoadScene("Testing");
    }

    public void LoadMainScenes(string Testing)
    {
        SceneManager.LoadScene("Prototype_Level");
    }

    public void LoadLeaveGame(string ExitGame)
    {
        Application.Quit();
    }

}
