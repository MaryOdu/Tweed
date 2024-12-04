using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class GuardAnimation : MonoBehaviour
    {
        /// <summary>
        /// The animator associated with this guard/npc
        /// </summary>
        private Animator m_animator;
        private Rigidbody m_rigidBody;

        /// <summary>
        /// The AI/Behaviour/Agent associated with this guard/npc.
        /// </summary>
        private GuardAI m_agent;

        public GuardAnimation()
        {
            
        }

        /// <summary>
        /// Called before the first frame is called of whatever scene this entity is instantiated in.
        /// </summary>
        private void Start()
        {
            m_agent = this.GetComponent<GuardAI>();
            m_animator = this.GetComponent<Animator>();
            m_rigidBody = this.GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        private void Update()
        {
            if (m_rigidBody.velocity.magnitude < 1)
            {
                m_animator.SetBool("IsIdle", true);

                m_animator.SetBool("IsPatroling", false);
                m_animator.SetBool("IsSearching", false);
                m_animator.SetBool("IsChasing", false);
                m_animator.SetBool("IsAttacking", m_agent.IsAttacking);
            }
            else
            {
                m_animator.SetBool("IsIdle", false);
                m_animator.SetBool("IsAttacking", false);

                switch (m_agent.State)
                {
                    case GuardState.Patrol:
                        m_animator.SetBool("IsPatroling", true);
                        m_animator.SetBool("IsSearching", false);
                        m_animator.SetBool("IsChasing", false);
                        break;
                    case GuardState.Search:
                        m_animator.SetBool("IsSearching", true);
                        m_animator.SetBool("IsPatroling", false);
                        m_animator.SetBool("IsChasing", false);
                        break;
                    case GuardState.Alert:

                        m_animator.SetBool("IsChasing", !m_agent.IsAttacking);
                        m_animator.SetBool("IsAttacking", m_agent.IsAttacking);
                        m_animator.SetBool("IsPatroling", false);
                        m_animator.SetBool("IsSearching", false);
                        break;
                }
            }

        }
    }
}
