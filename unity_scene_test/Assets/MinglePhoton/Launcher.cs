using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Newtonsoft.Json.Linq;
using System.Collections;
using UnityEngine.UI;

namespace com.unity.photon
{
  public class Launcher : MonoBehaviourPunCallbacks
  {
    [SerializeField] GameObject panel;
    [SerializeField] Slider laodSlider;

    #region Private Serializable Fields

    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 16;

    private bool _is_connect = false;

    Infomation _infomation;

    #endregion


    #region Private Fields

    string gameVersion = "1";

    #endregion

    #region MonoBehaviour CallBacks

    void Awake()
    {

      RNMessenger.SendToRN("Awake");
      // PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
      RNMessenger.SendToRN("Start");
      PhotonNetwork.GameVersion = gameVersion;
      PhotonNetwork.ConnectUsingSettings();
      _infomation = FindObjectOfType<Infomation>();
      PhotonNetwork.NickName = _infomation.NickName;

      // RNMessenger.SendToRN("Start " + PhotonNetwork.IsConnected + " offlinemode " + PhotonNetwork.OfflineMode);
      // if(!PhotonNetwork.IsConnected){
      //     RNMessenger.SendToRN("Start1");
      //     PhotonNetwork.GameVersion = gameVersion;
      //     PhotonNetwork.ConnectUsingSettings();
      // }else{
      //     RNMessenger.SendToRN("Start2 PUN OnConnectedToMaster " +  PhotonNetwork.NetworkClientState);
      //     PhotonNetwork.Disconnect();
      //     // PhotonNetwork.NetworkingClient.LoadBalancingPeer.Service();
      //     // PhotonNetwork.NetworkingClient.LoadBalancingPeer.SendOutgoingCommands();
      // }
    }

    #endregion


    #region Public Methods

    public override void OnConnectedToMaster()
    {
      RNMessenger.SendToRN("PUN OnConnectedToMaster " + PhotonNetwork.NetworkClientState);
      if (_is_connect) JoinOrCreateRoom(_infomation.RoomID);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
      RNMessenger.SendToRN("OnDisconnected");
      PhotonNetwork.ConnectUsingSettings();
      PhotonNetwork.Reconnect();
    }

    public void JoinOrCreateRoom(string roomName)
    {
      RNMessenger.SendToRN("JoinOrCreateRoom to : " + roomName);
      _is_connect = true;
      bool ret = PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayersPerRoom }, Photon.Realtime.TypedLobby.Default);
      RNMessenger.SendToRN("JoinOrCreateRoom ret : " + ret.ToString());
      //     PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnCreatedRoom()
    {
      RNMessenger.SendToRN("OnCreatedRoom");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
      RNMessenger.SendToRN("OnCreateRoomFailed : " + returnCode + " : " + message);
    }

    public override void OnJoinedRoom()
    {
      _is_connect = false;
      RNMessenger.SendToRN("OnJoinedRoom");
      StartCoroutine(PhotonLoad(_infomation.room_template_uuid));
    }

    IEnumerator PhotonLoad(string sceneName)
    {
      panel.SetActive(true);
      PhotonNetwork.LoadLevel(sceneName);

      while (PhotonNetwork.LevelLoadingProgress < 0.9f)
      {
        laodSlider.value = PhotonNetwork.LevelLoadingProgress;
        yield return null;
      }

      // PhotonNetwork.AutomaticallySyncScene = true;
      // panel.SetActive(false);
      // yield break;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
      RNMessenger.SendToRN("OnJoinRoomFailed " + returnCode.ToString() + " : " + message);
    }

    public void Connect()
    {
      // RNMessenger.SendToRN("Connect : "+PhotonNetwork.IsConnected.ToString());
      RNMessenger.SendToRN("ClientState : " + PhotonNetwork.NetworkClientState.ToString());
      if (PhotonNetwork.IsConnected)
      {
        JoinOrCreateRoom(_infomation.RoomID);
      }
      // else
      // {
      //     PhotonNetwork.GameVersion = gameVersion;
      //     PhotonNetwork.ConnectUsingSettings();
      // }

      // JoinOrCreateRoom("d6326f4d2ee440b4939103f1708afe6c");
    }

