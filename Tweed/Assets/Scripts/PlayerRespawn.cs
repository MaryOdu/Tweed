using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] GameObject PlayerBod;
    [SerializeField] GameObject RL1;

    public PlayerMovement PlayerMove;
    [SerializeField] float TimeToRespawn = 3f;
    [SerializeField] bool PlSpawn;

    

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
        PlayerBod = GameObject.Find("SK_SpiderBot");
        RL1 = GameObject.Find("Respawn1Start");
        Vector3 RL1Pos = RL1.transform.position;
        PlSpawn = false;
    }

    // Update is called once per frame
    void Update()
    {
         Vector3 PlayerPos = Player.transform.position;

        if (PlSpawn == true)
        {
          
            TimeToRespawn -= Time.deltaTime;
            if (TimeToRespawn <= 0.1f)
            {
                Player.transform.position = RL1.transform.position;
                if (TimeToRespawn <= 0f)
                {

                    PlayerMove.enabled = true;
                    PlayerBod.SetActive(true);
                    PlSpawn = false;
                    TimeToRespawn = 3f;
                }
            }
        }
    }

    public void respawn()
    {
        PlayerMove.enabled = false;
        PlayerBod.SetActive(false);
        PlSpawn = true;

    }
}
