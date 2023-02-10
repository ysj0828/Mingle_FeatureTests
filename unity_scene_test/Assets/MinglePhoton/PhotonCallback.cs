using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace com.unity.photon
{
    public class PhotonCallback : MonoBehaviourPunCallbacks
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        #region MonoBehaviourPunCallbbacks CallBacks

        public override void OnEnable() {
            RNMessenger.SendToRN("PUN OnEnable");
        }

        // public override void OnConnectedToMaster()
        // {
        //     RNMessenger.SendToRN("PUN OnConnectedToMaster");
        //     // PhotonNetwork.JoinLobby();
        // }

        // public override void OnDisconnected(DisconnectCause cause)
        // {
        //     RNMessenger.SendToRN(string.Format("PUN OnDisconnected: {0}.", cause));
        // }

        // public override void OnJoinedLobby()
        // {
        //     RNMessenger.SendToRN("PUN OnJoinedLobby");
        // }

        // public override void OnJoinedRoom(){
        //     RNMessenger.SendToRN("OnJoinedRoom ");
        //     PhotonNetwork.LoadLevel("Room");
        // }

        // public override void OnJoinRoomFailed(short returnCode, string message){
        //     RNMessenger.SendToRN("OnJoinRoomFailed "+returnCode.ToString()+" : "+message);
        // }

        #endregion
    }
}