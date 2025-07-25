using Assets.Scripts;
using Assets.Scripts.Environment;
using Assets.Scripts.Util;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SceneManagement;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] 
    private GameObject m_player;
    [SerializeField]
    private GameObject[] m_spawnPoints;

    private GameObject m_playerBody;

    private PlayerMovement m_plyerMove;
    [SerializeField] float TimeToRespawn = 3f;
    private bool m_playerSpawn;

    [SerializeField]
    private UIMenus m_canvas;
    [SerializeField] SpawnEffect Effect;

    //public PlayerRespawn()
    //{

    //}

    // Start is called before the first frame update
    void Start()
    {
        m_playerBody = m_player.FindChild("SK_SpiderBot");
        m_plyerMove = m_player.GetComponent<PlayerMovement>();
        m_playerSpawn = false;
    }

    // Update is called once per frame
    void Update()
    {
         Vector3 playerPos = m_player.transform.position;

        if (m_playerSpawn == true)
        {
            TimeToRespawn -= Time.deltaTime;
            if (TimeToRespawn <= 0.1f)
            {
                var rndIdx = Random.Range(0, m_spawnPoints.Length);
                var spawnPoint = m_spawnPoints[rndIdx];

                m_player.transform.position = spawnPoint.transform.position;
                if (TimeToRespawn <= 0f)
                {
                    Effect.PlayEffect();
                    m_plyerMove.enabled = true;
                    m_playerBody.SetActive(true);
                    m_playerSpawn = false;
                    TimeToRespawn = 3f;
                }
            }
        }
    }

    public void Respawn()
    {
        Debug.Log("Calling caught screen: Player Respawn script");
        m_canvas.CaughtScreen();
        m_plyerMove.enabled = false;
        m_playerBody.SetActive(false);
        m_playerSpawn = true;
    }

    /*public void ReplaceSpawn(GameObject RespawnPoint)
    {
        m_spawnPoints.Remove(RespawnPoint);
    }*/

    public void ReplaceSpawn(GameObject RespawnPoint)
    {
        m_spawnPoints[0] = RespawnPoint;
        //m_player = RespawnPoint;
        //m_spawnPoints newObject = new RespawnPoint;
    }
}
