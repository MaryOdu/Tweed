using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemy
{
    public class GuardPatrol : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> m_patrolPoints;

        private NavMeshAgent m_agent;
        private int m_currIdx;
        private GameObject m_destObj;

        public List<GameObject> PatrolPoints
        {
            get
            {
                return m_patrolPoints;
            }
            set
            {
                m_patrolPoints = value;
            }
        }


        public GuardPatrol()
        {
            m_patrolPoints = new List<GameObject>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            m_agent = GetComponent<NavMeshAgent>();
            m_patrolPoints = m_patrolPoints.Where(x => x != null).ToList();
        }

        // Update is called once per frame
        private void Update()
        {
            if (m_agent.remainingDistance <= m_agent.stoppingDistance)
            {
                if (!m_agent.hasPath || m_agent.velocity.sqrMagnitude == 0f)
                {
                    Debug.Log($"{gameObject.GetInstanceID()} : Agent has reached destination.");

                    var nextPoint = GetNextObject();

                    if (nextPoint != null)
                    {
                        m_agent.destination = nextPoint.transform.position;
                    }
                }
            }
        }

        private GameObject GetNextObject()
        {
            if (m_patrolPoints.Count < 1)
            {
                return null;
            }

            if (m_currIdx >= m_patrolPoints.Count - 1)
            {
                m_currIdx = 0;
            }
            else
            {
                m_currIdx++;

                while (m_patrolPoints[m_currIdx] == null) // Shouldn't be null, but hey-ho : Might as well check.
                {
                    m_currIdx++;
                }
            }

            return m_patrolPoints[m_currIdx];
        }
    }
}