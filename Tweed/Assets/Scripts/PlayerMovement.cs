using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Player Movement speed
    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float rotationSpeed = 700f;

    private Vector3 m_velocity;

    private Rigidbody m_rigidBody;

    [SerializeField] 
    private Transform CamDir;
    //private Vector3 PlayerMovementInput;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_velocity = Vector3.zero;

        m_rigidBody = GetComponent<Rigidbody>();
    }

   


    private void FixedUpdate()
    {


        //Player Movement
        /*PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));*/

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 PlayerMovementInput = new Vector3(horizontalInput, 0, verticalInput);
        PlayerMovementInput = Quaternion.AngleAxis(CamDir.rotation.eulerAngles.y, Vector3.up) * PlayerMovementInput;
        PlayerMovementInput.Normalize();

        m_velocity += PlayerMovementInput * (speed * Time.deltaTime);

        this.transform.position += m_velocity;

        m_velocity -= m_velocity * 0.25f;

        Debug.Log(m_velocity);
        //transform.Translate(PlayerMovementInput * speed * Time.deltaTime, Space.World);

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
