using Assets.Scripts.Util;
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

        [SerializeField]
        private float m_radius;

        public Scent()
        {
            m_xenoAttraction = 0.0f;
        }

        public float GetXenoAttraction(GameObject gameObj)
        {
            var baseAttraction = m_xenoAttraction * m_strength;
            var withinSmellRadius = this.GetSmellSphere().IsWithinBounds(gameObj.transform.position);
            return withinSmellRadius ? 0 : baseAttraction;
        }

        private BoundingSphere GetSmellSphere()
        {
            return new BoundingSphere(this.transform.position, m_radius);
        }
    }
}
