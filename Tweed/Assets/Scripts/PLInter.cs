using Assets.Scripts.Enemy;
using Assets.Scripts.NPC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLInter : MonoBehaviour
{

    public GameObject IntText;
    [SerializeField] bool interactable;

    [SerializeField] bool InterOnce = false;
    [SerializeField] bool Gates;
    [SerializeField] bool TargetSwap;
    [SerializeField] AudioSource InteractingSound;

    [SerializeField]
    private NPCDirector npcDirector;
    private GameObject currentPlayer;

    public Gateway happen;
    public Gateway happen2;
    void Start()
    {
        
    }

   
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && (InterOnce == false))
        {
            IntText.SetActive(true);
            interactable = true;

            currentPlayer = other.gameObject;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IntText.SetActive(false);
            interactable = false;

            currentPlayer = other.gameObject;
        }
    }
    private void Update()
    {
        if (Gates == true)
        {
            if ((interactable == true) && (InterOnce == false))
            {

                if (Input.GetKeyDown(KeyCode.E))
                {
                    InteractingSound.Play();
                    InterOnce = true;
                    happen.Open(); 
                    happen2.Open();
                }
            }
        }
        if (TargetSwap == true)
        {
            if ((interactable == true) /*&& (InterOnce == false)*/)
            {

                if (Input.GetKeyDown(KeyCode.E))
                {
                    InterOnce = true;
                    InteractingSound.Play();

                    if (currentPlayer != null)
                    {
                        npcDirector.RemoveTarget(currentPlayer);
                    }
                    Debug.Log("change target");
                }
            }
        }
    }
}
