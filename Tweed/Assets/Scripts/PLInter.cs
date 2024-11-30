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
    //[SerializeField] GameObject DoorL;
    //[SerializeField] GameObject DoorR;
    [SerializeField] bool Gates;
    [SerializeField] bool TargetSwap;

    [SerializeField]
    private NPCDirector npcDirector;
    private GameObject currentPlayer;

    // public GameObject Door;

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

        //need to call only the one gate in question

        if (Gates == true)
        {
            if ((interactable == true) && (InterOnce == false))
            {

                if (Input.GetKeyDown(KeyCode.E))
                {
                    InterOnce = true;
                    happen.Open(); happen2.Open();

                }
            }
        }
        if (TargetSwap == true)
        {
            if ((interactable == true) && (InterOnce == false))
            {

                if (Input.GetKeyDown(KeyCode.E))
                {
                    InterOnce = true;

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
