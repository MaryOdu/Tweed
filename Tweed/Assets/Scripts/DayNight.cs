using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DayNight : MonoBehaviour
{

   
    [SerializeField] GameObject LightStartRot;
  
    private float RotateSpeed = 1f;

   
    private void Start()
    {


    }
    void LateUpdate()
    {

        LightStartRot.transform.Rotate(Vector3.right, RotateSpeed * Time.deltaTime);

       

    }
}
