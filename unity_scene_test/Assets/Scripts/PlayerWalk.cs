using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerWalk : MonoBehaviour
{
    NavMeshAgent playerAgent;
    Animator playerAnimator;

    private void Start()
    {
        playerAgent = transform.GetComponent<NavMeshAgent>();
        playerAnimator = transform.GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        if (playerAgent.remainingDistance > playerAgent.stoppingDistance)
        {
            playerAnimator.SetBool("isWalking", true);
        }

        else
        {
            playerAgent.ResetPath();
            playerAnimator.SetBool("isWalking", false);
        }
    }
}
