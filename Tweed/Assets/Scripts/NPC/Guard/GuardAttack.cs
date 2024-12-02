using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemy
{
    public class GuardAttack : MonoBehaviour
    {
        /// <summary>
        /// The current target of this attack behaviour.
        /// </summary>
        [SerializeField]
        private GameObject m_target;

        /// <summary>
        /// The current maximum attack range.
        /// </summary>
        [SerializeField]
        private float m_attackRange;

        /// <summary>
        /// Is currently attacking? (true/false)
        /// </summary>
        private bool m_isAttacking;

        /// <summary>
        /// Sets Object with Collider, Timer value, and bool for if it should be used
        /// </summary>
        [SerializeField] GameObject NPCAttHitBox;
        [SerializeField] float TimeToAttack = 1f;
        private bool Att = false;

        /// <summary>
        /// The navmesh agent
        /// </summary>
        private NavMeshAgent m_agent;

        /// <summary>
        /// Gets or sets this agents current target.
        /// </summary>
        public GameObject Target
        {
            get
            {
                return m_target;
            }
            set
            {
                m_target = value;
            }
        }

        /// <summary>
        /// Gets and sets this agents Collider object.
        /// </summary>
        public GameObject AttBox
        {
            get
            {
                return NPCAttHitBox;
            }
            set
            {
                NPCAttHitBox = value;
            }
        }

        /// <summary>
        /// Gets whether this agent is currently attacking.
        /// </summary>
        public bool IsAttacking
        {
            get
            {
                return m_isAttacking;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public GuardAttack()
        {
            m_attackRange = 2.0f;
        }

        /// <summary>
        /// Called before the entitys' first frame
        /// </summary>
        private void Start()
        {
            m_agent = this.GetComponent<NavMeshAgent>();
            NPCAttHitBox.GetComponent<BoxCollider>();
        }

        /// <summary>
        /// Stops the agent dead in its tracks
        /// </summary>
        private void Stop()
        {
            m_agent.SetDestination(this.transform.position);
        }

        /// <summary>
        /// Resumes the agents pursuit.
        /// </summary>
        private void Resume()
        {
            m_agent.SetDestination(m_target.transform.position);
        }

        /// <summary>
        /// Called on every update / frame.
        /// </summary>
        private void Update()
        {
            if (m_target != null)
            {
                if (m_target.transform.position != m_agent.destination)
                {
                    this.Resume();
                }

                m_agent.destination = m_target.transform.position;

                if (m_agent.remainingDistance <= m_attackRange)
                {
                    this.Stop();
                    m_isAttacking = true;
                    Debug.Log($"{this.gameObject.GetInstanceID()} : Agent has reached destination. Attacking target.");
                    Att = true;  
                }
                else
                {
                    this.Resume();
                    m_isAttacking = false;
                    Att = false;
                    
                }

                if (Att == true)
                {
                   
                    TimeToAttack -= Time.deltaTime;
                    if ((TimeToAttack <= 0.9f) && (TimeToAttack >= 0.8))
                    {
                       
                        //NPCAttHitBox.GetComponent<BoxCollider>().enabled = true;
                    }
                    //else { NPCAttHitBox.GetComponent<BoxCollider>().enabled = false; }
                   

                    if (TimeToAttack <= 0f)
                    {
                        TimeToAttack = 1f;
                    }
                }
                else
                {
                    //NPCAttHitBox.GetComponent<BoxCollider>().enabled = false;
                    TimeToAttack = 1f;
                }

            }

        }

        
    }
}
