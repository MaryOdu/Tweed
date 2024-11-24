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
        private NavMeshAgent m_agent;

        /// <summary>
        /// The dictionary of targets that the NPC Agent will look out for.
        /// </summary>
        private Dictionary<int, GameObject> m_targets;

        /// <summary>
        /// The 'NPCDirector' which will monitor this agent/npc.
        /// </summary>
        [SerializeField]
        private NPCDirector m_director;

        
        [SerializeField]
        private NPCDirector new_director; // (zac) Holds NPCDirector on another object
        private GuardLight new_Light; //

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

        /// <summary>
        /// Gets the navmesh agent.
        /// </summary>
        protected NavMeshAgent NavAgent
        {
            get
            {
                return m_agent;
            }
        }

        /// <summary>
        /// Gets the list of targets this agent is looking for.
        /// </summary>
        protected List<GameObject> Targets
        {
            get
            {
                return m_targets.Values.ToList();
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
            m_agent = this.GetComponent<NavMeshAgent>();

            new_Light = this.GetComponent<GuardLight>();

            if (m_director != null)
            {
                m_director.RegisterAgent(this);
            }
        }

        public void targetSwitch() // <-- (Zac) added this, Can find in "PLinter" script
        {
            m_director = new_director;
            Debug.Log("target should switch");
            new_Light.friendLight(); // <-- (Zac) Calls the "Gaurd Light" script on current object to change patrol light.
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
        /// Adds a target to the list of targets this agent is looking out for.
        /// </summary>
        /// <param name="target">Target to be added to this agents target dictionary.</param>
        /// <returns>Added? (true/false)</returns>
        public bool AddTarget(GameObject target)
        {
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
    }
}
