using Assets.Scripts.NPC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemy
{
    public class GuardPatrol : MonoBehaviour
    {
        /// <summary>
        /// List of patrol points this guard has.
        /// </summary>
        [SerializeField]
        private List<GameObject> m_patrolPoints;

        /// <summary>
        /// List of patrol points that form a complete route back and forth
        /// </summary>
        private List<GameObject> m_completePatrol;

        /// <summary>
        /// The navmesh agent.
        /// </summary>
        private NavMeshAgent m_navAgent;

        /// <summary>
        /// The index of the current patrol point.
        /// </summary>
        private int m_currIdx;

        /// <summary>
        /// The current destination patrol point.
        /// </summary>
        private GameObject m_destObj;

        /// <summary>
        /// Gets or sets the list of patrol points this guard has.
        /// </summary>
        public List<GameObject> PatrolPoints
        {
            get
            {
                return m_patrolPoints;
            }
            set
            {
                m_patrolPoints = value;
                m_completePatrol =  this.CreateCompletePatrol();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GuardPatrol()
        {
            m_patrolPoints = new List<GameObject>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            m_navAgent = GetComponent<NavMeshAgent>();
            m_patrolPoints = m_patrolPoints.Where(x => x != null).ToList();
            m_completePatrol = this.CreateCompletePatrol();
        }

        private void OnEnable()
        {
            if (m_navAgent == null)
            {
                m_navAgent = GetComponent<NavMeshAgent>();
            }

            m_navAgent.enabled = true;
        }

        private void OnDisable()
        {
            m_navAgent.enabled = false;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!m_navAgent.enabled)
            {
                m_navAgent.enabled = true;
            }

            if (m_navAgent.remainingDistance <= m_navAgent.stoppingDistance)
            {
                if (!m_navAgent.hasPath || m_navAgent.velocity.sqrMagnitude == 0f)
                {
                    Debug.Log($"{gameObject.GetInstanceID()} : Agent has reached destination.");

                    var nextPoint = this.GetNextPatrolPointGameObject();

                    if (nextPoint != null)
                    {
                        m_navAgent.destination = nextPoint.transform.position;
                    }
                }
            }
        }

        private List<GameObject> CreateCompletePatrol()
        {
            var result = new List<GameObject>();

            if (!m_patrolPoints.Any())
            {
                return result;
            }

            var patrolPoints = new List<GameObject>(m_patrolPoints);

            var startEnd = m_patrolPoints[0];
            var middle = m_patrolPoints[m_patrolPoints.Count - 1];

            result.AddRange(patrolPoints);
            patrolPoints.Remove(startEnd);
            patrolPoints.Remove(middle);
            patrolPoints.Reverse();
            result.AddRange(patrolPoints);

            return result;
        }

        /// <summary>
        /// Gets the next patrol point
        /// </summary>
        /// <returns>Patrol point game object</returns>
        private GameObject GetNextPatrolPointGameObject()
        {
            if (m_completePatrol.Count < 1)
            {
                return null;
            }

            if (m_currIdx >= m_completePatrol.Count - 1)
            {
                m_currIdx = 0;
            }
            else
            {
                m_currIdx++;

                while (m_completePatrol[m_currIdx] == null) // Shouldn't be null, but hey-ho : Might as well check.
                {
                    m_currIdx++;
                }
            }

            return m_completePatrol[m_currIdx];
        }
    }
}