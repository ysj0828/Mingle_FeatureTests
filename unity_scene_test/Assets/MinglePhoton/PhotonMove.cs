using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace com.unity.photon
{
    public class PhotonMove : MonoBehaviour
    {
        private Animator m_Animator;
        private NavMeshAgent m_NavMeshAgent;
        private bool m_Running = false;

        void Start()
        {
            m_Animator = GetComponent<Animator> ();
            m_NavMeshAgent = GetComponent<NavMeshAgent>();    
        }

        //void Update()
        //{
        //    // if(!photonView.IsMine) return;
        //    if (Input.GetMouseButtonDown(0)) {
        //            RaycastHit hit;
                    
        //            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)) {
        //                m_NavMeshAgent.destination = hit.point;
        //            }
        //        }

        //    if(m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance){
        //        m_Running = false;
        //    }else{
        //        m_Running = true;
        //    }
        //    m_Animator.SetBool ("isWalking", m_Running);
        //}
    }
}