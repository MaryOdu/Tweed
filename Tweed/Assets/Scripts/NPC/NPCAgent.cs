using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.NPC
{
    /// <summary>
    /// Base calss for 'NPC/AI' behaviour scripts.
    /// </summary>
    public class NPCAgent : MonoBehaviour
    {
        /// <summary>
        /// The navmesh agent - used for navigating between points along a given mesh.
        /// </summary>
        private NavMeshAgent m_navAgent;

        /// <summary>
        /// The dictionary of targets that the NPC Agent will look out for.
        /// </summary>
        private Dictionary<int, GameObject> m_targets;

        /// <summary>
        /// The 'NPCDirector' which will monitor this agent/npc.
        /// </summary>
        [SerializeField]
        private NPCDirector m_director;

        /// <summary>
        /// The maximum sight range of this agent/npc.
        /// </summary>
        [SerializeField]
        private float m_sightRange;

        /// <summary>
        /// The maximum sight angle of this agent/npc - in degrees. 
        /// </summary>
        [SerializeField]
        private float m_sightAngle;

        /// <summary>
        /// Gets the maximum sight range of this agent/npc.
        /// </summary>
        public float SightRange
        {
            get
            {
                return m_sightRange;
            }
        }

        /// <summary>
        /// Gets the maximum sight angle of this agent/npc - in degrees. 
        /// </summary>
        public float SightAngle
        {
            get
            {
                return m_sightAngle;
            }
        }

        public virtual bool IsStopped
        {
            get
            {
                return (m_navAgent?.velocity ?? Vector3.zero).magnitude < 1;
            }
        }

        /// <summary>
        /// Gets the navmesh agent.
        /// </summary>
        protected NavMeshAgent NavAgent
        {
            get
            {
                return m_navAgent;
            }
        }

        /// <summary>
        /// Gets the list of targets this agent is looking for.
        /// </summary>
        public List<GameObject> Targets
        {
            get
            {
                return m_targets.Values.Where(x => x != null).ToList();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public NPCAgent()
        {
            m_targets = new Dictionary<int, GameObject>();
        }

        /// <summary>
        /// Unitys' Start method - called before first frame in scene.
        /// </summary>
        protected virtual void Start()
        {
            m_navAgent = this.GetComponent<NavMeshAgent>();

            if (m_director != null)
            {
                m_director.RegisterAgent(this);
            }
        }

        protected virtual void Update()
        {
            var keys = m_targets.Where(x => x.Value == null).Select(x => x.Key).ToList();
            
            foreach(var key in keys)
            {
                m_targets.Remove(key);
            }
        }

        /// <summary>
        /// Sets the sight range and ange of this agent/npc.
        /// </summary>
        /// <param name="sightRange">the maximum sight range of this agent/npc</param>
        /// <param name="sightAngle">the maximum sight angle of this agent/npc - in degrees</param>
        public void SetSightParameters(float sightRange, float sightAngle)
        {
            m_sightAngle = sightAngle;
            m_sightRange = sightRange;
        }

        /// <summary>
        /// Clears all targets this agent is currently looking out for.
        /// </summary>
        public void ClearTargets()
        {
            m_targets.Clear();
        }

        /// <summary>
        /// Adds a target to the list of targets this agent is looking out for.
        /// </summary>
        /// <param name="target">Target to be added to this agents target dictionary.</param>
        /// <returns>Added? (true/false)</returns>
        public bool AddTarget(GameObject target)
        {
            if (target == null)
            {
                return false;
            }

            if (!m_targets.ContainsKey(target.GetInstanceID()))
            {
                m_targets.Add(target.GetInstanceID(), target);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the target from the list of targets this agent is looking out for.
        /// </summary>
        /// <param name="target">Target to be removed from this agents' target dictionary.</param>
        /// <returns>Removed? (true/false)</returns>
        public bool RemoveTarget(GameObject target)
        {
            var key = target.GetInstanceID();
            return m_targets.Remove(key);
        }

        /// <summary>
        /// Checks whether the provided game object is a target of this agent.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool CheckIfGameObjectIsTarget(GameObject target)
        {
            return m_targets.ContainsKey(target.GetInstanceID());
        }


        /// <summary>
        /// Gets the list of targets ordered by distance - closest first.
        /// </summary>
        /// <returns></returns>
        protected List<GameObject> GetTargetsByDistance()
        {
            return this.Targets.OrderBy(x => (this.transform.position - x.transform.position).sqrMagnitude).ToList();
        }
    }
}
