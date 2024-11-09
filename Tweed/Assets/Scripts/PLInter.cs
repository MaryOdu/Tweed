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
    // public GameObject Door;

    public Gateway happen;
    public Gateway happen2;
    void Start()
    {
        
    }

   
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            IntText.SetActive(true);
            interactable = true;

        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            IntText.SetActive(false);
            interactable = false;
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

    }
}
