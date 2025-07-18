/*using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.PackageManager; Was giveing an error when trying to build the project to get menus running <- ('ZAC')
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemy
{
    public enum EnemyState2
    {
        Patrol,
        Search,
        Alert,
    }

    public class Test : MonoBehaviour
    {
        private const int debugRayDistance = 100;
        private EnemyState2 m_state;

        private AgentFollow m_agentFollow;
        private AgentPatrol m_agentPatrol;
        private AgentSearch m_agentSearch;


        //-----------------------------------------------------
        private NavMeshAgent m_agent;

        private Animator AI_Animation; //<- ZAC

        [SerializeField] private GameObject m_target;
        [SerializeField] LayerMask groundLayer, Player;
        [SerializeField] float attackRange;
        [SerializeField] bool PlayerInAttRange;

        //--------------------------------------


        [SerializeField]
        private List<GameObject> m_patrolPoints;

        [SerializeField]
        private float m_patrolSpeed;

        [SerializeField]
        private float m_searchSpeed;

        [SerializeField]
        private float m_alertSpeed;

        [SerializeField]
        private float m_sightRange;

        [SerializeField]
        private float m_sightAngle;

        private TimeSpan m_searchTime;

        private float m_playerLastSeenTime;


        public EnemyState2 State
        {
            get
            {
                return m_state;
            }
        }

        public float RemainingSearchTime
        {
            get
            {
                return (float)m_searchTime.TotalSeconds - (Time.time - m_playerLastSeenTime);
            }

        }

        public Test()
        {
            m_patrolPoints = new List<GameObject>();
            m_sightRange = 30.0f;
            m_sightAngle = 45.0f;
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
            m_state = EnemyState2.Patrol;


            //-----------------------------------------
            AI_Animation = GetComponent<Animator>();
            m_target = GameObject.Find("Player");
            //---------------------------
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

            var canSeePlayer = rayHit && hitInfo.collider.gameObject == m_target && hitInfo.distance < m_sightRange && angle < m_sightAngle;

            if (canSeePlayer)
            {
                Debug.DrawRay(ray.origin, ray.direction * debugRayDistance, Color.red);
                 m_agentFollow.Target = hitInfo.collider.gameObject;
                 m_state = EnemyState2.Alert;

                //------------------------------
                PlayerInAttRange = Physics.CheckSphere(transform.position, attackRange, Player);
                GoToPlayer();

                if (PlayerInAttRange) Attack();
                m_playerLastSeenTime = Time.time;
                Debug.Log("Spotted player");
                //-----------------------
            }
            else
            {
                m_state = m_playerLastSeenTime == 0 || this.RemainingSearchTime <= 0.0f ? EnemyState2.Patrol : EnemyState2.Search;

                Debug.DrawRay(ray.origin, ray.direction * debugRayDistance, Color.blue);

                if (m_agent.remainingDistance <= m_agent.stoppingDistance)
                {
                    if (!m_agent.hasPath || m_agent.velocity.sqrMagnitude == 0f)
                    {
                        if (this.RemainingSearchTime <= 0.0f)
                        {
                            m_state = EnemyState2.Patrol;
                            m_searchTime = TimeSpan.FromSeconds(60);
                        }
                    }
                }
            }

            this.UpdateActiveBehaviour();
        }
        //-------------------------------
        private void GoToPlayer()
        {
            m_agent.SetDestination(m_target.transform.position);
        }

        private void Attack()
        {
            AI_Animation.SetTrigger("IsAttacking");
            AI_Animation.SetBool("IsChaseing", true);
            m_agent.SetDestination(transform.position);
        }

        //----------------------------------

        private void UpdateActiveBehaviour()
        {
            switch (m_state)
            {
                case EnemyState2.Patrol:
                    m_agentPatrol.enabled = true;
                    m_agentFollow.enabled = false;
                    m_agentSearch.enabled = false;

                    m_agent.speed = m_patrolSpeed;
                    AI_Animation.SetBool("IsPatroling", true);
                    AI_Animation.SetBool("IsSearching", false);
                    AI_Animation.SetBool("IsChaseing", false);
                    break;
                case EnemyState2.Search:
                    m_agentPatrol.enabled = false;
                    m_agentFollow.enabled = false;
                    m_agentSearch.enabled = true;

                    m_agent.speed = m_searchSpeed;
                    AI_Animation.SetBool("IsSearching", true);
                    AI_Animation.SetBool("IsPatroling", false);
                    AI_Animation.SetBool("IsChaseing", false);
                    break;
                case EnemyState2.Alert:
                    m_agentPatrol.enabled = false;
                    m_agentFollow.enabled = true;
                    m_agentSearch.enabled = false;

                    m_agent.speed = m_alertSpeed;
                    AI_Animation.SetBool("IsChaseing", true);
                    AI_Animation.SetBool("IsPatroling", false);
                    AI_Animation.SetBool("IsSearching", false);
                    break;
            }
        }
    }
}*/
