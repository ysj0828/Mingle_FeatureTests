using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCWalkScript : MonoBehaviour
{
    NavMeshAgent thisAgent;
    Animator animator;

    [HideInInspector]

    public string currentAnimation;

    string[] animatorParam = new string[] { "isIdle1", "isIdle2", "isIdle3", "isInteraction", "isAngry", "isBlushing", "isCool", "isCrying", "isCussing", "isHeart", "isLaughing", "isSilly", "isSleeping", "isUWU" };

    bool startAnimation;

    // Start is called before the first frame update
    void Start()
    {
        thisAgent = this.transform.GetComponent<NavMeshAgent>();
        animator = this.transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (thisAgent.remainingDistance > 0.1)
        {
            if (!animator.GetBool("isWalking"))
            {
                animator.SetBool("isWalking", true);
            }

            if (startAnimation)
            {
                startAnimation = false;
            }
        }

        if (!startAnimation)
        {
            animator.SetBool(currentAnimation, false);
        }

        if (startAnimation && currentAnimation != string.Empty)
        {
            animator.SetBool(currentAnimation, true);
        }

        else if (thisAgent.remainingDistance <= 0.1)
        {
            thisAgent.ResetPath();
            animator.SetBool("isWalking", false);
            //DisableAllAnimation();
            startAnimation = true;
            //animator.SetBool(currentAnimation, true);
        }
    }

}
