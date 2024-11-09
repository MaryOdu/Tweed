using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class EnemyAnimation : MonoBehaviour
    {
        private Animator m_animator;
        private AgentAI m_agent;

        public EnemyAnimation()
        {
            
        }

        private void Start()
        {
            m_agent = this.GetComponent<AgentAI>();
            m_animator = this.GetComponent<Animator>();
        }

        private void Update()
        {
            //m_animator.ResetTrigger("IsAttacking");
            m_animator.SetBool("IsAttacking", false);

            switch (m_agent.State)
            {
                case EnemyState.Patrol:
                    m_animator.SetBool("IsPatroling", true);
                    m_animator.SetBool("IsSearching", false);
                    m_animator.SetBool("IsChasing", false);
                    break;
                case EnemyState.Search:
                    m_animator.SetBool("IsSearching", true);
                    m_animator.SetBool("IsPatroling", false);
                    m_animator.SetBool("IsChasing", false);
                    break;
                case EnemyState.Alert:

                    m_animator.SetBool("IsChasing", !m_agent.IsAttacking);
                    m_animator.SetBool("IsPatroling", false);
                    m_animator.SetBool("IsSearching", false);

                    if (m_agent.IsAttacking)
                    {
                        m_animator.SetBool("IsAttacking", true);
                    }
                    break;
            }
        }
    }
}
