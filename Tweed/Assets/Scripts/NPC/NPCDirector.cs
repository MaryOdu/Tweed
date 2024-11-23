using Assets.Scripts.NPC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCDirector : MonoBehaviour
{
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
    }

    // Update is called once per frame
    void Update()
    {
        
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
            return true;
        }

        return false;
    }

    private void UpdateAgentTargets(NPCAgent agent)
    {
        foreach (var target in m_targets)
        {
            agent.AddTarget(target);
        }
    }

    private void UpdateAgentTargets()
    {
        foreach (var kvp in m_agents)
        {
            this.UpdateAgentTargets(kvp.Value);
        }
    }
}
