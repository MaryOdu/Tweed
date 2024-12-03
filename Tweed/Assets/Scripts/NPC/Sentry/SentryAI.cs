using Assets.Scripts.Enemy;
using Assets.Scripts.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Assets.Scripts.NPC.Sentry
{
    public enum SentryState
    {
        Passive, 
        Alert
    }

    /// <summary>
    /// A script for managing the behaviour of a given sentry entity.
    /// </summary>
    public class SentryAI : NPCAgent
    {
        /// <summary>
        /// The target the sentry is currently after.
        /// </summary>
        private GameObject m_target;

        /// <summary>
        /// The rotation speed of the sentry when in passive mode.
        /// </summary>
        [SerializeField]
        private float m_passiveRotationSpeed;

        /// <summary>
        /// The rotation speed of the sentry when in alert mode.
        /// </summary>
        [SerializeField]
        private float m_alertRotationSpeed;

        /// <summary>
        /// The amount of time (in seconds) the sentry will observe any one lookAt target.
        /// </summary>
        [SerializeField]
        private float m_lookTimeout;

        /// <summary>
        /// The list of guard agents that are tied to this sentry. When this sentry goes into 'alert' mode, it will notify the guards within this list.
        /// </summary>
        [SerializeField]
        private List<GuardAI> m_guardList;

        /// <summary>
        /// The list of areas for the sentry to observe when in passive mode.
        /// </summary>
        [SerializeField]
        private List<GameObject> m_lookAtTargets;

        /// <summary>
        /// The current 'lookAt' target the sentry is looking at.
        /// </summary>
        private GameObject m_currentLookAtTarget;
        
        /// <summary>
        /// The index of the current 'lookAt' target.
        /// </summary>
        private int m_currIdx;

        /// <summary>
        /// The current sentry state. Passive/Alert.
        /// </summary>
        private SentryState m_state;

        /// <summary>
        /// The timer for the amount of time the sentry will look at any particular 'lookAt' target
        /// </summary>
        private GameTimer m_lookTimer;

        /// <summary>
        /// The sentry's current state.
        /// </summary>
        public SentryState State
        {
            get
            {
                return m_state;
            }
        }

        /// <summary>
        /// The current 'lookAt' target the sentry is looking toward.
        /// </summary>
        public GameObject CurrentLookAtTarget
        {
            get
            {
                return m_currentLookAtTarget;
            }
        }

        /// <summary>
        /// Constructor & Initialisation
        /// </summary>
        public SentryAI()
            : base()
        {
            m_guardList = new List<GuardAI>();
            m_lookAtTargets = new List<GameObject>();
            m_lookTimeout = 3.0f;

            m_currIdx = 0;

            m_lookTimer = new GameTimer(TimeSpan.FromSeconds(m_lookTimeout));
            m_lookTimer.OnTimerElapsed += this.LookTimer_OnTimerElapsed;

            m_state = SentryState.Passive;
            this.SetSightParameters(50.0f, 45.0f);
        }

        /// <summary>
        /// Called before first frame in Unity scene.
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Called every frame in Unity scene.
        /// </summary>
        protected override void Update()
        {
            foreach (var target in this.Targets)
            {
                if (target == null)
                {
                    continue;
                }

                var canSeeTarget = AIHelper.CanSeeObject(this.gameObject, target, this.SightRange, this.SightAngle, true);
                m_state = canSeeTarget ? SentryState.Alert : SentryState.Passive;

                if (canSeeTarget)
                {
                    m_target = target;
                    break;
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

            base.Update();
        }

        /// <summary>
        /// Sentry Alert behaviour. Will 1) Rotate to face target. 2) Alert all guards associated with this sentry.
        /// </summary>
        private void UpdateSentryAlert()
        {
            this.RotateToFaceTarget(m_target, m_alertRotationSpeed);
            this.AlertGuards();
        }

        /// <summary>
        /// Sentry passive behaviour. Will cycle though all 'lookAt' targets until it finds a target that will put it into alert mode.
        /// </summary>
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
                    m_lookTimer.ResetTimer();
                }
            }
        }

        
        /// <summary>
        /// Causes the sentry to rotate to face the target supplied.
        /// </summary>
        /// <param name="target">The target to rotate the sentry toward.</param>
        /// <param name="rotSpeed">The rotation speed by which to rotate.</param>
        /// <returns></returns>
        private bool RotateToFaceTarget(GameObject target, float rotSpeed)
        {
            var deltaV = target.transform.position - this.transform.position;

            var tgtRot = Quaternion.LookRotation(deltaV.normalized, this.transform.up);

            var qRot = Quaternion.RotateTowards(this.transform.rotation, tgtRot, rotSpeed);

            this.transform.rotation = qRot;

            return (qRot.eulerAngles.y - tgtRot.eulerAngles.y) < rotSpeed;
        }

        /// <summary>
        /// Allerts all assocaited guards.
        /// </summary>
        private void AlertGuards()
        {
            foreach (var guard in m_guardList) 
            {
                guard.AlertGuard(this.gameObject);
            }
        }

        /// <summary>
        /// Gets the next 'lookAt' target.
        /// </summary>
        /// <returns>'lookAt' target GameObject</returns>
        private GameObject GetNextLookatTarget()
        {
            m_currIdx = m_currIdx >= m_lookAtTargets.Count ? 0 : m_currIdx + 1;

            if (m_currIdx >= m_lookAtTargets.Count)
            {
                return null;
            }

            return m_lookAtTargets[m_currIdx];
        }

        /// <summary>
        /// Fired after the sentry has looked at a particular 'lookAt' target for a fixed period of time.
        /// </summary>
        /// <param name="sender">Method invoker</param>
        /// <param name="e">Event args</param>
        private void LookTimer_OnTimerElapsed(object sender, TimerElapsedEventArgs e)
        {
            m_currentLookAtTarget = this.GetNextLookatTarget();
            m_lookTimer.Stop();
        }

    }
}
