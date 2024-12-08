using Assets.Scripts.Enemy;
using Assets.Scripts.NPC;
using Assets.Scripts.NPC.Sentry;
using Assets.Scripts.NPC.Xeno;
using Assets.Scripts.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum XenoState
{
    Swarm, 
    Search,
    Alert
}

public class XenoAI : NPCAgent
{
    public event EventHandler<EventArgs> OnMeleeAttack;

    /// <summary>
    /// The current state of the xeno NPC/Agent
    /// </summary>
    [SerializeField]
    private XenoState m_state;

    /// <summary>
    /// The xeno NPCs' attack behaviour.
    /// </summary>
    private XenoAttack m_agentAttack;
    private Rigidbody m_rigidBody;

    /// <summary>
    /// The xeno NPCs' swarm behaviour
    /// </summary>
    private XenoSwarm m_agentSwarm;

    /// <summary>
    /// The xeno NPCs' search behaviour.
    /// </summary>
    private NPCSearch m_agentSearch;

    /// <summary>
    /// The NPCs' current target.
    /// </summary>
    private GameObject m_target;

    /// <summary>
    /// The default movement speed of this NPC.
    /// </summary>
    [SerializeField]
    private float m_movementSpeed;

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
    /// The amount of time this NPC will spend in an attack state after losing sight of the target.
    /// </summary>
    [SerializeField]
    private TimeSpan m_attackTime;

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
    /// The amount of time between agent updates...
    /// </summary>
    [SerializeField]
    private TimeSpan m_updateSpan;

    /// <summary>
    /// Simulates how long this agent takes to think between actions. As opposed to acting on every frame.
    /// </summary>
    private GameTimer m_thinkTimer;

    /// <summary>
    /// The last gameobject to put this agent into an 'alert' state.
    /// </summary>
    private GameObject m_alertedBy;

    public XenoState State
    {
        get
        {
            return m_state;
        }
    }

    public override bool IsStopped
    {
        get
        {
            switch (m_state)
            {
                case XenoState.Swarm:
                    return m_rigidBody.velocity.magnitude < 1;
                case XenoState.Search:
                case XenoState.Alert:
                    return base.IsStopped;
            }

            return true;
        }
    }

