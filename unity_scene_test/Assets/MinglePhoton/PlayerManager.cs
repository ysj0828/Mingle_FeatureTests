using Photon.Pun;
using UnityEngine;
using MingleCamera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

namespace com.unity.photon
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Private Fields

        private Animator m_Animator;
        private UnityEngine.AI.NavMeshAgent m_NavMeshAgent;
        private bool m_Running = false;

        public GameObject m_Pointer = null;
        public GameObject m_NickName = null;

        string currentAnimation;

        Coroutine emotionCoroutine;
        // Infomation _infomation;

        // AudioSource _effectAudioSource = new AudioSource();
        [SerializeField] AudioSource _effectAudioSource;
        GameObject _effectObject;

        #endregion

        #region Public Fields

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        #endregion

        [SerializeField] GameObject panel;
        [SerializeField] Slider loadingBar;

        List<AnimationClip> clips = new List<AnimationClip>();

        #region MonoBehaviour CallBacks

        private void OnDestroy()
        {
            if (photonView.IsMine)
            {
                Infomation.Instance.EmojiChanged -= PlayEmojiAnimation_Observer;
            }

        }

        void Awake()
        {
            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
                // GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<TapToMove>().avatarGO = transform.gameObject;
                Infomation.Instance.EmojiChanged += PlayEmojiAnimation_Observer;
            }
            DontDestroyOnLoad(this.gameObject);

            panel = GameObject.Find("Canvas").transform.Find("Load_Panel").gameObject;

        }

        [PunRPC]
        public void DestroyMyAvatar(int PVid)
        {
            Destroy(PhotonView.Find(PVid).gameObject);
        }


        void Start()
        {
            m_Pointer.GetComponent<Renderer>().enabled = photonView.IsMine;

            if (!photonView.IsMine) return;
            m_Animator = GetComponent<Animator>();
            // for (int i = 0; i < m_Animator.runtimeAnimatorController.animationClips.Length; i++)
            // {
            //   //Clip배열의 순서는 animator에 넣는 순서로 정해짐 . idle은 무조건 Clip[0](첫번째)여야함 
            //   clips.Add(m_Animator.runtimeAnimatorController.animationClips[i]);
            //   Debug.Log(m_Animator.runtimeAnimatorController.animationClips[i].ToString());
            // }
            // Debug.Log("INIT " + clips);

            m_NavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        void Update()
        {
            UpdateRender();
            if (photonView.IsMine) UpdateAnimation();
        }

        void UpdateRender()
        {
            // Debug.Log("PlayerManager.UpdateRender " +photonView.IsMine.ToString() + " " + photonView.Owner.NickName.ToString());
            m_NickName.transform.LookAt(Camera.main.transform.position);
            m_NickName.transform.Rotate(0, 180, 0);

            if (photonView.IsMine) m_NickName.GetComponent<TextMesh>().text = Infomation.Instance.NickName.ToString();
            else m_NickName.GetComponent<TextMesh>().text = photonView.Owner.NickName.ToString();
        }

        public void ClearAnimation()
        {
            // m_NavMeshAgent.ResetPath();
            Infomation.Instance.Animation = 0;
            // m_Animator.SetInteger("Emotion", 0);
            // m_Animator.SetBool("isIdle", true);
            // m_Animator.SetBool("isWalking", false);
            // m_Animator.SetBool("isRunning", false);
            // if (m_Animator.GetBool("isJumping"))
            // {

            // }
            // ClearMotion();
        }

        void ClearMotion()
        {
            m_Animator.SetBool("isWin", false);
            m_Animator.SetBool("isSad", false);
        }

        IEnumerator EmotionAnimation(string clipName)
        {
            // m_NavMeshAgent.ResetPath();
            m_Animator.SetBool("isWalking", false);
            m_Animator.SetBool("isRunning", false);
            m_Animator.SetBool("isIdle", false);

            // if (Infomation.Instance.Jumping)
            // {
            //     var stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
            //     var time = stateInfo.normalizedTime;
            //     while (true)
            //     {
            //         if (stateInfo.IsName("jump01_end"))
            //         {
            //             yield return null;
            //         }

            //         if (stateInfo.IsName("jump01_end") && stateInfo.normalizedTime % 1 < 0.95f)
            //         {
            //             yield return null;
            //         }

            //         else
            //         {
            //             m_Animator.Play(clipName);
            //             // currentAnimation = 0;
            //             yield break;
            //         }
            //     }
            // }

            // else
            // {

            // Debug.Log("Check Ani " + clipName);
            // foreach (var clip in clips)
            // {
            //   Debug.Log("check  Ani 2 " + clip.name + " " + clip.name == clipName);
            //   if (clip.name == clipName)
            //   {
            //     Debug.Log("Check Ani run " + clipName);
            //     m_Animator.Play(clipName);
            //     break;
            //   }
            // }
            m_Animator.Play(clipName);
            // m_Animator.SetInteger("Emotion", _infomation.E_Animation[clipName]);
            // yield return null;
            // m_Animator.SetInteger("Emotion", 0);
            // m_Animator.Play(clipName);

            while (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(clipName)) yield return null;
                else yield return new WaitForSeconds(0.1f);
            }

            // m_Animator.SetInteger("Emotion", 0);
            m_Animator.SetBool("isIdle", true);
            yield break;
            // }
        }

        void UpdateMovement()
        {
            if (!photonView.IsMine) return;

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
                {
                    if (!m_Animator.GetBool("isWalking"))
                    {
                        ClearAnimation();
                        m_Animator.SetBool("isWalking", true);
                    }
                    m_NavMeshAgent.SetDestination(hit.point);
                    // Debug.Log("update movement set destination");
                }
            }

            if (!m_NavMeshAgent.pathPending)
            {
                if (m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance)
                {
                    m_Animator.SetBool("isWalking", false);
                    m_NavMeshAgent.ResetPath();
                }
            }
        }
        float time2;
        void UpdateAnimation()
        {
            if (Infomation.Instance.Animation == 0) return;

            // if (m_Animator.GetInteger("Emotion") != 0) m_Animator.SetInteger("Emotion", 0);

            m_NavMeshAgent.ResetPath();
            //RPC 이동 한정 ResetPath
            photonView.RPC("ResetPath", RpcTarget.Others);
            m_Animator.SetBool("isWalking", false);
            m_Animator.SetBool("isRunning", false);
            m_Animator.SetBool("isIdle", true);
            // m_Animator.SetInteger("Emotion", _infomation.Animation);
            // _infomation.Animation=0;
        }

        [PunRPC]
        public void ChangeSceneRPCToMasterClient(string sceneName)
        {
            if (PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel(sceneName);
            else return;
        }

        #endregion


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
        }

        public void InstantiateEffect(int effectIndex)
        {
            // DestroyEffect();
            _effectObject = Instantiate(ParticleEffectManagerScript.instance.ParticleEffects[effectIndex], transform.position, transform.rotation);
            AudioClip clip = ParticleEffectManagerScript.instance.sfxClips[effectIndex];
            _effectAudioSource.PlayOneShot(clip);
        }

        public void DestroyEffect()
        {
            if (_effectObject != null) Destroy(_effectObject);
            if (_effectAudioSource.isPlaying) _effectAudioSource.Stop();
        }


        void PlayEmojiAnimation_Observer()
        {
            photonView.RPC("PlayEmojiAnimation_RPC", RpcTarget.All, photonView.ViewID, Infomation.Instance.Animation_String);
        }

        [PunRPC]
        void PlayEmojiAnimation_RPC(int PVid, string animation_string)
        {
            PhotonView.Find(PVid).gameObject.GetComponent<Animator>().Play(animation_string, -1, 0f);
        }
    }
}