    public void Message(string message)
    {
      // RNMessenger.SendToRN("Launcer Message : " + message);
      JObject json = JObject.Parse(message);

      // RNMessenger.SendToRN("Launcer Message : " + json["cmd"].ToString());

      // Type thisType = this.GetType();
      // MethodInfo theMethod = thisType.GetMethod("ConnectToRoom");

      // if(theMethod == null)
      // {
      //     RNMessenger.SendToRN("cmd Error : " + json["cmd"].ToString());
      //     return;
      // }

      StartCoroutine(json["cmd"].ToString(), json);

      // if(json["cmd"].ToString() == "ConnectToRoom"){
      //     _infomation.RoomID = json["RoomID"].ToString();
      //     _infomation.NickName = json["NickName"].ToString();
      //     PhotonNetwork.NickName = _infomation.NickName;
      //     // RNMessenger.SendToRN("ClientState : "+PhotonNetwork.NetworkClientState.ToString());
      //     JoinOrCreateRoom(_infomation.RoomID);
      // }
    }

    IEnumerator ConnectToRoom(JObject json)
    {
      _infomation.RoomID = json["RoomID"].ToString();
      _infomation.NickName = json["NickName"].ToString();
      _infomation.character_uuid = json["character_uuid"].ToString();
      _infomation.room_template_uuid = json["room_template_uuid"].ToString();

      PhotonNetwork.NickName = _infomation.NickName;
      JoinOrCreateRoom(_infomation.RoomID);

      yield return null;
    }
    IEnumerator UpdateRoomsInfo(JObject json)
    {
      _infomation.RoomData = json["RoomData"].ToObject<JArray>();

      yield return null;
    }


    public void SetRoomInfo()
    {

      if (PhotonNetwork.IsConnected)
      {

        // "room":{
        // "id":"4bc6f4a832054a528a7379ffcaecb427",
        // "thumbnail":"https://mingle-contents.s3.ap-northeast-2.amazonaws.com/default-room-template-thumbnails/room3.png",
        // "title":"룰루랄라,Mskim"
        // }

        JObject room = new JObject();
        room["id"] = "4bc6f4a832054a528a7379ffcaecb427";
        room["thumbnail"] = "https://mingle-contents.s3.ap-northeast-2.amazonaws.com/default-room-template-thumbnails/room3.png";
        room["title"] = "룰루랄라,Mskim";

        // {
        // "nickname":"Sktest",
        // "phone_number":"+821055516201",
        // "thumbnail":null,
        // "type":"OWNER",
        // "user_id":"a30526cd9f034c01be18c48fe4e62a4d"
        // },
        // {
        // "nickname":"Sktest",
        // "phone_number":"+8201055516201",
        // "thumbnail":null,
        // "type":"PARTICIPANT",
        // "user_id":"d496bfe458454d48aab3db68a5249d93"
        // }

        JObject member1 = new JObject();
        member1["nickname"] = "Sktest";
        member1["phone_number"] = "+821055516201";
        member1["thumbnail"] = "null";
        member1["type"] = "OWNER";
        member1["user_id"] = "a30526cd9f034c01be18c48fe4e62a4d";

        JObject member2 = new JObject();
        member2["nickname"] = "Sktest2";
        member2["phone_number"] = "+821055516202";
        member2["thumbnail"] = "null";
        member2["type"] = "PARTICIPANT";
        member2["user_id"] = "d496bfe458454d48aab3db68a5249d93";

        JArray members = new JArray();
        members.Add(member1);
        members.Add(member2);

        JObject roomInfo = new JObject();
        roomInfo["room"] = room;
        roomInfo["members"] = members;

        JArray RoomData = new JArray();
        RoomData.Add(roomInfo);
        RoomData.Add(roomInfo);
        RoomData.Add(roomInfo);
        RoomData.Add(roomInfo);

        // cmd: 'UpdateRoomsInfo',
        // RoomData: RoomData,

        JObject json = new JObject();
        json["cmd"] = "UpdateRoomsInfo";
        json["RoomData"] = RoomData;

        StartCoroutine(json["cmd"].ToString(), json);
        // UpdateRoomsInfo(json);

      }

    }

    #endregion

  }
}
