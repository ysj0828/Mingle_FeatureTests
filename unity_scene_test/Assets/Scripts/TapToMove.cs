using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using com.unity.photon;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace MingleCamera
{
    [DisallowMultipleComponent]
    public class TapToMove : MonoBehaviour, IPunObservable
    {
        GameObject wrench;

        internal GameObject ava
        {
            get
            {
                return avva;
            }
            set
            {
                avva = value;
                NMA = value.GetComponent<NavMeshAgent>();
                anim = value.GetComponent<Animator>();
            }
        }

        GameObject avva;

        PhotonTransformView transformView;

        public NavMeshAgent NMA;
        public Animator anim;

        public bool Jumping
        {
            get
            {
                return jumping;
            }

            set
            {
                jumping = value;
                MC.Jumping = value;
            }
        }
        bool jumping;

        [SerializeField] MasterController MC;

        //public GameObject avatarGO;
        // public GameObject avatarGO
        // {
        //     get
        //     {
        //         return avatargo;
        //     }

        //     set
        //     {
        //         avatargo = value;

        //         InitialiseTapToMove();
        //     }
        // }

        // IEnumerator DisableTransformView()
        // {
        //     Debug.Log("Disable transform view");
        //     PhotonTransformView PTview = transform.GetComponent<PhotonTransformView>();
        //     yield return null;
        //     if (PTview != null) PTview.enabled = false;
        //     yield break;
        // }

        private void OnEnable()
        {
            MC = GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<MasterController>();

            NMA = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            transformView = GetComponent<PhotonTransformView>();

            pView = GetComponent<PhotonView>();
            wrench = GameObject.Find("ui_wrench");

            // StartCoroutine(DisableTransformView());
        }

        private void Start()
        {
            transformView.enabled = false;
            if (pView.IsMine)
            {
                AnimatorIKPosition = Camera.main.transform.position;
            }
        }

        [PunRPC]
        void EnableWrench()
        {
            wrench.SetActive(true);
        }

        [PunRPC]
        void DisableWrench()
        {
            wrench.SetActive(false);
        }

        // GameObject avatargo;

        // GameObject raycastGO;
        // int raycastGOposIndex;
        int raycastGOlookIndex;

        public Vector3 hitPosition;
        Vector3 jumpPosition;

        bool limitUpdate;

        bool isMoving;

        string currentAnimation = null;

        PhotonView pView;

        public Vector3 AnimatorIKPosition;
        public Vector3 PhotonIKPosition;
        public bool EnableIKLook;
        public bool EnableIKLookOther;
        public bool LongPressEventBool;

        public void IsMineLongPressEvent(float touchTime, cinemachine_test CT)
        {
            if (touchTime <= 0.3f && touchTime < 0.6f)
            {
                return;
            }

            else if (touchTime >= 0.6f && !LongPressEventBool)
            {
                LongPressEventBool = true;
                CT.changeToFirstPerson();
            }
        }

        //! 상대방 정보창 띄우는 이벤트
        public void NotMineLongPressEvent(float touchTime)
        {
            if (touchTime <= 0.3f && touchTime < 0.6f)
            {
                return;
            }

            else if (touchTime >= 0.6f && !LongPressEventBool)
            {
                LongPressEventBool = true;

                Debug.Log("other avatar long press");

                // var json = new JObject();

                // json["cmd"] = "OnLongPressOtherAvatar?";

                // RNMessenger.SendToRN(JsonConvert.SerializeObject(json, Formatting.Indented));
            }
        }

        float lookAtWeightValue;

        IEnumerator IKLookAtWeightIncreaseLerp()
        {
            // EnableIKLook = false;

            while (true)
            {
                lookAtWeightValue = Mathf.Lerp(lookAtWeightValue, 1, 5.5f * Time.deltaTime);

                if (lookAtWeightValue >= 0.95f)
                {
                    lookAtWeightValue = 1;
                    yield return new WaitForSeconds(0.5f);
                    currentCoroutine = StartCoroutine(IKLookAtWeightDecreaseLerp());
                    yield break;
                }

                if (pView.IsMine)
                {
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    yield return null;
                }
            }
        }

        IEnumerator IKLookAtWeightDecreaseLerp()
        {
            while (true)
            {
                lookAtWeightValue = Mathf.Lerp(lookAtWeightValue, 0, 4.5f * Time.deltaTime);

                // Vector3 dir = (Camera.main.transform.position - transform.position).normalized;
                Vector3 dir = Vector3.zero;
                // if (EnableIKLook)
                // {
                if (pView.IsMine)
                {
                    dir = (new Vector3(PhotonIKPosition.x, transform.position.y, PhotonIKPosition.z) - transform.position).normalized;
                }
                else if (!pView.IsMine)
                {
                    dir = (new Vector3(AnimatorIKPosition.x, transform.position.y, AnimatorIKPosition.z) - transform.position).normalized;
                }
                // }

                // else if (EnableIKLookOther)
                // {
                //     if (pView.IsMine)
                //     {
                //         dir = (new Vector3(PhotonIKPosition.x, transform.position.y, PhotonIKPosition.z) - transform.position).normalized;
                //     }
                //     else if (!pView.IsMine)
                //     {
                //         dir = (new Vector3(AnimatorIKPosition.x, transform.position.y, AnimatorIKPosition.z) - transform.position).normalized;
                //     }
                // }

                Quaternion quatDir = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, quatDir, 4.5f * Time.deltaTime);

                if (lookAtWeightValue <= 0.05f)
                {
                    lookAtWeightValue = 0;
                    transform.rotation = quatDir;
                    currentCoroutine = null;
                    EnableIKLook = false;
                    yield break;
                }
                if (pView.IsMine)
                {
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    yield return null;
                }
            }
        }

        public void StopRunningCoroutines()
        {
            lookAtWeightValue = 0;
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
        }

        [PunRPC]
        public IEnumerator StartRPCCoroutine()
        {
            currentCoroutine = StartCoroutine(IKLookAtWeightIncreaseLerp());
            yield return null;
        }

        Coroutine currentCoroutine = null;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(PhotonIKPosition);
            }

            else if (stream.IsReading)
            {
                AnimatorIKPosition = (Vector3)stream.ReceiveNext();
            }
            // if (EnableIKLook)
            // {
            //     if (stream.IsWriting)
            //     {
            //         stream.SendNext(Camera.main.transform.position);
            //     }

            //     else if (stream.IsReading)
            //     {
            //         AnimatorIKPosition = (Vector3)stream.ReceiveNext();
            //     }
            // }

            // else if (EnableIKLookOther)
            // {
            //     if (stream.IsWriting)
            //     {
            //         stream.SendNext(PhotonIKPosition);
            //     }

            //     else if (stream.IsReading)
            //     {
            //         AnimatorIKPosition = (Vector3)stream.ReceiveNext();
            //     }
            // }
        }

        public void OnAnimatorIK()
        {
            if (pView.IsMine)
            {
                anim.SetLookAtPosition(PhotonIKPosition);
            }

            else
            {
                anim.SetLookAtPosition(AnimatorIKPosition);
            }

            anim.SetLookAtWeight(lookAtWeightValue);
            if (EnableIKLook)
            {
                // currentCoroutine = StartCoroutine(IKLookAtWeightIncreaseLerp());
                if (currentCoroutine == null)
                {
                    pView.RPC("StartRPCCoroutine", RpcTarget.All);
                    // Debug.Log("rpc call counter");
                }
            }

            // else if (EnableIKLookOther)
            // {
            // if (currentCoroutine == null)
            // {
            //     pView.RPC("StartRPCCoroutine", RpcTarget.All);
            // }
            // }
        }

        // void turnAvatar(RaycastHit hit)
        // {
        //     //// Vector3 dir = (hit.point - avatarGO.transform.position).normalized;
        //     Vector3 dir = (hit.point - ava.transform.position).normalized;
        //     Quaternion Qdir = Quaternion.LookRotation(dir);
        //     ava.transform.rotation = Qdir;
        //     ////avatarGO.transform.rotation = Qdir;
        // }

        [PunRPC]
        void turnAvatar(Vector3 hitPoint)
        {
            //// Vector3 dir = (hit.point - avatarGO.transform.position).normalized;
            Vector3 dir = (hitPoint - transform.position).normalized;
            Quaternion Qdir = Quaternion.LookRotation(dir);
            transform.rotation = Qdir;
            ////avatarGO.transform.rotation = Qdir;
        }

        // public void DoubleTap(RaycastHit hit)
        // {
        //     if (!Jumping)
        //     {
        //         Jumping = true;

        //         jumpPosition = hit.point;

        //         anim.SetBool("reachedDestination", false);
        //         anim.SetBool("closeToDestination", false);
        //         anim.SetBool(currentAnimation, false);

        //         anim.SetBool("isJumping", true);
        //         currentAnimation = "isJumping";

        //         NMA.enabled = false;

        //         ParabolaStart.transform.position = hit.point;

        //         float x = (ava.transform.position.x + ParabolaStart.transform.position.x) / 2;
        //         float y = (ava.transform.position.y + ParabolaStart.transform.position.y) / 2;
        //         float z = (ava.transform.position.z + ParabolaStart.transform.position.z) / 2;

        //         Vector3 midPoint = new Vector3(x, y, z);

        //         ParabolaMid.transform.position = midPoint + new Vector3(0, midPoint.magnitude * 0.25f, 0);

        //         ParabolaStart.transform.position = new Vector3(ParabolaStart.transform.position.x, Mathf.Clamp(ParabolaStart.transform.position.y, 0, 100), ParabolaStart.transform.position.z);
        //         ParabolaEnd.transform.position = new Vector3(ParabolaEnd.transform.position.x, Mathf.Clamp(ParabolaEnd.transform.position.y, 0, 100), ParabolaEnd.transform.position.z);

        //         PC.FollowParabola();
        //         limitUpdate = true;

        //     }
        // }


        [PunRPC]
        void SurpriseAvatar()
        {
            anim.Play("SurpriseAvatar");
        }

        [PunRPC]
        void EnableNMA()
        {
            // PhotonView.Find(PVid).gameObject.GetComponent<NavMeshAgent>().enabled = true;
            NMA.enabled = true;
            transformView.m_SynchronizePosition = true;
        }
        [PunRPC]
        void DisableNMA()
        {
            // PhotonView.Find(PVid).gameObject.GetComponent<NavMeshAgent>().enabled = false;
            NMA.enabled = false;
            transformView.m_SynchronizePosition = false;
        }

        [PunRPC]
        public void DoubleTap(Vector3 hitPoint)
        {
            if (!Jumping)
            {
                Jumping = true;

                jumpPosition = hitPoint;

                anim.SetBool("reachedDestination", false);
                anim.SetBool("closeToDestination", false);
                anim.SetBool(currentAnimation, false);

                anim.SetBool("isJumping", true);
                currentAnimation = "isJumping";

                pView.RPC("DisableNMA", RpcTarget.Others);
                NMA.enabled = false;

                pView.RPC("StartParabolic", RpcTarget.All, hitPoint);
                limitUpdate = true;
            }
        }

        [PunRPC]
        public void DoubleTapSelf()
        {
            anim.SetBool("reachedDestination", false);
            anim.SetBool("closeToDestination", false);
            anim.SetBool(currentAnimation, false);

            anim.SetBool("isJumping", true);
            currentAnimation = "";

            pView.RPC("DisableNMA", RpcTarget.Others);
            NMA.enabled = false;

            SetInitialPosition = false;
            StartCoroutine(SimpleParabolicSelf());
        }

        Vector3 ParabolicInitialPosition;
        float ParabolicInterpolationT;

        bool SetInitialPosition;
        float ParabolicHeight;


        [PunRPC]
        void StartParabolic(Vector3 hitPoint)
        {
            SetInitialPosition = false;
            StartCoroutine(SimpleParabolic(hitPoint));
        }

        float tempFloat;

        IEnumerator SimpleParabolicSelf()
        {
            bool reverse = false;
            while (true)
            {
                if (!SetInitialPosition)
                {
                    reverse = false;
                    tempFloat = 0;
                    ParabolicHeight = 4f;
                    anim.Play("jump01_loop");
                    SetInitialPosition = true;

                    if (anim.GetBool("closeToDestination")) anim.SetBool("closeToDestination", false);
                    if (anim.GetBool("reachedDestination")) anim.SetBool("reachedDestination", false);
                    if (!anim.GetBool("isJumping")) anim.SetBool("isJumping", true);
                    // if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "jump01_loop") anim.Play("jump01_loop");
                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName("jump01_loop")) anim.Play("jump01_loop");
                }

                if (!reverse)
                {
                    tempFloat += 0.05f;
                    if (tempFloat >= 2f)
                    {
                        reverse = true;
                    }
                }

                else if (reverse)
                {
                    tempFloat -= 0.05f;
                }

                //fps 60
                // tempFloat += 0.025f;

                //fps 30
                // tempFloat += 0.05f;

                transform.position = new Vector3(transform.position.x, tempFloat, transform.position.z);

                if (tempFloat < 0.05f && reverse)
                {
                    anim.SetBool("closeToDestination", true);

                    pView.RPC("EnableNMA", RpcTarget.Others);
                    NMA.enabled = true;
                    // Debug.Log("enalbe nma in ArriveAtDestination");
                    NMA.ResetPath();
                    //anim.SetBool("isWalking", false);
                    anim.SetBool("isJumping", false);
                    anim.SetBool("reachedDestination", true);
                    anim.SetBool("isIdle", true);
                    currentAnimation = "isIdle";
                    isMoving = false;
                    limitUpdate = false;

                    //anim.SetBool("closeToDestination", false);
                    Jumping = false;

                    yield break;
                }

                if (pView.IsMine)
                {
                    yield return new WaitForEndOfFrame();
                }

                else if (!pView.IsMine)
                {
                    // if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "jump01_loop") anim.Play("jump01_loop");
                    yield return null;
                }
            }
        }
        IEnumerator SimpleParabolic(Vector3 hitPoint)
        {
            while (true)
            {
                if (!SetInitialPosition)
                {
                    ParabolicInitialPosition = transform.position;
                    tempFloat = 0;
                    ParabolicHeight = Mathf.Clamp(Vector3.Distance(ParabolicInitialPosition, hitPoint) * 0.25f, 3, 10);
                    anim.Play("jump01_loop");
                    SetInitialPosition = true;
                }

                //fps 60
                tempFloat += 0.025f;

                //fps 30
                // tempFloat += 0.05f;

                transform.position = Parabola(ParabolicInitialPosition, hitPoint, ParabolicHeight, tempFloat);

                Vector2 transformv2 = new Vector2(transform.position.x, transform.position.z);
                Vector2 hitPointv2 = new Vector2(hitPoint.x, hitPoint.z);

                if ((transformv2 - hitPointv2).magnitude <= 0.01f)
                {
                    yield break;
                }

                if (pView.IsMine)
                {
                    yield return new WaitForEndOfFrame();
                }

                else if (!pView.IsMine)
                {
                    // if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "jump01_loop") anim.Play("jump01_loop");
                    yield return null;
                }
            }
        }

        Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }

        [PunRPC]
        public void IdleAnimation()
        {
            if (!anim.GetBool("isWalking") && !anim.GetBool("isRunning") && !anim.GetBool("isJumping"))
            {
                anim.Play("idle01");
            }
        }

        [PunRPC]
        public void SetDest(Vector3 destination, string clip)
        {
            anim.Play(clip);
            NMA.SetDestination(destination);
        }

        [PunRPC]
        public void ResetPath()
        {
            NMA.ResetPath();
            anim.SetBool("isIdle", true);
        }

        [PunRPC]
        public void Tap(Vector3 hitPoint, string hitTag)
        {
            if (!Jumping)
            {
                switch (hitTag)
                {
                    case "Walkable":
                        GetComponent<PlayerManager>().ClearAnimation();
                        GetComponent<PlayerManager>().DestroyEffect();
                        hitPosition = hitPoint;
                        if (currentAnimation != null)
                        {
                            anim.SetBool(currentAnimation, false);
                        }

                        pView.RPC("turnAvatar", RpcTarget.All, hitPoint);

                        if ((transform.position - hitPosition).magnitude > 4)
                        {
                            anim.Play("run01");
                            anim.SetBool("isRunning", true);
                            anim.SetBool("isIdle", false);
                            currentAnimation = "isRunning";
                            NMA.speed = 3.5f;
                            pView.RPC("SetDest", RpcTarget.All, hitPoint, "run01");
                        }

                        else if ((transform.position - hitPosition).magnitude < 4)
                        {
                            // anim.Play("walk");
                            anim.SetBool("isWalking", true);
                            anim.SetBool("isIdle", false);
                            currentAnimation = "isWalking";
                            NMA.speed = 2.5f;
                            pView.RPC("SetDest", RpcTarget.All, hitPoint, "walk");
                        }

                        // pView.RPC("SetDest", RpcTarget.All, hitPoint);

                        //anim.SetBool("isWalking", true);
                        limitUpdate = true;
                        isMoving = true;
                        break;
                }
            }
        }

        // public void Tap(RaycastHit hit)
        // {
        //     if (!Jumping)
        //     {
        //         if (NMA.enabled == false)
        //         {
        //             NMA.enabled = true;
        //         }

        //         switch (hit.transform.tag)
        //         {
        //             case "DJ":
        //                 if (currentAnimation != null)
        //                 {
        //                     anim.SetBool(currentAnimation, false);
        //                 }
        //                 currentAnimation = "isDJing";
        //                 raycastGO = hit.transform.gameObject;
        //                 raycastGOposIndex = raycastGO.transform.childCount - 1;
        //                 raycastGOlookIndex = raycastGO.transform.childCount - 2;

        //                 //FIXME
        //                 NMA.SetDestination(raycastGO.transform.GetChild(raycastGOposIndex).transform.position);

        //                 anim.SetBool("isWalking", true);
        //                 limitUpdate = true;
        //                 break;

        //             case "Walkable":
        //                 hitPosition = hit.point;
        //                 if (currentAnimation != null)
        //                 {
        //                     anim.SetBool(currentAnimation, false);
        //                 }
        //                 //currentAnimation = "isWalking";
        //                 //NMA.destination = hit.point;

        //                 turnAvatar(hit);


        //                 //// if ((avatarGO.transform.position - hitPosition).magnitude > 5)
        //                 if ((ava.transform.position - hitPosition).magnitude > 5)
        //                 {
        //                     anim.SetBool("isRunning", true);
        //                     anim.SetBool("isIdle", false);
        //                     currentAnimation = "isRunning";
        //                     NMA.speed = 3.5f;
        //                 }

        //                 //// else if ((avatarGO.transform.position - hitPosition).magnitude < 5)
        //                 else if ((ava.transform.position - hitPosition).magnitude < 5)
        //                 {
        //                     anim.SetBool("isWalking", true);
        //                     anim.SetBool("isIdle", false);
        //                     currentAnimation = "isWalking";
        //                     NMA.speed = 2.5f;
        //                 }

        //                 NMA.SetDestination(hit.point);

        //                 //anim.SetBool("isWalking", true);
        //                 limitUpdate = true;
        //                 isMoving = true;
        //                 break;

        //         }
        //     }
        // }

        public void ArriveAtDestination()
        {
            if (limitUpdate == true)
            {
                //// if (avatarGO.transform.position.y < 0)
                //// {
                ////     avatarGO.transform.position = new Vector3(avatarGO.transform.position.x, 0, avatarGO.transform.position.z);
                //// }
                // if (NMA == null)
                // {
                //     transform.GetComponent<NavMeshAgent>();
                // }

                if (transform.position.y < 0)
                {
                    transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                }

                // if (ava.transform.position.y < 0)
                // {
                //     ava.transform.position = new Vector3(ava.transform.position.x, 0, ava.transform.position.z);
                // }

                // if (!_infomation.Jumping)
                // {
                //   NMA.ResetPath();
                //   limitUpdate = false;
                //   return;
                // }

                if (currentAnimation == "isWalking" || currentAnimation == "isRunning")
                {
                    // if (NMA.remainingDistance <= NMA.stoppingDistance + 0.001f && Mathf.Abs((transform.position - hitPosition).magnitude) > 0.1f)
                    // // if (NMA.remainingDistance <= NMA.stoppingDistance + 0.001f && Mathf.Abs((ava.transform.position - hitPosition).magnitude) > 0.1f)
                    // {
                    //   NMA.destination = hitPosition;
                    // }

                    if (NMA.remainingDistance <= NMA.stoppingDistance + 0.001f && Mathf.Abs((transform.position - hitPosition).magnitude) <= 0.1f)
                    // else if (NMA.remainingDistance <= NMA.stoppingDistance + 0.001f && Mathf.Abs((ava.transform.position - hitPosition).magnitude) <= 0.1f)
                    {
                        NMA.ResetPath();
                        //anim.SetBool("isWalking", false);
                        anim.SetBool(currentAnimation, false);
                        anim.SetBool("isIdle", true);
                        currentAnimation = "isIdle";
                        isMoving = false;
                        limitUpdate = false;
                    }
                }

                else if (currentAnimation == "isJumping")
                {
                    Vector2 avatarPositionXY = new Vector2(transform.position.x, transform.position.z);

                    Vector2 jumpXY = new Vector2(jumpPosition.x, jumpPosition.z);

                    if (Mathf.Abs((avatarPositionXY - jumpXY).magnitude) > 0.15f)
                    {
                        if (anim.GetBool("closeToDestination")) anim.SetBool("closeToDestination", false);
                        if (anim.GetBool("reachedDestination")) anim.SetBool("reachedDestination", false);
                        if (!anim.GetBool("isJumping")) anim.SetBool("isJumping", true);
                        // if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "jump01_loop") anim.Play("jump01_loop");
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("jump01_loop")) anim.Play("jump01_loop");
                        // else if ()
                    }

                    if (Mathf.Abs((avatarPositionXY - jumpXY).magnitude) <= 0.15f)
                    {
                        anim.SetBool("closeToDestination", true);

                        pView.RPC("EnableNMA", RpcTarget.Others);
                        NMA.enabled = true;
                        // Debug.Log("enalbe nma in ArriveAtDestination");
                        NMA.ResetPath();
                        //anim.SetBool("isWalking", false);
                        anim.SetBool("isJumping", false);
                        anim.SetBool("reachedDestination", true);
                        anim.SetBool("isIdle", true);
                        currentAnimation = "isIdle";
                        isMoving = false;
                        limitUpdate = false;

                        //anim.SetBool("closeToDestination", false);
                        Jumping = false;
                    }
                }
            }
        }

        Quaternion rot;
        bool rotationEnded = true;
        float t;

        public void RotateAvatar()
        {
            if (!rotationEnded)
            {
                if (t < 0.8f)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, rot, t);
                    // ava.transform.rotation = Quaternion.Lerp(ava.transform.rotation, rot, t);
                    t += Time.deltaTime;
                }

                else
                {
                    rotationEnded = true;
                    //Animate();
                }
            }
        }

    }
}