using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.NPC.Sentry
{
    public class SentryAI : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_target;

        [SerializeField]
        private float m_sightRange;

        [SerializeField]
        private float m_sightAngle;

        [SerializeField]
        private TimeSpan m_searchTime;

        [SerializeField]
        private float m_playerLastSeenTime;

        [SerializeField]
        private List<GameObject> m_lookAtTargets;

        private GameObject m_currentLookAtTarget;
        private int m_currIdx;

        public SentryAI()
        {
            m_lookAtTargets = new List<GameObject>();
            m_sightRange = 50.0f;
            m_sightRange = 45.0f;
            m_currIdx = 0;
        }

        private void Start()
        {

        }

        private void Update()
        {
            m_currentLookAtTarget = this.GetNextLookatTarget();
        }

        private GameObject GetNextLookatTarget()
        {
            m_currIdx = m_currIdx >= m_lookAtTargets.Count ? 0 : m_currIdx + 1;
            return m_lookAtTargets[m_currIdx];
        }
    }
}
