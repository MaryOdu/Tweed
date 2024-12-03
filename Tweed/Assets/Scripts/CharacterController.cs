using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Health))]
    public class CharacterController : MonoBehaviour
    {
        private Health m_health;

        public CharacterController()
        {
            
        }

        private void Start()
        {
            m_health = this.GetComponent<Health>();

            if (m_health == null)
            {
                m_health = this.AddComponent<Health>();
            }

            m_health.OnDeath += this.Health_OnDeath;
        }

        private void Health_OnDeath(object sender, EventArgs e)
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
