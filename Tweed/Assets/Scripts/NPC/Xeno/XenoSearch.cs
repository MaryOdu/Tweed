using Assets.Scripts.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.NPC.Xeno
{
    public class XenoSearch : MonoBehaviour
    {
        /// <summary>
        /// The queue of search positions.
        /// </summary>
        private Queue<Vector3> m_searchQueue;

        /// <summary>
        /// The agents navmesh
        /// </summary>
        private NavMeshAgent m_agent;


        /// <summary>
        /// The current target of the search function.
        /// </summary>
        [SerializeField]
        private GameObject m_target;

        /// <summary>
        /// The local search radius. The size of the area the agent will search when <see cref="SearchArea"/> is in Local mode.
        /// </summary>
        [SerializeField]
        private float m_localSearchRadius;


        /// <summary>
        /// Gets or sets the agents curreent target.
        /// </summary>
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

        public XenoSearch()
        {
            m_searchQueue = new Queue<Vector3>();
        }


        protected void Start()
        {
        }

        private void Update()
        {
            if (m_searchQueue.Count < 1)
            {
                this.SearchLevel();
            }

            if (m_agent.remainingDistance <= m_agent.stoppingDistance)
            {
                if (!m_agent.hasPath || m_agent.velocity.sqrMagnitude == 0f)
                {
                    if (m_searchQueue.Any())
                    {
                        var nextPoint = m_searchQueue.Dequeue();
                        m_agent.destination = nextPoint;
                    }
                }
            }
        }


        private void SearchLevel()
        {
            var allPoints = this.GetAllNavPoints();
            var point = allPoints.OrderBy(x => (m_target.transform.position - x.transform.position).magnitude).FirstOrDefault();

            if (point == null)
            {
                return;
            }

            m_searchQueue.Enqueue(point.transform.position);
        }

        /// <summary>
        /// Agent will perform a quick check of its surroundings.
        /// </summary>
        private void LookAround()
        {
            var currPos = this.gameObject.transform.position;

            var point1 = new Vector3(currPos.x + Random.Range(-m_localSearchRadius, m_localSearchRadius), currPos.y, currPos.z + Random.Range(-m_localSearchRadius, m_localSearchRadius));
            var point2 = new Vector3(currPos.x + Random.Range(-m_localSearchRadius, m_localSearchRadius), currPos.y, currPos.z + Random.Range(-m_localSearchRadius, m_localSearchRadius));
            var point3 = new Vector3(currPos.x + Random.Range(-m_localSearchRadius, m_localSearchRadius), currPos.y, currPos.z + Random.Range(-m_localSearchRadius, m_localSearchRadius));

            m_searchQueue.Enqueue(point1);
            m_searchQueue.Enqueue(point2);
            m_searchQueue.Enqueue(point3);
        }


        /// <summary>
        /// Gets all patrol points within the scene.
        /// </summary>
        /// <returns>A list of all patrol/nav points.</returns>
        private List<GameObject> GetAllNavPoints()
        {
            return GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .Where(x => x.GetComponent<NavPoint>() != null)
                .ToList();
        }
    }
}
