using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public Transform Target;
    public float UpdateSpeed = 0.1f; // how frequently to recalculate path based on Target transform's position

    private NavMeshAgent Agent;

    [SerializeField] private Animator Animator;
    private const string IsWalking = "IsWalking";
    private const string IsAttacking = "IsAttacking";

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        StartCoroutine(FollowTarget());
    }

    private void Update()
    {
        Animator.SetBool(IsWalking, Agent.velocity.magnitude > 0.01f);
        Animator.SetBool(IsAttacking, Agent.velocity.magnitude < 0.01f && Agent.hasPath);

        if (Vector3.Distance(Agent.transform.position, Target.transform.position) > 1.5f)
        {
            StartCoroutine(FollowTarget());
        }
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateSpeed);

        while (enabled)
        {
            Agent.SetDestination(Target.transform.position);

            yield return Wait;
        }
    }
}
