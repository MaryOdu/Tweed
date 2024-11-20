using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public static class AIHelper
    {
        //public struct CanSeeResult
        //{
        //    public bool Success;
        //    public Ray Ray;
        //    public RaycastHit HitInfo;
        //}

        public static bool CanSeeObject(GameObject agent, GameObject target, float sightRange, float sightAngle, bool drawDebug = false)
        {
            var tgtPos = target.gameObject.transform.position;
            var deltaV = tgtPos - agent.transform.position;
            var ray = new Ray(agent.transform.position, deltaV.normalized);

            var rayHit = Physics.Raycast(ray, out var hitInfo, float.PositiveInfinity, LayerMask.GetMask("Default"));

            var b = (tgtPos - agent.transform.position).normalized;
            var a = agent.transform.forward.normalized;
            var angle = Vector3.Angle(a, b);

            var result = rayHit && hitInfo.collider.gameObject == target && hitInfo.distance < sightRange && angle < sightAngle;

            if (drawDebug)
            {
                if (result)
                {
                    Debug.DrawRay(ray.origin, ray.direction * b.magnitude, Color.red);
                }
                else
                {
                    Debug.DrawRay(ray.origin, ray.direction * b.magnitude, Color.blue);
                }
            }



            return result;
        }
    }
}
