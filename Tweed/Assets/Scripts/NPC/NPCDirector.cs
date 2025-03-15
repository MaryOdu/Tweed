using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.NPC
{
    public class NPCDirector : MonoBehaviour
    {
        [SerializeField]
        private Dictionary<string, NPCHubDirector> m_hubDirectors;

        public NPCDirector()
        {
            m_hubDirectors = new Dictionary<string, NPCHubDirector>();
        }

        private void Start()
        {
            this.Initialise();
        }

        private void Update()
        {
            
        }

        private void Initialise()
        {
            var hubDirs = this.GetComponentsInChildren<NPCHubDirector>();

            foreach(var hub in hubDirs)
            {
                m_hubDirectors.Add(hub.gameObject.name, hub);
            }
        }

        public void ClearPlayerFromHubTargets(GameObject npcHub)
        {
            m_hubDirectors[npcHub.gameObject.name].RemovePlayerFromTargets();
        }

        public void AddPlayerToHubTargets(GameObject npcHub)
        {
            m_hubDirectors[npcHub.gameObject.name].RemovePlayerFromTargets();
        }
    }
}
