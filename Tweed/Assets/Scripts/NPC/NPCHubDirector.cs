using Assets.Scripts.NPC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCHubDirector : MonoBehaviour
{
    [SerializeField]
    private GameObject m_player;

    /// <summary>
    /// The list of targets that this NPC Director is seeking to attack.
    /// </summary>
    [SerializeField]
    private List<GameObject> m_targets;

    /// <summary>
    /// Agents
    /// </summary>
    [SerializeField]
    private List<GameObject> m_agents;

    /// <summary>
    /// The list of NPC agents associated with this director.
    /// </summary>
    [SerializeField]
    private Dictionary<int, NPCAgent> m_agentDictionary;


    /// <summary>
    /// Gets the list of agent GameObjects' registered unto this director.
    /// </summary>
    public List<GameObject> AgentGameObjects
    {
        get
        {
            return m_agentDictionary.Select(x => x.Value.gameObject).ToList();
        }
    }

    public List<GameObject> Targets
    {
        get
        {
            return m_targets;
        }
    }

    private int m_oldTargetCount;

    /// <summary>
    /// Constructor
    /// </summary>
    public NPCHubDirector()
    {
        m_targets = new List<GameObject>();
        m_agents = new List<GameObject>();
        m_agentDictionary = new Dictionary<int, NPCAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.UpdateAgentTargets();
        this.UpdateAllAgentLights();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_oldTargetCount != m_targets.Count)
        {
            this.InitialiseAgentDictionary();
            this.UpdateAgentTargets();
            this.UpdateAllAgentLights();
        }

        m_oldTargetCount = m_targets.Count;
    }

    private void InitialiseAgentDictionary()
    {
        m_agentDictionary = new Dictionary<int, NPCAgent>();

        for (int i = 0; i < m_agents.Count; i++)
        {
            var obj = m_agents[i];
            var agent = obj.GetComponent<NPCAgent>();
            var key = obj.GetInstanceID();

            m_agentDictionary.Add(key, agent);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="agent"></param>
    /// <returns></returns>
    public bool RegisterAgent(NPCAgent agent)
    {
        var key = agent.gameObject.GetInstanceID();

        if (!m_agentDictionary.ContainsKey(key))
        {
            m_agentDictionary.Add(key, agent);
            this.UpdateAgentTargets(agent);
            this.UpdateAllAgentLights();
            return true;
        }

        return false;
    }

    public void ClearAgents()
    {
        m_agents.Clear();
    }


    public void ClearTargets()
    {
        m_targets.Clear(); 
    }

    public bool RemovePlayerFromTargets()
    {
        return this.RemoveTarget(m_player);
    }

    public bool AddPlayerToTargets()
    {
        return this.AddTarget(m_player);
    }

    public bool RemoveTarget(GameObject target)
    {
        var key = target.GetInstanceID();
        var removedCount = m_targets.RemoveAll(x => x == null || x.GetInstanceID() == key);
        this.UpdateAgentTargets();
        this.UpdateAllAgentLights();

        return removedCount > 0;
    }

    public bool AddTarget(GameObject target)
    {
        foreach (var tgt in m_targets)
        {
            if (tgt.GetInstanceID() == target.GetInstanceID())
            {
                return false;
            }
        }

        m_targets.Add(target);
        return true;
    }

    private void UpdateAgentTargets(NPCAgent agent)
    {
        agent.ClearTargets();

        foreach (var target in m_targets)
        {
            if (target != null)
            {
                agent.AddTarget(target);
            }
        }
    }

    private void UpdateAgentTargets()
    {
        foreach (var kvp in m_agentDictionary)
        {
            this.UpdateAgentTargets(kvp.Value);
        }
    }

    public void UpdateAllAgentLights()
    {
        foreach (var kvp in m_agentDictionary)
        {
            var passive = !kvp.Value.Targets.Contains(m_player);
            this.UpdateAgentLight(kvp.Value, passive);
        }
    }

    private void UpdateAgentLight(NPCAgent agent, bool passive)
    {
        var lightCmp = agent.GetComponent<GuardLight>();

        if (lightCmp != null)
        {
            lightCmp.UsePassiveColour = passive;
        }
    }
}
