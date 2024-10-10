using System;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.Util
{
    public class Vector3Comparer : IComparer<Vector3>
    {
        public int Compare(Vector3 v1, Vector3 v2)
        {
            return Comparer<float>.Default.Compare(v1.x, v2.x)
                + Comparer<float>.Default.Compare(v1.y, v2.y)
                + Comparer<float>.Default.Compare(v1.z, v2.z);
        }
    }
}
