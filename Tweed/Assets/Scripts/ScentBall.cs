using Assets.Scripts.Util;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScentBall : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] GameObject ScentBomb;
    [SerializeField] GameObject BallSpawner;
    private GameObject Canister;
    [SerializeField] PlayerMovement PlayerControls;

    [SerializeField] float ShotStrength = 10f;
    [SerializeField] Transform FirePoint;
    //private Vector3 FireDirection = Vector3.up;
    private float YPlus = 1f;
    private float ZPlus = -1f;

    public bool OneBall = false;
    private float RotateXSpeed = 80f;
    private float RotateZSpeed = 32f;



    void Start()
    {

        Canister = Player.FindChild("ScentOrbPos");
        PlayerControls.GetComponent<PlayerMovement>();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && PlayerControls.PlayerShot == false)
        {
            transform.SetParent(Canister.transform);

            transform.localPosition = Vector3.zero;
           

            PlayerControls.YouHaveBall();
            //Send player controls value that they still hold a ball PlayerControls

        }

        if (other.CompareTag("Turrain"))
        {
            transform.SetParent(BallSpawner.transform);

            transform.localPosition = Vector3.zero;
            Rigidbody rb = ScentBomb.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            //rb.constraints = RigidbodyConstraints.FreezePositionY;
            //rb.constraints = RigidbodyConstraints.FreezePositionZ;
           
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        ScentBomb.transform.Rotate(Vector3.right, RotateXSpeed * Time.deltaTime);
        ScentBomb.transform.Rotate(Vector3.up, RotateZSpeed * Time.deltaTime);

        
    }

    public void BallReturn()
    {
        Vector3 FireDirection = new Vector3(0f, YPlus, ZPlus);
        Rigidbody rb = ScentBomb.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
        Vector3 FireAngle = FirePoint.TransformDirection(FireDirection);
        if (rb != null)
        {

            transform.SetParent(null);
            rb.AddForce(FireAngle * ShotStrength, ForceMode.Impulse);
        }


    }
}
