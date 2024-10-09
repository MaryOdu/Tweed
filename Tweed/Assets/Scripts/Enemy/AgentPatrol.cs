using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentPatrol : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> m_points;

    private NavMeshAgent m_agent;
    private int m_currIdx;
    private GameObject m_destObj;

    public AgentPatrol()
    {
        m_points = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_agent.remainingDistance <= m_agent.stoppingDistance)
        {
            if (!m_agent.hasPath || m_agent.velocity.sqrMagnitude == 0f)
            {
                var nextPoint = this.GetNextObject();
                m_agent.destination = nextPoint.transform.position;
            }
        }
    }

    public GameObject GetNextObject()
    {
        if (m_currIdx >= m_points.Count)
        {
            m_currIdx = 0;
        }
        else
        {
            m_currIdx++;

            while (m_points[m_currIdx] == null) // Shouldn't be null, but hey-ho : Might as well check.
            {
                m_currIdx++;
            }
        }
        
        return m_points[m_currIdx];
    }
}
