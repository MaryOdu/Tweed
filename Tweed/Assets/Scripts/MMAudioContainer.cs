using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioContainer : MonoBehaviour
{
    public AudioSource ButtonGoSound;
    public AudioSource MainMenuAmb;

    //Make audio scripts for player and mobs. Call the audio in these scripts
    //In the player/mobs call their respective audio scrips and when action performs call the named function in them to play

    
    public void PlayButtonGoSound()
    {
        ButtonGoSound.Play();
    }
    public void PauseAmbiantSound()
    {
        ButtonGoSound.Pause();
    }

    public void PlayMainMenu()
    {
        MainMenuAmb.Play();
    }
    public void PuaseMainMenu()
    {
        MainMenuAmb.Pause();
    }

    // Start is called before the first frame update
    void Start()
    {
       

    }

    // Update is called once per frame
    void Update()
    {
        //AmbiantSound.GetComponent<PlayerMovement>().AmbiantS();
       // WalkingSound.GetComponent<PlayerMovement>().WalkingS();
        //EatingSound.GetComponent<PlayerAnimations>().EatS();
    }
}
