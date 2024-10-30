using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public class TargetObjectComparer : GameObjectPositionComparer
    {
        private GameObject m_target;

        public TargetObjectComparer(GameObject target)
        {
            m_target = target;
        }

        public override int Compare(GameObject x, GameObject y)
        {
            var v1 = m_target.transform.position - x.transform.position;
            var v2 = m_target.transform.position - y.transform.position;

            return (int)(v1 - v2).magnitude;
        }
    }
}   
