using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    internal class SpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private bool m_showAtRuntime;

        private MeshRenderer meshRenderer;

        public SpawnPoint()
        {

        }

        // Start is called before the first frame update
        void Start()
        {
            this.meshRenderer = this.GetComponent<MeshRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            this.meshRenderer.enabled = m_showAtRuntime;
        }
    }
}
