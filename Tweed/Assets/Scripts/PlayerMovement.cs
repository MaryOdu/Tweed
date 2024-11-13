using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.AI;


public class PlayerMovement : MonoBehaviour
{
    //Player Movement speed
    [SerializeField]
    private float SpeedUp = 0.8f;

    [SerializeField]
    private float rotationSpeed = 700f;

    private Vector3 m_velocity;

    private Rigidbody m_rigidBody;

    [SerializeField] 
    private Transform CamDir;
    //private Vector3 PlayerMovementInput;

    [SerializeField]
    Animator animator;
    private bool WalkOn;
    

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_velocity = Vector3.zero;

        m_rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
        
    }


    private void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical"); 

        Vector3 PlayerMovementInput = new Vector3(horizontalInput, 0, verticalInput);
        PlayerMovementInput = Quaternion.AngleAxis(CamDir.rotation.eulerAngles.y, Vector3.up) * PlayerMovementInput;
        PlayerMovementInput.Normalize();

        m_velocity += PlayerMovementInput * (SpeedUp * Time.deltaTime);

        this.transform.position += m_velocity;

        m_velocity -= m_velocity * 0.25f;

        if (PlayerMovementInput != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(PlayerMovementInput, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            animator.SetBool("IsWalking", true);
            WalkOn = true;
        }
        else { animator.SetBool("IsWalking", false); WalkOn = false; }
        //bug where when direction key is lifted running animation still plays, 
        //need a condenced way to tie this to KeyCodes.

        if (WalkOn == true && (Input.GetKeyDown(KeyCode.LeftShift)))
        {
            animator.SetBool("IsRunning", true);
            SpeedUp = 1.6f;
        }
        if (WalkOn == false || Input.GetKeyUp(KeyCode.LeftShift)) { animator.SetBool("IsRunning", false); SpeedUp = 0.8f;}
    }

}
