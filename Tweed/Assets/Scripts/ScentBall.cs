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

    public bool OneBall = false;
    private float RotateXSpeed = 80f;
    private float RotateZSpeed = 32f;



    void Start()
    {
       
        
        Canister = Player.FindChild("ScentOrbPos");
        PlayerControls.GetComponent<PlayerMovement>();

        //BallSpawner = BallSpawner.FindParent()
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
    }

    // Update is called once per frame
    void LateUpdate()
    {
        ScentBomb.transform.Rotate(Vector3.right, RotateXSpeed * Time.deltaTime);
        ScentBomb.transform.Rotate(Vector3.up, RotateZSpeed * Time.deltaTime);
    }

    public void BallReturn()
    {
        transform.SetParent(BallSpawner.transform);

        transform.localPosition = Vector3.zero;
        
    }
}
