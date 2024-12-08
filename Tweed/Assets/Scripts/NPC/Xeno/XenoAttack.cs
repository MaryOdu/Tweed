using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AI;
using UnityEngine;

namespace Assets.Scripts.NPC.Xeno
{
    [RequireComponent(typeof(NPCMeleeAttack))]
    public class XenoAttack : MonoBehaviour
    {
        public event EventHandler<EventArgs> OnAttackComplete;

        /// <summary>
        /// The current target of this attack behaviour.
        /// </summary>
        [SerializeField]
        private GameObject m_target;

        /// <summary>
        /// The current maximum attack range.
        /// </summary>
        [SerializeField]
        private float m_attackRange;

        /// <summary>
        /// The navmesh agent
        /// </summary>
        private NavMeshAgent m_navAgent;
        private NPCMeleeAttack m_melee;

        /// <summary>
        /// Gets or sets this agents current target.
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
        /// Gets whether this agent is currently attacking.
        /// </summary>
        public bool IsMeleeAttacking
        {
            get
            {
                return m_melee?.IsAttacking ?? false;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public XenoAttack()
        {
            m_attackRange = 2.0f;
        }

        /// <summary>
        /// Called before the entitys' first frame
        /// </summary>
        protected void Start()
        {
            m_navAgent = this.GetComponent<NavMeshAgent>();
            m_melee = this.GetComponent<NPCMeleeAttack>();
            m_melee.AttackRange = m_attackRange;
        }

        /// <summary>
        /// Stops the agent dead in its tracks
        /// </summary>
        private void Stop()
        {
            m_navAgent.SetDestination(this.transform.position);
        }

        /// <summary>
        /// Resumes the agents pursuit.
        /// </summary>
        private void Resume()
        {
            m_navAgent.SetDestination(m_target.transform.position);
        }

        private void OnEnable()
        {
            if (m_melee == null)
            {
                m_melee = this.GetComponent<NPCMeleeAttack>();
            }

            m_melee.enabled = true;
        }

        private void OnDisable()
        {
            m_melee.enabled = false;
        }

        /// <summary>
        /// Called on every update / frame.
        /// </summary>
        private void Update()
        {
            if (!m_navAgent.enabled)
            {
                m_navAgent.enabled = true;
            }

            if (m_target != null)
            {
                var cmpHealth = m_target.GetComponent<Health>();

                if (cmpHealth && cmpHealth.IsDead)
                {
                    this.OnAttackComplete.Invoke(this, EventArgs.Empty);
                    return;
                }

                m_navAgent.stoppingDistance = m_attackRange;

                m_melee.Target = m_target;
                var dPos = this.transform.position - m_target.transform.position;
                m_navAgent.enabled = dPos.magnitude > m_attackRange;

                if (m_navAgent.enabled)
                {
                    m_navAgent.SetDestination(m_target.transform.position);
                }

                this.RotateToFaceTarget();
            }
        }

        private void RotateToFaceTarget()
        {
            var dPos = (m_target.transform.position - this.transform.position).normalized;
            dPos.y = 0;
            var qRot = Quaternion.LookRotation(dPos);

            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, qRot, Time.deltaTime);
        }
    }
}
