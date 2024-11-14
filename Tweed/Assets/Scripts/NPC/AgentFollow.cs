using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemy
{
    public class AgentFollow : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_target;

        private NavMeshAgent m_agent;

        public GameObject Target
        {
            get
            {
                return m_target;
            }
            set
            {
                m_target = value;
            }
        }

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

                if (m_agent.remainingDistance <= m_agent.stoppingDistance)
                {
                    if (!m_agent.hasPath || m_agent.velocity.sqrMagnitude == 0f)
                    {
                        Debug.Log($"{this.gameObject.GetInstanceID()} : Agent has reached destination.");
                    }
                }
            }
        }
    }
}