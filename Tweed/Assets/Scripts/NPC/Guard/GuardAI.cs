using Assets.Scripts.NPC;
using Assets.Scripts.NPC.Sentry;
using Assets.Scripts.Util;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public enum GuardState
    {
        Patrol,
        Search,
        Alert,
    }

    [RequireComponent(typeof(GuardAttack))]
    [RequireComponent(typeof(GuardPatrol))]
    [RequireComponent(typeof(GuardSearch))]
    public class GuardAI : NPCAgent
    {
        /// <summary>
        /// The current state of the guard NPC/agent
        /// </summary>
        [SerializeField]
        private GuardState m_state;

        /// <summary>
        /// The gauard NPCs' attack behaviour.
        /// </summary>
        private GuardAttack m_agentAttack;

        /// <summary>
        /// The guard NPCs' patrol behaviour.
        /// </summary>
        private GuardPatrol m_agentPatrol;

        /// <summary>
        /// The guard NPCs' search behaviour.
        /// </summary>
        private GuardSearch m_agentSearch;

        /// <summary>
        /// The NPCs' current target.
        /// </summary>
        private GameObject m_target;

        /// <summary>
        /// The patrol points assigned to this NPC.
        /// </summary>
        [SerializeField]
        private List<GameObject> m_patrolPoints;

        /// <summary>
        /// The movement speed of this NPC when on patrol.
        /// </summary>
        [SerializeField]
        private float m_patrolSpeed;

        /// <summary>
        /// The movement speed of this NPC when searching for a target.
        /// </summary>
        [SerializeField]
        private float m_searchSpeed;

        /// <summary>
        /// The movement speed of this NPC when alerted.
        /// </summary>
        [SerializeField]
        private float m_alertSpeed;

        /// <summary>
        /// The amount of time this NPC will spend searching for a player before returning to its' patrol state.
        /// </summary>
        [SerializeField]
        private TimeSpan m_searchTime;

        /// <summary>
        /// The time a target was last seen.
        /// </summary>
        [SerializeField]
        private float m_targetLastSeenTime;

        /// <summary>
        /// The distance the agent will stop by when reaching its destination.
        /// </summary>
        [SerializeField]
        private float m_stopDistance;

        /// <summary>
        /// The distance the agent will stop by when reaching its destination and is attacking.
        /// </summary>
        [SerializeField]
        private float m_attackStopDistance;

        /// <summary>
        /// The last gameobject to put this agent into an 'alert' state.
        /// </summary>
        private GameObject m_alertedBy;


        /// <summary>
        /// The amount of time between agent updates...
        /// </summary>
        [SerializeField]
        private TimeSpan m_updateSpan;

        /// <summary>
        /// Simulates how long this agent takes to think between actions. As opposed to acting on every frame.
        /// </summary>
        private GameTimer m_thinkTimer;

        /// <summary>
        /// Gets the NPC agents current state.
        /// </summary>
        public GuardState State
        {
            get
            {
                return m_state;
            }
        }

        /// <summary>
        /// Gets the NPC agents remaining search time.
        /// </summary>
        public float RemainingSearchTime
        {
            get
            {
                return (float)m_searchTime.TotalSeconds - (Time.time - m_targetLastSeenTime);
            }
        }

        /// <summary>
        /// Gets whether the NPC agent is currently attacking something.
        /// </summary>
        public bool IsAttacking
        {
            get
            {
                return m_agentAttack.IsAttacking;
            }
        }

        /// <summary>
        /// Gets the NPC agents' current target.
        /// </summary>
        public GameObject Target
        {
            get
            {
                return m_target;
            }
        }

        public bool IsStopped
        {
            get
            {
                return (m_agentAttack.RemainingDistance + m_agentPatrol.RemainingDistance + m_agentSearch.RemainingDistance) < 1;
            }
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        public GuardAI()
            : base()
        {
            m_patrolPoints = new List<GameObject>();
            
            m_attackStopDistance = 8f;
            m_stopDistance = 3f;

            m_updateSpan = TimeSpan.FromSeconds(0.5);
            m_thinkTimer = new GameTimer(m_updateSpan);
            m_thinkTimer.AutoReset = true;
            m_thinkTimer.OnTimerElapsed += ThinkTimer_OnTimerElapsed;

            this.SetSightParameters(30.0f, 45.0f);
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            m_thinkTimer.SetTimeSpan(m_updateSpan);
            
            m_thinkTimer.Start();

            m_agentAttack = this.GetComponent<GuardAttack>();
            m_agentPatrol = this.GetComponent<GuardPatrol>();
            m_agentSearch = this.GetComponent<GuardSearch>();

            if (m_agentAttack == null)
            {
                m_agentAttack = this.AddComponent<GuardAttack>();
            }

            if (m_agentPatrol == null)
            {
                m_agentPatrol = this.AddComponent<GuardPatrol>();
            }
            
            if (m_agentSearch == null)
            {
                m_agentSearch = this.AddComponent<GuardSearch>();
            }

            m_agentSearch.OnSearchComplete += this.SearchAgent_OnSearchComplete;
            m_agentAttack.OnAttackComplete += this.AgentAttack_OnAttackComplete;

            m_agentPatrol.enabled = false;
            m_agentAttack.enabled = false;
            m_agentSearch.enabled = false;

            m_agentSearch.Target = m_target;

            m_agentAttack.Target = m_target;

            m_agentPatrol.PatrolPoints = m_patrolPoints;

            m_searchTime = TimeSpan.FromSeconds(60);
            m_state = GuardState.Patrol;

            base.Start();
        }


        // Update is called once per frame
        protected override void Update()
        {
            m_thinkTimer.Tick();
            base.Update();
        }

        private void SearchAgent_OnSearchComplete(object sender, EventArgs e)
        {
            m_state = GuardState.Patrol;
            m_searchTime = TimeSpan.Zero;
        }

        private void AgentAttack_OnAttackComplete(object sender, EventArgs e)
        {
            m_state = GuardState.Patrol;
            m_searchTime = TimeSpan.Zero;
        }

        private void ThinkTimer_OnTimerElapsed(object sender, TimerElapsedEventArgs e)
        {
            bool targetSeen = false;

            var orderedTargets = this.GetTargetsByDistance();

            foreach (var target in orderedTargets)
            {
                targetSeen = this.QueryAlertedBy() || AIHelper.CanSeeObject(this.gameObject, target, this.SightRange, this.SightAngle, true);

                if (targetSeen)
                {
                    var health = target.GetComponent<Health>();

                    if (health && health.IsDead)
                    {
                        targetSeen = false;
                        continue;
                    }

                    this.SetTarget(target);
                    break;
                }
            }

            if (targetSeen)
            {
                m_agentAttack.Target = m_target;
                m_state = GuardState.Alert;
                m_targetLastSeenTime = Time.time;
            }
            else
            {
                m_state = m_targetLastSeenTime == 0 || this.RemainingSearchTime <= 0.0f ? GuardState.Patrol : GuardState.Search;

                if (this.WithinStoppingDistance() && this.RemainingSearchTime <= 0.0f)
                {
                    m_state = GuardState.Patrol;
                    m_searchTime = TimeSpan.FromSeconds(60);
                }
            }

            this.UpdateActiveBehaviour();
            this.UpdateStopDsistance();
        }

        /// <summary>
        /// Checks to see whether the npc/agent is within stopping distance.
        /// </summary>
        /// <returns>is within stopping distance? (true/false)</returns>
        private bool WithinStoppingDistance()
        {
            if (this.NavAgent.remainingDistance <= this.NavAgent.stoppingDistance)
            {
                if (!this.NavAgent.hasPath || this.NavAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sets the current target of the agent/npc.
        /// </summary>
        /// <param name="value">Target object</param>
        public void SetTarget(GameObject value)
        {
            m_target = value;
            m_agentSearch.Target = m_target;
            m_agentAttack.Target = m_target;
        }

        /// <summary>
        /// Updates the stopping distance of the navAgent depending on wether the NPC is attacking or not.
        /// </summary>
        private void UpdateStopDsistance()
        {
            if (m_agentAttack.enabled)
            {
                this.NavAgent.stoppingDistance = m_attackStopDistance;
            }
            else
            {
                this.NavAgent.stoppingDistance = m_stopDistance;
            }
        }

        /// <summary>
        /// Will place this NPC/Agent into alert state.
        /// </summary>
        /// <param name="alertedBy">Entity which has alerted this guard.</param>
        public void AlertGuard(GameObject alertedBy)
        {
            m_alertedBy = alertedBy;
            m_state = GuardState.Alert;
        }

        /// <summary>
        /// Query's the sentry that has alerted this guard NPC/agent.
        /// </summary>
        /// <returns>Target in sight & is this guard in pursuit? (true / false)</returns>
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

        /// <summary>
        /// Query's a specified sentry.
        /// </summary>
        /// <param name="sentry">Sentry to be queried.</param>
        /// <returns>Target in sight & is this guard in pursuit? (true / false)</returns>
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
                        return this.NavAgent.SetDestination(sentry.CurrentLookAtTarget.transform.position);
                    }
                    break;
            }

            return false;
        }

        /// <summary>
        /// Switches enabled behaviours based upon the guards current state <see cref="GuardState"/>
        /// </summary>
        private void UpdateActiveBehaviour()
        {
            switch (m_state)
            {
                case GuardState.Patrol:
                    m_agentPatrol.enabled = true;
                    m_agentAttack.enabled = false;
                    m_agentSearch.enabled = false;

                    this.NavAgent.speed = m_patrolSpeed;

                    break;
                case GuardState.Search:
                    m_agentPatrol.enabled = false;
                    m_agentAttack.enabled = false;
                    m_agentSearch.enabled = true;

                    this.NavAgent.speed = m_searchSpeed;

                    break;
                case GuardState.Alert:
                    m_agentPatrol.enabled = false;
                    m_agentAttack.enabled = true;
                    m_agentSearch.enabled = false;

                    this.NavAgent.speed = m_alertSpeed;

                    break;
            }
        }
    }
}