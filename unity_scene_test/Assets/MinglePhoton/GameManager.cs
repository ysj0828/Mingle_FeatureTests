using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;
using MingleCamera;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;

namespace com.unity.photon
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields
        public static GameManager Instance = null;
        public static int PrefabIndex;

        [Tooltip("The prefab to use for representing the player")]
        public GameObject[] playerPrefab;
        Infomation _infomation;

        [SerializeField] cinemachine_test CT = null;
        [SerializeField] MasterController MC = null;
        [SerializeField] GameObject EffectManager;

        public static GameObject MyAvatar = null;

        #endregion

        #region MonoBehaviour CallBacks

        void SceneChangeEvent(Scene scene, LoadSceneMode mode)
        {
            switch (scene.name)
            {
                case "MingleMain":
                    SceneManager.sceneLoaded -= SceneChangeEvent;
                    Instance = null;
                    Destroy(this.gameObject);
                    return;
                    break;
                case "MingleCharacter":
                case "MingleMenu":
                case "MingleProfile":
                case "MingleRoom":
                    return;
                    break;
            }

            foreach (GameObject go in GameObject.FindGameObjectsWithTag("PlayerAvatar"))
            {
                var view = go.GetComponent<PhotonView>();
                if (view.IsMine) view.RPC("DestroyMyAvatar", RpcTarget.Others, view.ViewID);
                Destroy(MyAvatar);
            }

            MyAvatar = PhotonNetwork.Instantiate(_infomation.character_uuid, _infomation.InitialPosition[SceneManager.GetActiveScene().name], Quaternion.identity);
            CT.refTarget = MyAvatar.transform;
            CT.SetAvatarAsMain();
            // TTM.ava = temp;
            StartCoroutine(MC.FindMyAvatar());
        }

        void Start()
        {
            Camera.main.cullingMask = -1;

            _infomation = FindObjectOfType<Infomation>();
            CT = GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<cinemachine_test>();
            MC = GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<MasterController>();
            SceneManager.sceneLoaded += SceneChangeEvent;
            PhotonNetwork.AutomaticallySyncScene = true;
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }

            else
            {
                Destroy(this.gameObject);
            }
            // if (Instance != null)
            // {
            //     return;
            // }
            // Instance = this;
            // DontDestroyOnLoad(this);

            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            // else
            // {
            //     // Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
            //     RNMessenger.SendToRN(string.Format("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName));

            //     //PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            //     var temp = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(7f, 5f, -4f), Quaternion.identity, 0);


            //     GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<cinemachine_test>().AvatarNullCheck = temp;

            //     ////GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<TapToMove>().avatarGO = temp;



            //     // Debug.Log(GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<TapToMove>().avatarGO);
            // }

            else if (playerPrefab != null)
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    // Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    RNMessenger.SendToRN(string.Format("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName));
                    // var CT = GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<cinemachine_test>();
                    //PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                    // PrefabIndex = Random.Range(0, 4);
                    // MyAvatar = PhotonNetwork.Instantiate(FindObjectOfType<Infomation>().character_uuid, new Vector3(11, 0, -3), Quaternion.identity, 0);
                    MyAvatar = PhotonNetwork.Instantiate(_infomation.character_uuid, _infomation.InitialPosition[SceneManager.GetActiveScene().name], Quaternion.identity);

                    CT.AvatarNullCheck = MyAvatar;
                    MC.myAvatar = MyAvatar;


                    // TapToMove ttm = GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<TapToMove>();
                    // ttm.ava = temp;
                    // GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<TapToMove>().avatarGO = temp;

                    // Debug.Log(GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<TapToMove>().avatarGO);

                }

            }
        }

        private void OnDestroy()
        {
            // LeaveRoom();
        }

        #endregion


        #region MonoBehaviourPunCallbacks CallBacks

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            RNMessenger.SendToRN(string.Format("OnPlayerEnteredRoom() {0}", other.NickName));
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            RNMessenger.SendToRN(string.Format("OnPlayerLeftRoom() {0}", other.NickName));
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom != null) PhotonNetwork.LeaveRoom();
        }

        public void Message(string message)
        {
            var repalce = Regex.Replace(message, @"\s+", "").ToString();

            RNMessenger.SendToRN("GameManager Message : " + repalce);
            JObject json = JObject.Parse(repalce);
            if (json["cmd"].ToString() == "LeaveRoom")
            {
                LeaveRoom();
            }
            else if (json["cmd"].ToString() == "UpdateAnimation")
            {
                // FindObjectOfType<Infomation>().Animation = (int)json["Animation"];
                // _infomation.Animation = _infomation.E_Animation[json["Animation"].ToString()];
                _infomation.Animation_String = json["Animation"].ToString();
                // Debug.Log(json["Animation"].ToString());

                // _infomation.Animation = json["Animation"].ToString();
                // Debug.Log("ANI " + _infomation.Animation);
            }
        }

        #endregion
    }
}
