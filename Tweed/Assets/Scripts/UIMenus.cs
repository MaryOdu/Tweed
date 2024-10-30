using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMenus : MonoBehaviour
{

    public string MainMenu = "Main Menu";
    public string MapScene = "MainScenes";
    public string PlayerScene = "Testing";
    public string NPCScene = "EnemyAgent_Test";

    void Start()
    {
        //too be an audio function call
    }

    public void LoadMainMenu(string MainMenu)
    {
        SceneManager.LoadScene("Main Menu");
    }
    public void LoadPlayerScene(string Testing)
    {
        SceneManager.LoadScene("Testing");
    }

}
