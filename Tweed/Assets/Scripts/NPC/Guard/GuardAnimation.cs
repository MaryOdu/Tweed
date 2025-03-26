using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class GuardAnimation : MonoBehaviour
    {
        /// <summary>
        /// The animator associated with this guard/npc
        /// </summary>
        private Animator m_animator;
        private Rigidbody m_rigidBody;
        [SerializeField] AudioSource WalkSound;
        [SerializeField] AudioSource RunSound;
        /// <summary>
        /// The AI/Behaviour/Agent associated with this guard/npc.
        /// </summary>
        private GuardAI m_agent;

        public GuardAnimation()
        {
            
        }

        /// <summary>
        /// Called before the first frame is called of whatever scene this entity is instantiated in.
        /// </summary>
        private void Start()
        {
            m_agent = this.GetComponent<GuardAI>();
            m_animator = this.GetComponent<Animator>();
            m_rigidBody = this.GetComponent<Rigidbody>();

            m_agent.OnMeleeAttack += this.Agent_OnMeleeAttack;
        }

        private void Agent_OnMeleeAttack(object sender, EventArgs e)
        {
            m_animator.Play("Attack1Armed");
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        private void Update()
        {
            if (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 && !m_animator.IsInTransition(0))
            {
                return;
            }

            //var planarVel = new Vector3(m_rigidBody.velocity.x, 0, m_rigidBody.velocity.z); // FML - Apparently agents have a vertical velocity of +600
            
            if (m_agent.IsStopped)
            {
                m_animator.Play("Idle");

                if (this.WalkSound)
                {
                    WalkSound.Pause();
                }
            }
            else
            {
                switch (m_agent.State)
                {
                    case GuardState.Patrol:
                        m_animator.Play("WalkForwardArmed");

                        if (this.WalkSound)
                        {
                            WalkSound.Play();
                        }
                        if (this.RunSound)
                        {
                            RunSound.Pause();
                        }
                       
                        break;
                    case GuardState.Search:
                        m_animator.Play("WalkForwardAiming");

                        if (this.WalkSound)
                        {
                            WalkSound.Play();
                        }
                        break;
                    case GuardState.Alert:

                        m_animator.Play("RunArmed");
                        break;
                }
            }

        }
    }
}
