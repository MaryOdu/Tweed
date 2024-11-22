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
        private Animator m_animator;
        private GuardAI m_agent;

        public GuardAnimation()
        {
            
        }

        private void Start()
        {
            m_agent = this.GetComponent<GuardAI>();
            m_animator = this.GetComponent<Animator>();
        }

        private void Update()
        {
            //m_animator.ResetTrigger("IsAttacking");
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
