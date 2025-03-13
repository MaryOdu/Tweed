using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnActor : MonoBehaviour
{

    
    [SerializeField] float rotateSpeed = 60f;
    //When called moves Object to desired point to start bounceing
    [SerializeField] float MTPspeed = 0.5f;
    private Vector3 EndPos;

    private float Ariveing = 0.2f;
    private bool startBounce = false;

    //Bounce between 2 points
    //private Vector3 StartPos;
    private Vector3 TopPos;
    private Vector3 BotPos;
    [SerializeField] float BounceSpeed = 0.8f;
    private float ToReachTime = 0f;

    void Start()
    {
        EndPos = new Vector3(0, 0.8f, 0);
        TopPos = new Vector3(0, 0.8f, 0);
        BotPos = new Vector3(0, 0.6f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, EndPos, MTPspeed * Time.deltaTime);
        transform.RotateAround(transform.position, Vector3.up, rotateSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.localPosition, EndPos) <= Ariveing)
        {
            startBounce = true;
        }
        if (startBounce == true)
        {
            ToReachTime += Time.deltaTime * BounceSpeed;
            transform.localPosition = Vector3.Lerp(TopPos, BotPos, Mathf.PingPong(ToReachTime, 1));
        }
        
    }
}
