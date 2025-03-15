using Assets.Scripts.Util;
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
        [SerializeField]
        private PlayerRespawn m_spawner;

        private Health m_health;

        [SerializeField]
        private GameObject m_UI;

        private GameObject m_uiInteract;


        public CharacterController()
        {
            
        }

        private void Start()
        {
            m_uiInteract = m_UI.FindChild("IntText");

            m_health = this.GetComponent<Health>();

            if (m_health == null)
            {
                m_health = this.AddComponent<Health>();
            }

            m_health.OnDeath += this.Health_OnDeath;
        }

        private void Update()
        {
            
        }

        void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Switch"))
            {
                this.ShowUISwitch();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    var controlPanel = other.GetComponent<ControlPanel>();
                    controlPanel.FireTrigger();
                }
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Switch"))
            {
                this.HideUISwitch();
            }
        }

        private void Health_OnDeath(object sender, EventArgs e)
        {
            Debug.Log("This calling Respawn: Character controler script");
            //GameObject.Destroy(this.gameObject);
            m_health.SetHealth(100);
            m_spawner.Respawn();
        }

        public void ShowUISwitch()
        {
            m_uiInteract.gameObject.SetActive(true);
        }

        public void HideUISwitch()
        {
            m_uiInteract.gameObject.SetActive(false);
        }
    }
}
