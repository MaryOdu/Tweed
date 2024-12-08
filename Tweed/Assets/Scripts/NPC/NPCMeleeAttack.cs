using Assets.Scripts.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;

namespace Assets.Scripts.NPC
{
    public class NPCMeleeAttack : MonoBehaviour
    {
        private GameObject m_target;

        [SerializeField]
        private float m_attackDamage;

        [SerializeField]
        private float m_attackCooldown;

        private float m_attackRange;

        private GameTimer m_attackCooldownTimer;

        private bool m_canAttack;
        private bool m_isAttacking;

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

        public float AttackRange
        {
            get
            {
                return m_attackRange;
            }
            set
            {
                m_attackRange = value;
            }
        }

        public bool IsAttacking => m_isAttacking;

        public NPCMeleeAttack()
        {
            m_attackCooldownTimer = new GameTimer();
            m_attackCooldownTimer.OnTimerElapsed += AttackCooldownTimer_OnTimerElapsed;
            m_attackCooldownTimer.AutoReset = false;
            m_attackDamage = 30;
            m_attackCooldown = 1.0f;
        }

        private void Start()
        {
            m_canAttack = true;
            m_attackCooldownTimer.SetTimeSpan(TimeSpan.FromSeconds(m_attackCooldown));
        }

        private void Update()
        {
            
            m_attackCooldownTimer.Tick();

            this.ProcessAttack();
        }

        private void ProcessAttack()
        {
            if (!m_canAttack || m_target == null)
            {
                m_isAttacking = false;
                return;
            }

            var dist = (m_target.transform.position - this.transform.position).magnitude;
            m_isAttacking = dist <= m_attackRange;

            if (m_isAttacking)
            {
                m_attackCooldownTimer.Start();
                m_canAttack = false;

                var health = m_target.GetComponent<Health>();

                if (health != null)
                {
                    // ATTACK!
                    health.RemoveHealth(m_attackDamage);
                }
            }
        }

        private void AttackCooldownTimer_OnTimerElapsed(object sender, TimerElapsedEventArgs e)
        {
            m_attackCooldownTimer.ResetTimer();

            if (m_target == null)
            {
                return;
            }

            var dirV = m_target.transform.position - this.transform.position;
            m_canAttack = dirV.magnitude < m_attackRange;

            if (m_canAttack)
            {
                m_attackCooldownTimer.Stop();
                m_canAttack = true;
            }
        }
    }
}
