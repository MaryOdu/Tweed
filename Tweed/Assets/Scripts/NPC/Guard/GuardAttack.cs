using Assets.Scripts.NPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemy
{
    enum AttackMode
    {
        Melee,
        Ranged
    }

    [RequireComponent(typeof(NPCMeleeAttack))]
    public class GuardAttack : MonoBehaviour
    {
        public event EventHandler OnAttackComplete;

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
        /// Is currently attacking? (true/false)
        /// </summary>
        private bool m_isAttacking;

        /// <summary>
        /// The attack mode this agent is currently in (currently only melee)
        /// </summary>
        private AttackMode m_attackMode;

        /// <summary>
        /// The navmesh agent
        /// </summary>
        private NavMeshAgent m_agent;

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
        public bool IsAttacking
        {
            get
            {
                return m_isAttacking;
            }
        }

        public float RemainingDistance
        {
            get
            {
                return m_agent?.remainingDistance ?? 0;
            }
        }

        /// <summary>
        /// Melee attacker component
        /// </summary>
        private NPCMeleeAttack m_meleeAtacker;

        /// <summary>
        /// Constructor
        /// </summary>
        public GuardAttack()
        {
            m_attackRange = 2.0f;
        }

        /// <summary>
        /// Called before the entitys' first frame
        /// </summary>
        private void Start()
        {
            m_agent = this.GetComponent<NavMeshAgent>();
            m_meleeAtacker = this.GetComponent<NPCMeleeAttack>();

            m_meleeAtacker.OnAttack += this.MeleeAtacker_OnAttack;
        }

        private void MeleeAtacker_OnAttack(object sender, EventArgs e)
        {
            m_isAttacking = true;
        }

        /// <summary>
        /// Stops the agent dead in its tracks
        /// </summary>
        private void Stop()
        {
            m_agent.SetDestination(this.transform.position);
        }

        /// <summary>
        /// Resumes the agents pursuit.
        /// </summary>
        private void Resume()
        {
            m_agent.SetDestination(m_target.transform.position);
            m_agent.stoppingDistance = m_attackRange;
        }

        /// <summary>
        /// Called on every update / frame.
        /// </summary>
        private void Update()
        {
            if (m_target != null)
            {
                var health = m_target.GetComponent<Health>();

                if (health)
                {
                    if (health.IsDead)
                    {
                        this.OnAttackComplete?.Invoke(this, EventArgs.Empty);
                        m_target = null;
                        return;
                    }
                }

                if (m_target.transform.position != m_agent.destination)
                {
                    this.Resume();
                }

                m_agent.destination = m_target.transform.position;

                if (m_agent.remainingDistance < m_attackRange)
                {
                    this.Stop();
                    Debug.Log($"{this.gameObject.GetInstanceID()} : Agent has reached destination. Attacking target.");
                }
                else
                {
                    this.Resume();
                }

                
            }

        }

        
    }
}
