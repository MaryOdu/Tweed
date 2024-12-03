using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    public class Scent : MonoBehaviour
    {
        [SerializeField]
        [Range(-100,100)]
        private float m_xenoAttraction = 0.0f;

        [SerializeField]
        [Range(0f, 1f)]
        private float m_strength;

        public float XenoAttraction
        {
            get
            {
                return m_xenoAttraction * m_strength;
            }
        }

        public Scent()
        {
            m_xenoAttraction = 0.0f;
        }
    }
}
