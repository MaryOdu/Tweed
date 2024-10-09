using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemy
{
    public class AgentAI : MonoBehaviour
    {
        private AgentFollow m_agentFollow;
        private AgentPatrol m_agentPatrol;
        private NavMeshAgent m_agent;

        [SerializeField]
        private GameObject m_player;
        private TimeSpan m_chaseCooldown;
        private float m_playerLastSeenTime;


        // Start is called before the first frame update
        void Start()
        {
            m_agent = this.GetComponent<NavMeshAgent>();
            m_agentFollow = this.GetComponent<AgentFollow>();
            m_agentPatrol = this.GetComponent<AgentPatrol>();

            m_agentPatrol.enabled = false;
            m_agentFollow.enabled = false;

            m_chaseCooldown = TimeSpan.FromSeconds(10);
        }

        // Update is called once per frame
        void Update()
        {
            var deltaV = m_player.gameObject.transform.position - this.gameObject.transform.position;
            var ray = new Ray(this.gameObject.transform.position, deltaV.normalized);

            var rayHit = Physics.Raycast(ray, out var hitInfo, float.PositiveInfinity, LayerMask.GetMask("Default"));

            if (rayHit && hitInfo.collider.gameObject.tag == "Player")
            {
                Debug.DrawRay(ray.origin, ray.direction, Color.red);
                m_agentFollow.Target = hitInfo.collider.gameObject;
                m_agentFollow.enabled = true;
                m_agentPatrol.enabled = false;
                m_playerLastSeenTime = Time.time;
            }
            else
            {
                m_agentFollow.enabled = false;
                Debug.DrawRay(ray.origin, ray.direction, Color.blue);

                if (m_agent.remainingDistance <= m_agent.stoppingDistance)
                {
                    if (!m_agent.hasPath || m_agent.velocity.sqrMagnitude == 0f)
                    {
                        var deltaT = Time.time - m_playerLastSeenTime;

                        if (deltaT > m_chaseCooldown.TotalSeconds)
                        {
                            m_agentPatrol.enabled = true;
                        }
                    }
                }
            }
        }
    }
}