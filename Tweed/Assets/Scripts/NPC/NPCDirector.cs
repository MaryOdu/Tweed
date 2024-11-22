using Assets.Scripts.NPC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDirector : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> m_targets;

    private Dictionary<int, NPCAgent> m_agents;

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
