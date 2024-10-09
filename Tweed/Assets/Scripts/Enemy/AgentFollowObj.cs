using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemy
{
    public class AgentFollowObject : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_target;

        private NavMeshAgent m_agent;

        // Start is called before the first frame update
        void Start()
        {
            m_agent = GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            if (m_target != null)
            {
                m_agent.destination = m_target.transform.position;
            }
        }
    }
}