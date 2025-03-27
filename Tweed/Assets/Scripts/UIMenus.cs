using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenus : MonoBehaviour
{
    
    [SerializeField] string MainMenu = "Main Menu";
    [SerializeField] string MainScenes = "MainGameScene";
    [SerializeField] string Testing = "AnimationTesting";
    [SerializeField] string Prototype_Level = "EnemyAgent_Test";
    public MMAudioCon MMMusic;
    public MGAudioCon MGMusic;


    //[SerializeField] GameObject WonGame;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject CaughtMenu;
    [SerializeField] float RespawnTime = 3f;
    private bool Caught = false;
    //private bool WonTrue = false;

<<<<<<< HEAD

    void Start()
    {
=======
    /*private int width = 1920; 
    private int height = 700; 
    private bool isFullScreen = true;


    private void Awake()
    {
        Screen.SetResolution(width, height, isFullScreen);
    }*/

    void Start()
    {

       
>>>>>>> 1a0816a (Menu fix, turrain pain and texture fix)
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

    public void LoadMainMenu(string MainMenu){SceneManager.LoadScene("MainMenu");}
    // MMMusic.PlayMainMenu();
    public void LoadMainGameScenes(string MainScenes) {SceneManager.LoadScene("MainScenes"); MMMusic.PuaseMainMenu(); MGMusic.PlayGameAmbiance(); }
    public void LoadAnimationTesting(string Testing){SceneManager.LoadScene("Testing");}
    public void LoadPrototype_Level(string Prototype_Level) {SceneManager.LoadScene("Prototype_Level");}
    public void LoadLeaveGame(string ExitGame){Application.Quit();}

}
