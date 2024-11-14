using Assets.Scripts.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Enemy
{
    public enum SearchArea
    {
        Local,
        Patrol,
        Level
    }

    internal class GuardSearch : MonoBehaviour
    {
        private SearchArea m_searchArea;
        private NavMeshAgent m_agent;
        private GuardPatrol m_patrol;

        private List<GameObject> m_searchPoints;

        private Queue<Vector3> m_searchQueue;

        [SerializeField]
        private GameObject m_target;

        [SerializeField]
        private float m_localSearchRadius;

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

        public SearchArea SearchArea
        {
            get
            {
                return m_searchArea;
            }
            set
            {
                m_searchArea = value;
            }
        }

        public GuardSearch()
        {
            m_searchQueue = new Queue<Vector3>();
            m_localSearchRadius = 10.0f;
            m_searchArea = SearchArea.Local;
        }

        private void Start()
        {
            m_patrol = this.GetComponent<GuardPatrol>();
            m_agent = this.GetComponent<NavMeshAgent>();
            m_searchPoints = m_patrol.PatrolPoints;
        }

        private void Update()
        {
            if (m_agent.remainingDistance <= m_agent.stoppingDistance)
            {
                if (!m_agent.hasPath || m_agent.velocity.sqrMagnitude == 0f)
                {
                    Debug.Log($"{gameObject.GetInstanceID()} : Agent has reached destination.");

                    if (m_searchQueue.Count > 0)
                    {
                        var nextPoint = m_searchQueue.Dequeue();
                        m_agent.destination = nextPoint;
                    }
                    else if (m_searchArea == SearchArea.Local)
                    {
                        Debug.Log("Searching Local Area");
                        this.LookAround();
                        m_searchArea = SearchArea.Patrol;
                    }
                    else if (m_searchArea == SearchArea.Patrol)
                    {
                        Debug.Log("Searching Patrol");
                        this.SearchPatrol();
                    }
                    //else
                    //{
                    //    Debug.Log("Searching Level");
                    //    this.SearchLevel();
                    //    m_searchArea = SearchArea.Local;
                    //}
                }
            }
        }

        private List<GameObject> GetAllNavPoints()
        {
            return FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .Where(x => x.GetComponent<PatrolPoint>() != null)
                .ToList();
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
        /// Agent will search through their entire patrol.
        /// </summary>
        public void SearchPatrol()
        {
            var orderedPoints = m_searchPoints.OrderBy(x => (m_target.transform.position - x.transform.position).sqrMagnitude).ToList();

            var searchCount = Math.Max((int)((0.3f) * orderedPoints.Count), 1);

            for (int i = 0; i < 2; i++)
            {
                orderedPoints.Sort(new RandomComparer());
                var points = orderedPoints.Take(searchCount).ToList();
                
                foreach (var point in points)
                {
                    m_searchQueue.Enqueue(point.transform.position);
                }
            }
        }

        /// <summary>
        /// Agent will search through the entire level.
        /// </summary>
        public void SearchLevel()
        {
            m_searchPoints = this.GetAllNavPoints();
            this.SearchPatrol();
        }
    }
}
