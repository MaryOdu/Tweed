using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody PlayerBody;

    //public float rotationSpeed;
    private Vector3 turn;
    public float mouseSensitivity = .3f;

    //Player Movement speed
    public static float speed = 5f;
    private Vector3 PlayerMovementInput;

    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

   
    private void FixedUpdate()
    {
        //Player rotation by mouse
        turn.x += Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.localRotation = Quaternion.Euler(0, turn.x, 0);

        //Player Movement
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput) * speed;
        PlayerBody.velocity = new Vector3(MoveVector.x, PlayerBody.velocity.y, MoveVector.z);
    }
}
