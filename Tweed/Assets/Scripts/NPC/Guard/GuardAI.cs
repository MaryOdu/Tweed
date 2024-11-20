using Assets.Scripts.NPC.Sentry;
using Assets.Scripts.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.PackageManager; Was giveing an error when trying to build the project to get menus running <- ('ZAC')
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemy
{
    public enum GuardState
    {
        Patrol,
        Search,
        Alert,
    }

    public class GuardAI : MonoBehaviour
    {
        private const int debugRayDistance = 100;

        [SerializeField]
        private GuardState m_state;

        private GuardAttack m_agentAttack;
        private GuardPatrol m_agentPatrol;
        private GuardSearch m_agentSearch;

        private NavMeshAgent m_agent;

        [SerializeField]
        private GameObject m_target;

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

        [SerializeField]
        private TimeSpan m_searchTime;

        [SerializeField]
        private float m_playerLastSeenTime;

        [SerializeField]
        private float m_stopDistance;

        [SerializeField]
        private float m_attackStopDistance;

        private GameObject m_alertedBy;


        public GuardState State
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

        public bool IsAttacking
        {
            get
            {
                return m_agentAttack.IsAttacking;
            }
        }

        public float SightRange
        {
            get
            {
                return m_sightRange;
            }
        }

        public float SightAngle
        {
            get
            {
                return m_sightAngle;
            }
        }

        public GuardAI()
        {
            m_patrolPoints = new List<GameObject>();
            m_sightRange = 30.0f;
            m_sightAngle = 45.0f;
            m_attackStopDistance = 8f;
            m_stopDistance = 3f;
        }

        // Start is called before the first frame update
        void Start()
        {
            m_agent = this.GetComponent<NavMeshAgent>();
            m_agentAttack = this.AddComponent<GuardAttack>();
            m_agentPatrol = this.AddComponent<GuardPatrol>();
            m_agentSearch = this.AddComponent<GuardSearch>();

            m_agentPatrol.enabled = false;
            m_agentAttack.enabled = false;
            m_agentSearch.enabled = false;

            m_agentSearch.Target = m_target;
            m_agentAttack.Target = m_target;

            m_agentPatrol.PatrolPoints = m_patrolPoints;

            m_searchTime = TimeSpan.FromSeconds(60);
            m_state = GuardState.Patrol;
        }

        // Update is called once per frame
        void Update()
        {
            var playerSeen = this.QueryAlertedBy() || AIHelper.CanSeeObject(this.gameObject, m_target, m_sightRange, m_sightAngle, true);

            if (playerSeen)
            {
                m_agentAttack.Target = m_target;
                m_state = GuardState.Alert;
                m_playerLastSeenTime = Time.time;
            }
            else
            {
                m_state = m_playerLastSeenTime == 0 || this.RemainingSearchTime <= 0.0f ? GuardState.Patrol : GuardState.Search;

                if (m_agent.remainingDistance <= m_agent.stoppingDistance)
                {
                    if (!m_agent.hasPath || m_agent.velocity.sqrMagnitude == 0f)
                    {
                        if (this.RemainingSearchTime <= 0.0f)
                        {
                            m_state = GuardState.Patrol;
                            m_searchTime = TimeSpan.FromSeconds(60);
                        }
                    }
                }
            }

            this.UpdateActiveBehaviour();
            this.UpdateStopDsistance();
        }

        private void UpdateStopDsistance()
        {
            if (m_agentAttack.enabled)
            {
                m_agent.stoppingDistance = m_attackStopDistance;
            }
            else
            {
                m_agent.stoppingDistance = m_stopDistance;
            }
        }



        public void AlertGuard(GameObject alertedBy)
        {
            m_alertedBy = alertedBy;
            m_state = GuardState.Alert;
        }

        private bool QueryAlertedBy()
        {
            if (m_alertedBy != null)
            {
                var sentry = m_alertedBy.GetComponent<SentryAI>();

                if (sentry != null)
                {
                    return this.QuerySentry(sentry);
                }
            }

            return false;
        }

        private bool QuerySentry(SentryAI sentry)
        {
            switch (sentry.State)
            {
                case SentryState.Passive:
                    m_alertedBy = null;
                    m_state = GuardState.Search;
                    break;
                case SentryState.Alert:
                    if (sentry.CurrentLookAtTarget != null)
                    {
                        return m_agent.SetDestination(sentry.CurrentLookAtTarget.transform.position);
                    }
                    break;
            }

            return false;
        }

        private void UpdateActiveBehaviour()
        {
            switch (m_state)
            {
                case GuardState.Patrol:
                    m_agentPatrol.enabled = true;
                    m_agentAttack.enabled = false;
                    m_agentSearch.enabled = false;

                    m_agent.speed = m_patrolSpeed;

                    break;
                case GuardState.Search:
                    m_agentPatrol.enabled = false;
                    m_agentAttack.enabled = false;
                    m_agentSearch.enabled = true;

                    m_agent.speed = m_searchSpeed;

                    break;
                case GuardState.Alert:
                    m_agentPatrol.enabled = false;
                    m_agentAttack.enabled = true;
                    m_agentSearch.enabled = false;

                    m_agent.speed = m_alertSpeed;

                    break;
            }
        }
    }
}