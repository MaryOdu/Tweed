﻿using Assets.Scripts.Environment;
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

    internal class NPCSearch : MonoBehaviour
    {
        public event EventHandler OnSearchComplete;

        /// <summary>
        /// The guards current search area <see cref="SearchArea"/>
        /// </summary>
        private SearchArea m_searchArea;

        /// <summary>
        /// The agents navmesh
        /// </summary>
        private NavMeshAgent m_agent;

        /// <summary>
        /// The current list of earch points.
        /// </summary>
        private List<GameObject> m_searchPoints;

        /// <summary>
        /// The queue of search positions.
        /// </summary>
        private Queue<Vector3> m_searchQueue;

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

        [SerializeField]
        private float m_searchRadius;

        private float m_updateTime;

        private GameTimer m_updateTimer;

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

        /// <summary>
        /// Gets or sets the agents current search area.
        /// </summary>
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

        /// <summary>
        /// Constructor
        /// </summary>
        public NPCSearch()
        {
            m_searchQueue = new Queue<Vector3>();
            m_localSearchRadius = 10.0f;
            m_searchRadius = 100.0f;
            m_searchArea = SearchArea.Local;

            m_updateTimer = new GameTimer();
            m_updateTimer.OnTimerElapsed += this.UpdateTimer_OnTimerElapsed;
            m_updateTime = 1.0f;
        }

        /// <summary>
        /// Called once, before the first frame on scene.
        /// </summary>
        private void Start()
        {
            m_updateTimer.SetTimeSpan(TimeSpan.FromSeconds(m_updateTime));
            m_updateTimer.Start();

            m_agent = this.GetComponent<NavMeshAgent>();
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        private void Update()
        {
            m_updateTimer.Tick();
        }

        private void OnEnable()
        {
            if (m_agent != null)
            {
                m_agent.enabled = true;
            }
        }

        private void OnDisable()
        {
            if (m_agent != null)
            {
                m_agent.enabled = false;
            }
        }

        /// <summary>
        /// Gets all patrol points within the scene.
        /// </summary>
        /// <returns>A list of all patrol/nav points.</returns>
        private List<GameObject> GetNavPointsInRadius(float radius)
        {
            return Physics.OverlapSphere(this.transform.position, radius, LayerMask.GetMask("NavPoint")).Select(x => x.gameObject).ToList();
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
            if (m_target == null)
            {
                Debug.LogWarning("SearchPatrol -> Target is null!");

                this.OnSearchComplete?.Invoke(this, EventArgs.Empty);

                return;
            }

            var orderedPoints = m_searchPoints.OrderBy(x => (m_target.transform.position - x.transform.position).sqrMagnitude).ToList();

            var searchCount = Math.Max((int)((0.3f) * orderedPoints.Count), 1);

            for (int i = 0; i < 1; i++)
            {
                //orderedPoints.Sort(new RandomComparer());
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
        public void SearchNearbyArea()
        {
            m_searchPoints = this.GetNavPointsInRadius(m_searchRadius);

            if (m_searchPoints.Count < 1)
            {
                this.OnSearchComplete.Invoke(this, EventArgs.Empty);
            }

            this.SearchPatrol();
        }

        /// <summary>
        /// Agent will search through the entire level.
        /// </summary>
        public void SearchLargeArea()
        {
            m_searchPoints = this.GetNavPointsInRadius(m_searchRadius * 3);

            if (m_searchPoints.Count < 1)
            {
                this.OnSearchComplete.Invoke(this, EventArgs.Empty);
            }

            this.SearchPatrol();
        }

        private void UpdateTimer_OnTimerElapsed(object sender, TimerElapsedEventArgs e)
        {
            if (m_target == null)
            {
                return;
            }

            var health = m_target.GetComponent<Health>();

            if (health)
            {
                if (health.IsDead)
                {
                    this.OnSearchComplete?.Invoke(this, EventArgs.Empty);
                    m_target = null;
                    return;
                }
            }

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
                        Debug.Log("Searching local area");
                        this.SearchNearbyArea();
                    }
                    else
                    {
                        Debug.Log("Searching Level");
                        this.SearchLargeArea();
                        m_searchArea = SearchArea.Local;
                    }
                }
            }
        }
    }
}
