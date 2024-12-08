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
        /// <summary>
        /// Check to see whether a provided agent can see a given target.
        /// </summary>
        /// <param name="agent">The agent to be tested.</param>
        /// <param name="target">The target game object</param>
        /// <param name="sightRange">The agents' sight range.</param>
        /// <param name="sightAngle">The agents' sight angle.</param>
        /// <param name="drawDebug">Draws a debug line between the agent and the target.</param>
        /// <returns>Can see object? (true/false)</returns>
        public static bool CanSeeObject(GameObject agent, GameObject target, float sightRange, float sightAngle, LayerMask layerMask, bool drawDebug = false)
        {
            var tgtPos = target.gameObject.transform.position;
            var deltaV = tgtPos - agent.transform.position;

            if (deltaV.magnitude > sightRange)
            {
                return false;
            }

            var a = agent.transform.forward.normalized;
            var b = (deltaV).normalized;
            var angle = Vector3.Angle(a, b);

            if (angle > sightAngle)
            {
                return false;
            }

            var ray = new Ray(agent.transform.position, b);
            var rayHit = Physics.Raycast(ray, out var hitInfo, deltaV.magnitude, layerMask);

            var result = rayHit && hitInfo.collider.gameObject == target; //&& hitInfo.distance < sightRange && angle < sightAngle;

            if (drawDebug)
            {
                if (result)
                {
                    Debug.DrawRay(ray.origin, ray.direction * deltaV.magnitude, Color.red);
                }
                else
                {
                    Debug.DrawRay(ray.origin, ray.direction * deltaV.magnitude, Color.blue);
                }
            }



            return result;
        }
    }
}
