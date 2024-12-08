using Assets.Scripts.Environment;
using Assets.Scripts.Util;
using Cinemachine.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.NPC.Xeno
{
    public class XenoSwarm : MonoBehaviour
    {
        private const int LastKnownPositionSize  = 10;

        [SerializeField]
        private float m_swarmRadius;

        [SerializeField]
        private float m_smellRadius;

        [SerializeField]
        private float m_swarmSeperation;

        [SerializeField]
        [Range(0,100)]
        private float m_objectSightRange;

        //[SerializeField]
        //private bool m_enableCohesion;

        //[SerializeField]
        //private bool m_enableSwarmDir;

        //[SerializeField]
        //private bool m_enableSeperation;

        //[SerializeField]
        //private bool m_enableNoise;

        //[SerializeField]
        //private bool m_enableAvoidObjects;


        [SerializeField]
        [Range(0, 100)]
        private float m_coherance;

        [SerializeField]
        [Range(0, 100)]
        private float m_separation;

        [SerializeField]
        [Range(0, 200)]
        private float m_collisionAvoidance;

        [SerializeField]
        [Range(0, 100)]
        private float m_alignment;

        [SerializeField]
        [Range(0, 100)]
        private float m_noise;

        [SerializeField]
        [Range(0, 100)]
        private float m_smell;

        [SerializeField]
        [Range(0, 1000)]
        private float m_rotationSpeed;

        [SerializeField]
        [Range(0, 180)]
        private float m_sightAngle;

        [SerializeField]
        [ReadOnly(true)]
        private float m_boredom;

        private float m_movementSpeed;
        private Rigidbody m_rigidBody;
        private NavMeshAgent m_agent;
        private List<XenoAI> m_swarm;
        private List<Scent> m_smells;
        private Queue<Vector3> m_lastKnownPositions;

        private GameTimer m_thinkTimer;

        private float m_updateInterval;


        public float MovementSpeed
        {
            get
            {
                return m_movementSpeed;
            }
            set
            {
                m_movementSpeed = value;
            }
        }

        public XenoSwarm()
        {
            m_swarmRadius = 20.0f;
            m_swarmSeperation = 3.0f;
            m_rotationSpeed = 10.0f;
            m_movementSpeed = 1.0f;
            m_updateInterval = 0.5f;

            m_coherance = 20;
            m_alignment = 20;
            m_separation = 20;
            m_collisionAvoidance = 20;
            m_noise = 20;
            m_smell = 30;
            m_objectSightRange = 10;
            m_boredom = 0;
            m_smellRadius = 100.0f;
            m_sightAngle = 45f;

            m_swarm = new List<XenoAI>();
            m_smells = new List<Scent>();
            m_lastKnownPositions = new Queue<Vector3>(LastKnownPositionSize);

            m_thinkTimer = new GameTimer(TimeSpan.FromSeconds(m_updateInterval));
            m_thinkTimer.OnTimerElapsed += this.ThinkTimer_OnTimerElapsed;
        }

        protected void Start()
        {
            m_rigidBody = this.GetComponent<Rigidbody>();

            m_agent = this.GetComponent<NavMeshAgent>();

            m_thinkTimer.SetTimeSpan(TimeSpan.FromSeconds(m_updateInterval + Random.value));
            m_thinkTimer.Start();
        }

        private void Update()
        {
            if (m_agent.enabled)
            {
                m_agent.enabled = false;
            }

            m_thinkTimer.Tick();

            var forward = this.transform.forward;
            var separation = this.GetSeperationDirection(m_swarm);
            var objSeparation = this.GetCollisionAvoidanceDirection();
            var swarmDir = this.GetSwarmDirection(m_swarm);
            var cohesion = this.GetCohesionDirection(m_swarm);
            var fwd = m_rigidBody.velocity.magnitude > 0.0f ? m_rigidBody.velocity : this.transform.forward;
            var noise = new Vector3(fwd.x + UnityEngine.Random.Range(-1, 1), 0, fwd.z +  UnityEngine.Random.Range(-1, 1)).normalized;
            var smell = this.GetSmellDirection(m_smells);

            var dir = Vector3.zero;

            m_boredom -= 0.03f * Time.deltaTime;

            m_boredom = Math.Clamp(m_boredom, 0, 1);

            dir += separation * m_separation * (1f + m_boredom);
            dir += objSeparation * m_collisionAvoidance;
            dir += swarmDir * m_alignment;
            dir += (cohesion * m_coherance) * (1f - m_boredom);
            dir += noise * m_noise * (1f + m_boredom);
            dir += smell * m_smell * (1f - m_boredom);

            m_swarmRadius += 0.01f;
            m_swarmRadius = Mathf.Clamp(m_swarmRadius, 5, 100);
            this.MoveInDirection(dir.normalized);
            this.UpdateRotation();
        }

        private Vector3 GetSmellDirection(List<Scent> scents)
        {
            var result = Vector3.zero;

            foreach(var scent in scents)
            {
                if (scent != null)
                {
                    var deltaV = scent.transform.position - this.transform.position;
                    var distance = deltaV.sqrMagnitude;
                    result += (scent.GetXenoAttraction(this.gameObject) * 1/distance) * deltaV;
                }

            }

            return result.normalized;
        }


        /// <summary>
        /// Updates the cohesion between swarm members
        /// </summary>
        /// <param name="swarm">Nearby xenos'</param>
        private Vector3 GetCohesionDirection(List<XenoAI> swarm)
        {
            var swarmPosition = this.GetSwarmPosition(swarm);
            var result = (swarmPosition - this.transform.position).normalized;
            return result.normalized;
        }

        /// <summary>
        /// Collision avoidance direction
        /// </summary>
        /// <param name="gameObjects"></param>
        /// <returns></returns>
        private Vector3 GetCollisionAvoidanceDirection()
        {
            var result = Vector3.zero;

            var frontRight = Quaternion.Euler(0, 45, 0) * this.transform.forward;
            var frontLeft = Quaternion.Euler(0, -45, 0) * this.transform.forward;

            bool hitf = Physics.Raycast(this.transform.position, this.transform.forward, out var hitInfo1, m_objectSightRange, LayerMask.GetMask("Default"));
            bool hitfr = Physics.Raycast(this.transform.position, frontRight, out var hitInfo2, m_objectSightRange, LayerMask.GetMask("Default"));
            bool hitfl = Physics.Raycast(this.transform.position, frontLeft, out var hitInfo3, m_objectSightRange, LayerMask.GetMask("Default"));

            if (hitf)
            {
                var dV = this.transform.position - hitInfo1.point;
                var distance = dV.magnitude;
                result += (dV * 1 / distance);
            }

            if (hitfr)
            {
                var dV = this.transform.position - hitInfo3.point;
                var distance = dV.magnitude;

                result += (dV * 1 / distance);
            }
            else if (hitfl)
            {
                var dV = this.transform.position - hitInfo2.point;
                var distance = dV.magnitude;
                result += this.transform.right;
            }

            return result.normalized;
        }

        private Vector3 GetVelocity()
        {
            return new Vector3(m_rigidBody.velocity.x, 0, m_rigidBody.velocity.z);
        }

        /// <summary>
        /// Updates the seperation between swarm members
        /// </summary>
        /// <param name="swarm">Nearby xenos'</param>
        private Vector3 GetSeperationDirection(List<XenoAI> swarm)
        {
            var result = Vector3.zero;

            foreach (var xeno in swarm)
            {
                if (xeno != null)
                {
                    if (xeno.gameObject == this.gameObject)
                    {
                        continue;
                    }
                    var deltaV = this.transform.position - xeno.transform.position;
                    var distance = deltaV.magnitude;

                    if (distance < m_swarmSeperation)
                    {
                        deltaV = deltaV / (distance < 1f ? 1.0f : distance);
                        result += deltaV;
                    }
                }
            }

            return result.normalized;
        }

        /// <summary>
        /// Gets the average heading of the nearby swarm
        /// </summary>
        /// <param name="swarm">List of local xenos</param>
        /// <returns>The average heading of local xenos</returns>
        private Vector3 GetSwarmDirection(List<XenoAI> swarm)
        {
            var result = Vector3.zero;

            foreach (var xeno in swarm)
            {
                if (xeno != null)
                {
                    result += xeno.Velocity;
                }
            }

            return result.normalized;
        }

        /// <summary>
        /// Gets the average position of the nearby swarm
        /// </summary>
        /// <param name="xenos">List of local xenos</param>
        /// <returns>Average position of local xenos</returns>
        private Vector3 GetSwarmPosition(List<XenoAI> xenos)
        {
            var result = Vector3.zero;

            foreach (var xeno in xenos)
            {
                if (xeno != null)
                {
                    var pos = xeno.transform.position;
                    result += new Vector3(pos.x, 0, pos.z);
                }
            }

            result /= xenos.Count;

            return result;
        }

        private void MoveInDirection(Vector3 dir)
        {
            dir.y = 0;
            var vel = this.GetVelocity();
            m_rigidBody.velocity = Vector3.Lerp(vel, dir * m_movementSpeed, Time.deltaTime);
        }

        private void UpdateRotation()
        {
            var tgtRot = Quaternion.LookRotation(m_rigidBody.velocity, Vector3.up);
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, tgtRot, m_rotationSpeed * Time.deltaTime);
        }

        private List<GameObject> GetSurroundingObjects()
        {
            var result = new List<GameObject>();
            var colliders = Physics.OverlapSphere(this.transform.position, m_swarmRadius, LayerMask.GetMask("Default"));

            foreach (var obj in colliders)
            {
                result.Add(obj.gameObject);
            }

            return result;
        }

        private float GetBoredom()
        {
            float result = 0f;

            var currPos = this.transform.position;

            if (m_lastKnownPositions.Count >= LastKnownPositionSize)
            {
                m_lastKnownPositions.Dequeue();
            }

            m_lastKnownPositions.Enqueue(currPos);

            foreach(var pos in m_lastKnownPositions)
            {
                var deltaV = pos - currPos;

                if (deltaV.magnitude < (m_movementSpeed * Time.deltaTime))
                {
                    result += 0.02f;
                }
            }

            return Mathf.Clamp(result, 0, 1f);
        }

        /// <summary>
        /// Returns a list of all surrounding xenos'
        /// </summary>
        /// <returns></returns>
        private List<XenoAI> GetSurroundingXenos()
        {
            var result = new List<XenoAI>();

            var nearByXenoObjects = Physics.OverlapSphere(this.transform.position, m_swarmRadius, LayerMask.GetMask("Xeno"));

            foreach (var obj in nearByXenoObjects)
            {
                var canSeeFriendly = AIHelper.CanSeeObject(this.gameObject, obj.gameObject, m_swarmRadius, m_sightAngle, LayerMask.GetMask("Xeno"), true);

                if (canSeeFriendly)
                {
                    var xAi = obj.GetComponent<XenoAI>();

                    if (xAi != null)
                    {
                        result.Add(xAi);
                    }
                }
            }

            return result;
        }

        public List<Scent> GetSurroundingScents()
        {
            var result = new List<Scent>();

            var neabyScentObjects = Physics.OverlapSphere(this.transform.position, m_smellRadius, LayerMask.GetMask("Default"));

            foreach (var obj in neabyScentObjects)
            {
                var scent = obj.GetComponent<Scent>();

                if (scent != null)
                {
                    result.Add(scent);
                }
            }

            return result;
        }

        private void ThinkTimer_OnTimerElapsed(object sender, TimerElapsedEventArgs e)
        {
            m_boredom += this.GetBoredom();
            m_swarm = this.GetSurroundingXenos();
            m_smells = this.GetSurroundingScents();
            m_swarmRadius = 100 * (1f - m_boredom);
        }

    }
}
