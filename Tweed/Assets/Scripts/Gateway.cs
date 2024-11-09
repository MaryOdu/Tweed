using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Gateway : MonoBehaviour
{
    [SerializeField] float EndPos;
    [SerializeField] float ObSpeed = 0.2f;
    [SerializeField] bool DoorOpen = false;
    

    //[SerializeField] GameObject Door;

    //OpenDoor

    /* [SerializeField]
     public float rotateStair = 40.0f;
     [SerializeField]
     public  float RotateAnker = 10.0f;
     [SerializeField]
     public  float rotateNeg = -30.0f;


     public float TravelSpeed = 30.0f; // Adjust the speed in degrees per second.
     private Vector3 StartPos;
     private Vector3 RightDoorPos;
     private Vector3 LeftDoorPos;


     [SerializeField] bool DoorRight;
     [SerializeField] bool DoorLeft;
     [SerializeField] bool AnkerF;

     void Start()
     {
         StartPos = transform.position;
         //RightDoorRot = transform.position * Quaternion.Euler(90, 0, 0);
         RightDoorPos = transform.position * Vector3.(10, 0, 0);
         LeftDoorPos = transform.position * Vector3.(-10, 0, 0);
     }*/


    private void Update()
    {



        // if (AnkerF){transform.RotateAround(transform.position, Vector3.up, RotateAnker * Time.deltaTime); }

        


        if (DoorOpen == true)
        {
            float currentX = Mathf.Lerp(transform.position.x, EndPos, ObSpeed * Time.deltaTime);
            transform.position = new Vector3(currentX, transform.position.y, transform.position.z);
        }

    }

    public void Open()
    {
        DoorOpen = true;
    }
 
    /*IEnumerator RotateRightDoor()
    {
        float journeyLength = Vector3.Angle(StartPos, RightDoorPos);
        float startTime = Time.time;

        while (Time.time < startTime + journeyLength / TravelSpeed)
        {
            float journeyCovered = (Time.time - startTime) * TravelSpeed;
            float journeyFraction = journeyCovered / journeyLength;
            transform.position = Vector3.Slerp(StartPos, RightDoorPos, journeyFraction);
            yield return null;
        }

        transform.position = RightDoorPos; // Ensure we end up with the exact target rotation.
    }
    IEnumerator RotateLeftDoor()
    {
        float journeyLength = Vector3.Angle(StartPos, LeftDoorPos);
        float startTime = Time.time;

        while (Time.time < startTime + journeyLength / TravelSpeed)
        {
            float journeyCovered = (Time.time - startTime) * TravelSpeed;
            float journeyFraction = journeyCovered / journeyLength;
            transform.position = Vector3.Slerp(StartPos, LeftDoorPos, journeyFraction);
            yield return null;
        }

        transform.position = LeftDoorPos; // Ensure we end up with the exact target rotation.
    }*/
}


