using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //[SerializeField] private Rigidbody PlayerBody;


    //Player Movement speed
    public static float speed = 5f;
    public static float rotationSpeed = 700f;
    //private Vector3 PlayerMovementInput;

    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

   
    private void FixedUpdate()
    {


        //Player Movement
        /*PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));*/

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 PlayerMovementInput = new Vector3(horizontalInput, 0, verticalInput);
        PlayerMovementInput.Normalize();

        transform.Translate(PlayerMovementInput * speed * Time.deltaTime, Space.World);

        if (PlayerMovementInput != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(PlayerMovementInput, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        //MovePlayer();
    }

    private void MovePlayer()
    {
        /*Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput) * speed;
        PlayerBody.velocity = new Vector3(MoveVector.x, PlayerBody.velocity.y, MoveVector.z);*/

       
    }
}
