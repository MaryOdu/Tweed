using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    internal class Health : MonoBehaviour
    {
        public event EventHandler OnDeath;

        [SerializeField]
        private float m_maxHealth;

        [SerializeField]
        private float m_currHealth;

        private bool m_isDead;

        public UIMenus Canvas;
        public PlayerRespawn PlayerCaught;

        public bool IsDead
        {
            get
            {
                return m_isDead;
            }
        }

        public Health()
        {
            m_maxHealth = 100.0f;
            m_currHealth = m_maxHealth;
        }

        private void Start()
        {
            m_currHealth = m_maxHealth;
        }

        public void AddHealth(float amount)
        {
            m_currHealth += Math.Abs(amount);
            m_isDead = m_currHealth <= 0;
        }

        public void RemoveHealth(float amount)
        {
            m_currHealth -= Math.Abs(amount);

            if (m_currHealth <= 0 && m_isDead == false)
            {
                m_isDead = true;
                //this.OnDeath?.Invoke(this, EventArgs.Empty);
                PlayerCaught.respawn();
                Canvas.CaughtScreen();
            }
        }
    }
}
