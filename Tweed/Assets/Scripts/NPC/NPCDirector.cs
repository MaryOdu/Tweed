using Assets.Scripts.NPC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCDirector : MonoBehaviour
{
    [SerializeField]
    private GameObject m_player;

    /// <summary>
    /// The list of targets that this NPC Director is seeking to attack.
    /// </summary>
    [SerializeField]
    private List<GameObject> m_targets;

    /// <summary>
    /// The list of NPC agents associated with this director.
    /// </summary>
    private Dictionary<int, NPCAgent> m_agents;

    /// <summary>
    /// Gets the list of agent GameObjects' registered unto this director.
    /// </summary>
    public List<GameObject> AgentGameObjects
    {
        get
        {
            return m_agents.Select(x => x.Value.gameObject).ToList();
        }
    }

    private int m_oldTargetCount;

    /// <summary>
    /// Constructor
    /// </summary>
    public NPCDirector()
    {
        m_targets = new List<GameObject>();
        m_agents = new Dictionary<int, NPCAgent>();
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
            this.UpdateAgentTargets();
            this.UpdateAllAgentLights();
        }

        m_oldTargetCount = m_targets.Count;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="agent"></param>
    /// <returns></returns>
    public bool RegisterAgent(NPCAgent agent)
    {
        var key = agent.gameObject.GetInstanceID();

        if (!m_agents.ContainsKey(key))
        {
            m_agents.Add(key, agent);
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

    public bool RemoveTarget(GameObject target)
    {
        var key = target.GetInstanceID();
        var removedCount = m_targets.RemoveAll(x => x == null || x.GetInstanceID() == key);
        this.UpdateAgentTargets();
        this.UpdateAllAgentLights();

        return removedCount > 0;
    }

    public void AddTarget(GameObject target)
    {
        foreach (var tgt in m_targets)
        {
            if (tgt.GetInstanceID() == target.GetInstanceID())
            {
                return;
            }
        }

        m_targets.Add(target);
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
        foreach (var kvp in m_agents)
        {
            this.UpdateAgentTargets(kvp.Value);
        }
    }

    public void UpdateAllAgentLights()
    {
        foreach (var kvp in m_agents)
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
