using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemy
{
    public class GuardAttack : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_target;

        [SerializeField]
        private float m_attackRange;

        private bool m_isAttacking;

        private NavMeshAgent m_agent;

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

        public bool IsAttacking
        {
            get
            {
                return m_isAttacking;
            }
        }

        public GuardAttack()
        {
            m_attackRange = 2.0f;
        }

        private void Start()
        {
            m_agent = this.GetComponent<NavMeshAgent>();
        }

        private void Stop()
        {
            m_agent.SetDestination(this.transform.position);
        }

        private void Resume()
        {
            m_agent.SetDestination(m_target.transform.position);
        }

        private void Update()
        {
            if (m_target != null)
            {
                if (m_target.transform.position != m_agent.destination)
                {
                    this.Resume();
                }


                m_agent.destination = m_target.transform.position;

                if (m_agent.remainingDistance <= m_attackRange)
                {
                    this.Stop();
                    m_isAttacking = true;
                    Debug.Log($"{this.gameObject.GetInstanceID()} : Agent has reached destination. Attacking target.");
                }
                else
                {
                    this.Resume();
                    m_isAttacking = false;
                }
            }
        }
    }
}
