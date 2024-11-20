using Assets.Scripts.Enemy;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Timer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.NPC.Sentry
{
    public enum SentryState
    {
        Passive, 
        Alert
    }

    public class SentryAI : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> m_targets;

        private GameObject m_target;

        [SerializeField]
        private float m_sightRange;

        [SerializeField]
        private float m_sightAngle;

        [SerializeField]
        private float m_passiveRotationSpeed;

        [SerializeField]
        private float m_alertRotationSpeed;

        [SerializeField]
        private float m_lookTimeout;

        [SerializeField]
        private TimeSpan m_searchTime;

        [SerializeField]
        private List<GuardAI> m_guardList;

        [SerializeField]
        private List<GameObject> m_lookAtTargets;

        private GameObject m_currentLookAtTarget;
        private int m_currIdx;

        private SentryState m_state;
        private Timer m_lookTimer;

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

        public SentryState State
        {
            get
            {
                return m_state;
            }
        }

        public GameObject CurrentLookAtTarget
        {
            get
            {
                return m_currentLookAtTarget;
            }
        }

        public List<GameObject> Targets
        {
            get
            {
                return m_targets;
            }
        }

        public SentryAI()
        {
            m_guardList = new List<GuardAI>();
            m_lookAtTargets = new List<GameObject>();
            m_sightRange = 50.0f;
            m_sightAngle = 45.0f;
            m_lookTimeout = 3.0f;

            m_currIdx = 0;

            m_lookTimer = new Timer(TimeSpan.FromSeconds(m_lookTimeout));
            m_lookTimer.OnTimerElapsed += this.LookTimer_OnTimerElapsed;

            m_state = SentryState.Passive;
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            foreach (var target in m_targets)
            {
                var canSeeTarget = AIHelper.CanSeeObject(this.gameObject, target, m_sightRange, m_sightAngle, true);
                m_state = canSeeTarget ? SentryState.Alert : SentryState.Passive;

                if (canSeeTarget)
                {
                    m_target = target;
                }
            }

            switch (m_state)
            {
                case SentryState.Passive:
                    this.UpdateSentryPassive();
                    break;
                case SentryState.Alert:
                    this.UpdateSentryAlert();
                    break;
            }
        }

        private void UpdateSentryAlert()
        {
            this.RotateToFaceTarget(m_target, m_alertRotationSpeed);
            this.AlertGuards();
        }

        private void UpdateSentryPassive()
        {
            m_lookTimer.Tick();

            if (m_currentLookAtTarget == null)
            {
                m_lookTimer.Start();
            }

            if (m_currentLookAtTarget != null)
            {
                var rotationComplete = this.RotateToFaceTarget(m_currentLookAtTarget, m_passiveRotationSpeed);

                if (rotationComplete)
                {
                    m_lookTimer.Start();
                }
                else
                {
                    m_lookTimer.Stop();
                }
            }
        }

        

        private bool RotateToFaceTarget(GameObject target, float rotSpeed)
        {
            var deltaV = target.transform.position - this.transform.position;

            var tgtRot = Quaternion.LookRotation(deltaV, this.transform.up);

            var qRot = Quaternion.RotateTowards(this.transform.rotation, tgtRot, rotSpeed);

            this.transform.rotation = qRot;

            return (qRot.eulerAngles.y - tgtRot.eulerAngles.y) < rotSpeed;
        }

        private void AlertGuards()
        {
            foreach (var guard in m_guardList) 
            {
                guard.AlertGuard(this.gameObject);
            }
        }

        private GameObject GetNextLookatTarget()
        {
            m_currIdx = m_currIdx >= m_lookAtTargets.Count ? 0 : m_currIdx + 1;

            if (m_currIdx >= m_lookAtTargets.Count)
            {
                return null;
            }

            return m_lookAtTargets[m_currIdx];
        }

        private void LookTimer_OnTimerElapsed(object sender, TimerElapsedEventArgs e)
        {
            m_currentLookAtTarget = this.GetNextLookatTarget();
            m_lookTimer.Stop();
        }

    }
}
