using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class enemyAInew : MonoBehaviour
{
    [SerializeField] private Transform player, manager;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;

    private Vector3 walkPoint, playerPos;
    private bool walkPointSet, alreadyAttacked;
    [SerializeField] private float walkPointRange, timeBetweenAttacks, sightRange, attackRange;

    [SerializeField]
    private Animator movingViewAnimations;
    private float angle, SideAngle;

    private RaycastHit hit;
    [SerializeField] private HideAfterTime enemyProjectile;
    private HideAfterTime projectile;
    private static poolMono<HideAfterTime> enemyProjectiles;
    [SerializeField] private Transform bulletSpawn;

    private void Start()
    {
        enemyProjectiles = new poolMono<HideAfterTime>(enemyProjectile,20,manager,true);
    }

    private void Update()
    {
        movingViewAnimations.transform.LookAt(player);

        if (!(Physics.CheckSphere(transform.position, sightRange, whatIsPlayer))) //if not in sightRange
            Patroling();
        else if (!(Physics.CheckSphere(transform.position, attackRange, whatIsPlayer))) //if not in attackRange
            ChasePlayer();
        else
            AttackPlayer();
    }

    private void Patroling()
    {
        CheckSides();

        if (!walkPointSet)
            Invoke("SearchWalkPoint", 3);
        //else agent.SetDestination(walkPoint);

        else if (Vector3.Distance(transform.position, walkPoint) < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        if (!walkPointSet)
        {
            walkPoint.x = transform.position.x + Random.Range(-walkPointRange, walkPointRange);
            walkPoint.z = transform.position.z + Random.Range(-walkPointRange, walkPointRange);

            if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            {
                walkPointSet = true;
                agent.SetDestination(walkPoint);
            }
        }
    }

    private void CheckSides()
    {
        playerPos = player.position - transform.position;
        angle = Mathf.Abs(Vector3.SignedAngle(playerPos, transform.forward, Vector3.up));
        SideAngle = Mathf.Abs(Vector3.SignedAngle(playerPos, transform.right, Vector3.up));

        if (angle > 315f || angle < 45f)
            movingViewAnimations.SetInteger("moving", 1); //forward

        else if (angle > 135f && angle < 225f)
            movingViewAnimations.SetInteger("moving", 3); //backward

        else if (SideAngle > 315f || SideAngle < 45f)
            movingViewAnimations.SetInteger("moving", 2); //left

        else if (SideAngle > 135f && SideAngle < 225f)
            movingViewAnimations.SetInteger("moving", 4); //right
    }

    private void ChasePlayer()
    {
        movingViewAnimations.SetInteger("moving", 1);
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        movingViewAnimations.SetInteger("moving", 5);

        transform.LookAt(player);
        Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity);
        if(hit.collider != null && hit.collider.gameObject.tag == "Player")
            agent.SetDestination(transform.position);
        else ChasePlayer();

        if(!alreadyAttacked)
        {
            projectile = enemyProjectiles.GetFreeElement();
            projectile.transform.position = bulletSpawn.position;
            projectile.transform.rotation = transform.rotation;
            GetComponent<AudioSource>().Play();

            alreadyAttacked = true;
            Invoke("ResetAttack", timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}
