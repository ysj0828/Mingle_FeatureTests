using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MingleCamera;
using com.unity.photon;
using Photon.Pun;

public class ParticleEffectSMB : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // GameObject sfxManager = GameObject.FindGameObjectWithTag("AudioManager");
        PlayerManager playerManager = animator.gameObject.GetComponent<PlayerManager>();
        playerManager.DestroyEffect();

        int index = stateInfo.IsName("AngryShaded") ? 1 : 0;
        playerManager.InstantiateEffect(index);
        // animator.SetInteger("Emotion", 0);
        // Infomation.Instance.Animation = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime > 0.5)
        {
            // animator.SetInteger("Emotion", 0);
            Infomation.Instance.Animation = 0;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerManager playerManager = animator.gameObject.GetComponent<PlayerManager>();

        playerManager.DestroyEffect();

        // var photonView = GameObject.FindGameObjectWithTag("PlayerAvatar").GetComponent<PhotonView>();
        // photonView.RPC("PlayEmojiAnimation_RPC", RpcTarget.All, photonView.ViewID, Infomation.Instance.Animation_String);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
