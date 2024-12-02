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
    private XenoSearch m_agentSearch;

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

    public XenoState State
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
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        m_rigidBody = this.GetComponent<Rigidbody>();

        m_agentSwarm = this.GetComponent<XenoSwarm>();
        m_agentSearch = this.GetComponent<XenoSearch>();
        m_agentAttack = this.GetComponent<XenoAttack>();

        if (m_agentSwarm == null)
        {
            m_agentSwarm = this.AddComponent<XenoSwarm>();
        }

        if (m_agentSearch == null)
        {
            m_agentSearch = this.AddComponent<XenoSearch>();
        }
        
        if (m_agentAttack == null)
        {
            m_agentAttack = this.AddComponent<XenoAttack>();
        }
        
        m_agentSwarm.enabled = true;
        m_agentSearch.enabled = false;
        m_agentAttack.enabled = false;

        m_searchTime = TimeSpan.FromSeconds(30);
        m_state = XenoState.Swarm;

        base.Start();
    }

    // Update is called once per frame
    private void Update()
    {
        bool targetSeen = false;

        var orderedTargets = this.GetTargetsByDistance();

        foreach (var target in orderedTargets)
        {
            targetSeen = this.QueryAlertedBy() || AIHelper.CanSeeObject(this.gameObject, target, this.SightRange, this.SightAngle, true);

            if (targetSeen)
            {
                this.SetTarget(target);
                break;
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
}
