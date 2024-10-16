using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLInter : MonoBehaviour
{

    public GameObject IntText;
    public bool interactable;
    
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
}
