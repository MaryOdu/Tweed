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
            return (int)(x.transform.position - m_target.transform.position).sqrMagnitude;
        }
    }
}   
