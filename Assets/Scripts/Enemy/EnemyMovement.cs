using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public Transform Target;
    public float UpdateRate = 0.1f; // how frequently to recalculate path based on Target transform's position

    private NavMeshAgent Agent;

    [SerializeField] private Animator Animator;
    private const string IsWalking = "IsWalking";
    //private const string IsAttacking = "IsAttacking";

    private Coroutine FollowCoroutine;
    
    private Transform targetTransform;
    private bool isStopped;
    public float movingTargetUpdateRate;
    //public bool stopSetDestination;

    private void Awake()
    {
        isStopped = false;
        //stopSetDestination = false;
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        
    }

    public void StartChasing()
    {
        if (FollowCoroutine == null)
        {
            targetTransform = Target.transform;
            movingTargetUpdateRate = UpdateRate;
            Debug.Log("StartedChasing");
            FollowCoroutine = StartCoroutine(FollowTarget());
        }
        else
        {
            Debug.LogWarning("Called StartChasing on Enemy that is already chasing! This is likely a bug in some calling class!");
        }
    }

    private void Update()
    {
        Animator.SetBool(IsWalking, Agent.velocity.magnitude > 0.01f);
        //Animator.SetBool(IsAttacking, Agent.velocity.magnitude < 0.01f && Agent.hasPath && Vector3.Distance(Agent.transform.position, Target.transform.position) <= Agent.stoppingDistance);

        //Debug.Log(Vector3.Distance(Agent.transform.position, Target.transform.position));

        //if (Vector3.Distance(Agent.transform.position, Target.transform.position) > 1.5f && !stopSetDestination)
        //{
        //    //StartCoroutine(FollowTarget());
        //}

        //if (!Target.gameObject.activeSelf)
        //{
        //    StopAllCoroutines();
        //}

        if (!Target.gameObject.activeSelf && !isStopped)
        {
            Debug.Log("stop");
            UpdateRate = movingTargetUpdateRate;
            StopCoroutine(FollowCoroutine);
            FollowCoroutine = null;
            Agent.isStopped = true;
            isStopped = true;
        }

        if (Target.gameObject.activeSelf && isStopped)
        {
            Debug.Log("play again");
            isStopped = false;
            StartChasing();
        }
    }

    private IEnumerator FollowTarget()
    {
        if (targetTransform.position == Target.position)
        {
            UpdateRate *= 10000;
        }

        WaitForSeconds Wait = new WaitForSeconds(UpdateRate);
        
        while (enabled && Target.gameObject.activeSelf)
        {
            Agent.SetDestination(Target.transform.position);
            Agent.isStopped = false;
            Debug.Log("aaa");
            yield return Wait;
        }

        if (!Target.gameObject.activeSelf)
        {
            Debug.Log("bb");
            UpdateRate = movingTargetUpdateRate;
            Agent.Stop();
            yield return null;
        }
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == Target)
        {
            stopSetDestination = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == Target)
        {
            stopSetDestination = false;
        }
    } */
}
