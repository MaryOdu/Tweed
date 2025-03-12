using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPortal : MonoBehaviour
{

    [SerializeField] GameObject VortexRot;
    [SerializeField] GameObject WonUI;
    [SerializeField] float TimeToEnd = 3f;
    private bool End;


    private float RotateSpeed = 80f;
    // Start is called before the first frame update
    void Start()
    {
        WonUI.SetActive(false);
        End = false;
    }

    // Update is called once per frame
    void Update()
    {
        VortexRot.transform.Rotate(Vector3.up, RotateSpeed * Time.deltaTime);

        if (End == true)
        {
            TimeToEnd -= Time.deltaTime;
            if (TimeToEnd <= 0.1f)
            {

                SceneManager.LoadScene("MainMenu");
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Andy - OnCrabsBack")
        {
            WonUI.SetActive(true);
            End = true;
         
        }
    }
}
