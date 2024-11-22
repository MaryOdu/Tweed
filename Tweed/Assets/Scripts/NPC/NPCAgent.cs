using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.NPC
{
    public class NPCAgent : MonoBehaviour
    {
        private NavMeshAgent m_agent;

        private Dictionary<int, GameObject> m_targets;

        [SerializeField]
        private NPCDirector m_director;

        [SerializeField]
        private float m_sightRange;

        [SerializeField]
        private float m_sightAngle;

        public float SightRange
        {
            get
            {
                return m_sightRange;
            }
        }

        public float SightAngle
        {
            get
            {
                return m_sightAngle;
            }
        }

        protected NavMeshAgent NavAgent
        {
            get
            {
                return m_agent;
            }
        }

        protected List<GameObject> Targets
        {
            get
            {
                return m_targets.Values.ToList();
            }
        }

        public NPCAgent()
        {
            m_targets = new Dictionary<int, GameObject>();
        }

        protected virtual void Start()
        {
            m_agent = this.GetComponent<NavMeshAgent>();
            m_director.RegisterAgent(this);
        }

        public void SetSightParameters(float sightRange, float sightAngle)
        {
            m_sightAngle = sightAngle;
            m_sightRange = sightRange;
        }

        public bool AddTarget(GameObject target)
        {
            if (!m_targets.ContainsKey(target.GetInstanceID()))
            {
                m_targets.Add(target.GetInstanceID(), target);
                return true;
            }

            return false;
        }
    }
}
