using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public static class BoundingSphereExtensions
    {
        public static bool IsWithinBounds(this BoundingSphere sphere, Vector3 point)
        {
            var deltaV = point - sphere.position;
            var distance = deltaV.magnitude;
            return distance < sphere.radius;
        }
    }
}
