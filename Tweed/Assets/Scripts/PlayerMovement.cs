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

    public UIMenus Canvas;
    // public PlayerRespawn PlayerCaught;

    [SerializeField] float WalkableAngleUnder = 40f;
    [SerializeField] float SlideingSpeed = 20f;
    private Vector3 floorAngle;

    public ScentBall ScentShot;
    private bool IsSliding
    {
        get
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 1f))
            {
                floorAngle = slopeHit.normal;
                return Vector3.Angle(floorAngle, Vector3.up) > WalkableAngleUnder;
                /*float groundAngle = Vector3.Angle(slopeHit.normal, Vector3.up);

                Debug.DrawRay(transform.position, Vector3.down, Color.red);
                if (groundAngle > WalkableAngle)
                {
                    Vector3 slideDirection = Vector3.Project(Vector3.down, hit.normal).normalized;

                    m_velocity = slideDirection * Slideing;

                }
                else
                {

                }*/
            }
            else
            {
                return false;
            }
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_velocity = Vector3.zero;

        m_rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        ScentShot.GetComponent<ScentBall>();

}

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (this.Canvas != null)
            {
                Canvas.Pause();
            }
        }

        if (Input.GetMouseButtonDown(0) && ScentShot.OneBall == true)
        {
            ScentShot.BallReturn();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("XenoAttack"))
        {
            
            //PlayerCaught.respawn();
            //Canvas.CaughtScreen();
          
        }
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

        //
        //When on a slope call bool
        //
        if (IsSliding)
        {
            m_velocity = new Vector3(floorAngle.x, -floorAngle.y, floorAngle.z) * SlideingSpeed;
            Debug.Log("Am Slideing");
        }
        
    }

}