    /// <summary>
    /// Gets the NPC agents remaining search time.
    /// </summary>
    public float RemainingAttackTime
    {
        get
        {
            return (float)m_attackTime.TotalSeconds - (Time.time - m_targetLastSeenTime);
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
    /// Gets the NPC agents' current target.
    /// </summary>
    public GameObject Target
    {
        get
        {
            return m_target;
        }
    }

    public Vector3 Velocity
    {
        get
        {
            return m_rigidBody.velocity;
        }
    }

    public XenoAI()
        : base()
    {
        m_stopDistance = 3f;
        m_attackStopDistance = 7f;
        this.SetSightParameters(20.0f, 35.0f);

        m_updateSpan = TimeSpan.FromSeconds(0.3);
        m_thinkTimer = new GameTimer(m_updateSpan);
        m_thinkTimer.AutoReset = true;
        m_thinkTimer.OnTimerElapsed += ThinkTimer_OnTimerElapsed;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        m_thinkTimer.SetTimeSpan(m_updateSpan);

        m_thinkTimer.Start();

        m_rigidBody = this.GetComponent<Rigidbody>();

        m_agentSwarm = this.GetComponent<XenoSwarm>();
        m_agentSearch = this.GetComponent<NPCSearch>();
        m_agentAttack = this.GetComponent<XenoAttack>();

        //if (m_agentSwarm == null)
        //{
        //    m_agentSwarm = this.AddComponent<XenoSwarm>();
        //}

        //if (m_agentSearch == null)
        //{
        //    m_agentSearch = this.AddComponent<NPCSearch>();
        //}
        
        //if (m_agentAttack == null)
        //{
        //    m_agentAttack = this.AddComponent<XenoAttack>();
        //}
        
        m_agentSwarm.enabled = true;
        m_agentSearch.enabled = false;
        m_agentAttack.enabled = false;

        m_agentSearch.OnSearchComplete += AgentSearch_OnSearchComplete;

        m_searchTime = TimeSpan.FromSeconds(30);
        m_attackTime = TimeSpan.FromSeconds(0.1f);

        m_state = XenoState.Swarm;

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        m_thinkTimer.Tick();

        if (m_agentAttack.IsMeleeAttacking)
        {
            this.OnMeleeAttack?.Invoke(this, EventArgs.Empty);
        }

        base.Update();
    }

    private void ThinkTimer_OnTimerElapsed(object sender, TimerElapsedEventArgs e)
    {
        bool targetSeen = false;

        var orderedTargets = this.GetTargetsByDistance();

        foreach (var target in orderedTargets)
        {
            var sightAngle = m_state == XenoState.Alert ? this.SightAngle * 2 : this.SightAngle;
            targetSeen = this.QueryAlertedBy() || AIHelper.CanSeeObject(this.gameObject, target, this.SightRange, sightAngle, LayerMask.GetMask("Default"), true);

            if (targetSeen)
            {
                this.SetTarget(target);
                m_state = XenoState.Alert;
                m_targetLastSeenTime = Time.time;
                break;
            }
            else
            {
                if (m_state == XenoState.Alert && this.RemainingAttackTime <= 0)
                {
                    m_state = XenoState.Search;
                    
                }

                if (m_state == XenoState.Search && this.RemainingSearchTime <= 0)
                {
                    m_state = XenoState.Swarm;
                }
            }
        }

        this.UpdateActiveBehaviour();
        this.UpdateStopDsistance();
    }

    public void AlertXeno(GameObject alertedBy)
    {
        m_alertedBy = alertedBy;
        m_state = XenoState.Alert;
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
    /// Query's the sentry that has alerted this guard NPC/agent.
    /// </summary>
    /// <returns>Target in sight & is this guard in pursuit? (true / false)</returns>
    private bool QueryAlertedBy()
    {
        if (m_alertedBy != null)
        {
            var xeno = m_alertedBy.GetComponent<XenoAI>();

            if (xeno != null)
            {
                return this.QueryXeno(xeno);
            }
        }

        return false;
    }

    /// <summary>
    /// Query's a specified sentry.
    /// </summary>
    /// <param name="sentry">Sentry to be queried.</param>
    /// <returns>Target in sight & is this guard in pursuit? (true / false)</returns>
    private bool QueryXeno(XenoAI xeno)
    {
        switch (xeno.State)
        {
            case XenoState.Search:
                m_alertedBy = null;
                m_state = XenoState.Search;
                break;
            case XenoState.Alert:
                if (xeno.Target != null)
                {
                    m_state = XenoState.Alert;
                    return this.NavAgent.SetDestination(xeno.Target.transform.position);
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
            case XenoState.Swarm:
                this.NavAgent.speed = m_movementSpeed;
                m_agentSwarm.MovementSpeed = m_movementSpeed;

                m_agentSwarm.enabled = true;
                m_agentSearch.enabled = false;
                m_agentAttack.enabled = false;
                break;
            case XenoState.Search:
                this.NavAgent.speed = m_searchSpeed;
                m_agentSwarm.MovementSpeed = m_searchSpeed;

                m_agentSwarm.enabled = false;
                m_agentSearch.enabled = true;
                m_agentAttack.enabled = false;
                break;
            case XenoState.Alert:
                this.NavAgent.speed = m_alertSpeed;
                m_agentSwarm.MovementSpeed = m_alertSpeed;

                m_agentSwarm.enabled = false;
                m_agentSearch.enabled = false;
                m_agentAttack.enabled = true;
                break;
        }
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

    private void AgentSearch_OnSearchComplete(object sender, EventArgs e)
    {
        m_state = XenoState.Swarm;
    }
}
