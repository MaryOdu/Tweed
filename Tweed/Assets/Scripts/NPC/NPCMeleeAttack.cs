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
        public event EventHandler OnAttack;

        private NPCAgent m_agent;

        //[SerializeField]
        //private float m_attackRange;

        [SerializeField]
        private float m_attackDamage;

        [SerializeField]
        private float m_attackCooldown;

        private GameTimer m_attackCooldownTimer;

        private bool m_canAttack;

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

            m_agent = this.GetComponent<NPCAgent>();
        }

        private void Update()
        {
            m_attackCooldownTimer.Tick();
        }

        private void AttackCooldownTimer_OnTimerElapsed(object sender, TimerElapsedEventArgs e)
        {
            m_canAttack = true;
            m_attackCooldownTimer.ResetTimer();
            m_attackCooldownTimer.Stop();
        }

        private void OnTriggerStay(Collider other)
        {
            var rhsObj = other.gameObject;
            var isTarget = m_agent.CheckIfGameObjectIsTarget(rhsObj);
            var health = rhsObj.GetComponent<Health>();

            if (isTarget && m_canAttack && health != null)
            {
                // ATTACK!
                m_canAttack = false;
                health.RemoveHealth(m_attackDamage);
                m_attackCooldownTimer.Start();
                this.OnAttack?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
