using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemy
{
    public enum EnemyState
    {
        Patrol,
        Search,
        Alert,
    }

    public class AgentAI : MonoBehaviour
    {
        private EnemyState m_state;

        private AgentFollow m_agentFollow;
        private AgentPatrol m_agentPatrol;
        private AgentSearch m_agentSearch;

        private NavMeshAgent m_agent;

        [SerializeField]
        private GameObject m_target;

        [SerializeField]
        private List<GameObject> m_patrolPoints;

        [SerializeField]
        private float m_agentPatrolSpeed;
        [SerializeField]
        private float m_agentSearchSpeed;
        [SerializeField]
        private float m_agentAlertSpeed;

        private TimeSpan m_searchTime;

        private float m_playerLastSeenTime;

        public EnemyState State
        {
            get
            {
                return m_state;
            }
        }

        public AgentAI()
        {
            m_patrolPoints = new List<GameObject>();
        }

        // Start is called before the first frame update
        void Start()
        {
            m_agent = this.GetComponent<NavMeshAgent>();
            m_agentFollow = this.AddComponent<AgentFollow>();
            m_agentPatrol = this.AddComponent<AgentPatrol>();
            m_agentSearch = this.AddComponent<AgentSearch>();

            m_agentPatrol.enabled = false;
            m_agentFollow.enabled = false;
            m_agentSearch.enabled = false;

            m_agentSearch.Target = m_target;
            m_agentFollow.Target = m_target;

            m_agentPatrol.PatrolPoints = m_patrolPoints;

            m_searchTime = TimeSpan.FromSeconds(60);
            m_state = EnemyState.Patrol;
        }

        // Update is called once per frame
        void Update()
        {
            var playerPos = m_target.gameObject.transform.position;
            var deltaV = playerPos - this.gameObject.transform.position;
            var ray = new Ray(this.gameObject.transform.position, deltaV.normalized);

            var rayHit = Physics.Raycast(ray, out var hitInfo, float.PositiveInfinity, LayerMask.GetMask("Default"));

            var b = (playerPos - this.gameObject.transform.position).normalized;
            var a = this.gameObject.transform.forward.normalized;
            var angle = Vector3.Angle(a, b);

            var canSeePlayer = rayHit && hitInfo.collider.gameObject.tag == "Player" && hitInfo.distance < 30.0f && angle < 45.0f;

            if (canSeePlayer)
            {
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
                m_agentFollow.Target = hitInfo.collider.gameObject;
                m_state = EnemyState.Alert;
                m_playerLastSeenTime = Time.time;
            }
            else
            {
                var deltaT = Time.time - m_playerLastSeenTime;
                m_state = m_playerLastSeenTime == 0 || deltaT > m_searchTime.TotalSeconds ? EnemyState.Patrol : EnemyState.Search;

                Debug.DrawRay(ray.origin, ray.direction * 100, Color.blue);

                if (m_agent.remainingDistance <= m_agent.stoppingDistance)
                {
                    if (!m_agent.hasPath || m_agent.velocity.sqrMagnitude == 0f)
                    {
                        if (deltaT > m_searchTime.TotalSeconds)
                        {
                            m_state = EnemyState.Patrol;
                        }
                    }
                }
            }

            this.UpdateActiveBehaviour();
        }

        private void UpdateActiveBehaviour()
        {
            switch (m_state)
            {
                case EnemyState.Patrol:
                    m_agentPatrol.enabled = true;
                    m_agentFollow.enabled = false;
                    m_agentSearch.enabled = false;

                    m_agent.speed = m_agentPatrolSpeed;
                    break;
                case EnemyState.Search:
                    m_agentPatrol.enabled = false;
                    m_agentFollow.enabled = false;
                    m_agentSearch.enabled = true;

                    m_agent.speed = m_agentSearchSpeed;
                    break;
                case EnemyState.Alert:
                    m_agentPatrol.enabled = false;
                    m_agentFollow.enabled = true;
                    m_agentSearch.enabled = false;

                    m_agent.speed = m_agentAlertSpeed;
                    break;
            }
        }
    }
}