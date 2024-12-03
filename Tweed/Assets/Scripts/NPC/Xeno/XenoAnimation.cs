using Assets.Scripts.Enemy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.NPC.Xeno
{
    public class XenoAnimation : MonoBehaviour
    {
        private Animator m_animator;
        private Rigidbody m_rigidBody;
        private XenoAI m_agent;

        public XenoAnimation()
        {
            
        }

        /// <summary>
        /// Called before the first frame is called of whatever scene this entity is instantiated in.
        /// </summary>
        private void Start()
        {
            m_agent = this.GetComponent<XenoAI>();
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
            }
            else
            {
                m_animator.SetBool("IsIdle", false);

                switch (m_agent.State)
                {
                    case XenoState.Swarm:
                        m_animator.SetBool("IsPatroling", false);
                        m_animator.SetBool("IsSearching", false);
                        m_animator.SetBool("IsSwarming", true);
                        break;
                    case XenoState.Search:
                        m_animator.SetBool("IsSearching", true);
                        m_animator.SetBool("IsPatroling", false);
                        m_animator.SetBool("IsSwarming", false);
                        break;
                    case XenoState.Alert:
                        m_animator.SetBool("IsChasing", !m_agent.IsAttacking);
                        m_animator.SetBool("IsAttacking", m_agent.IsAttacking);
                        m_animator.SetBool("IsPatroling", false);
                        m_animator.SetBool("IsSearching", false);
                        m_animator.SetBool("IsSwarming", false);
                        break;
                }
            }
        }
    }
}
