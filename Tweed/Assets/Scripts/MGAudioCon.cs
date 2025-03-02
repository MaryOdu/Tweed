using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGAudioCon : MonoBehaviour
{
    [SerializeField] AudioSource GoButton;
    [SerializeField] AudioSource GameAmbiance;
    // Start is called before the first frame update

    public void PlayButtonSound(){ GoButton.Play(); }
    public void PauseButtonSound() { GoButton.Pause(); }

    public void PlayGameAmbiance() { GameAmbiance.Play(); }
    public void PauseGameAmbiance() { GameAmbiance.Pause(); }


    void Start()
    {
        GoButton.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
