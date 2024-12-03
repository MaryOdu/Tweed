using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    private NavMeshAgent agent;
    Animator animator;

    GameObject Player;


    //[SerializeField] static float speedWalk = 2f;
    Vector3 walkTo;

    [SerializeField] LayerMask groundLayer, PlayerTar;
    [SerializeField] float attackRange;
    [SerializeField] bool PlayInAttRange;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Player = GameObject.Find("Player");
        animator = GetComponent<Animator>();
        //animator.SetBool("IsSearching", true); //Just to get it to the chaseing animation
        //agent.speed = speedWalk;
    }

    // Update is called once per frame
    void Update()
    {
        PlayInAttRange = Physics.CheckSphere(transform.position, attackRange, PlayerTar);

        GoToGate();

        if (PlayInAttRange) Attack();
        else
        {
            animator.SetBool("IsChaseing", true);
        }

    }

    private void GoToGate()
    {
        agent.SetDestination(Player.transform.position);

    }

    private void Attack()
    {
        animator.SetBool("IsChaseing", false);
        animator.SetTrigger("IsAttacking2");
        agent.SetDestination(transform.position);

    }

}