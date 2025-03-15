using Assets.Scripts.NPC;
using Assets.Scripts.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class SwitchController : MonoBehaviour
    {
        [SerializeField]
        private NPCDirector m_npcDirector;

        private void Start()
        {
            
        }

        private void Update()
        {
        }

        public void PassifyBots(GameObject npcDirector)
        {
            m_npcDirector.AddPlayerToHubTargets(npcDirector);
        }

        public void ToggleGate(GameObject gateObj)
        {
            var gate = gateObj.GetComponent<GateController>();

            if (gate.IsOpen)
            {
                gate.Close();
            }
            else
            {
                gate.Open();
            }
        }
    }
}
