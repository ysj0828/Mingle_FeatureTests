using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
using com.unity.photon;
using Exoa.Cameras;

namespace MingleMain
{
  public class MainManager : MonoBehaviour
  {
    RoomManager _room_manager;
    FriendManager _friend_manager;
    RequestManager _requst_manager;
    Infomation _infomation;
    void Start()
    {
      Application.targetFrameRate = 60;
      _room_manager = FindObjectOfType<RoomManager>();
      _friend_manager = FindObjectOfType<FriendManager>();
      _requst_manager = FindObjectOfType<RequestManager>();
      _infomation = FindObjectOfType<Infomation>();

      _room_manager.gameObject.SetActive(false);
      _friend_manager.gameObject.SetActive(false);
      _requst_manager.gameObject.SetActive(false);

      if (!_infomation.isFirst)
      {
        UpdateVeiew();
      }
    }

    void UpdateVeiew()
    {
      _room_manager.updateRoomData();
      _friend_manager.updateFriendData();

      _room_manager.gameObject.SetActive(false);
      _friend_manager.gameObject.SetActive(false);
      _requst_manager.gameObject.SetActive(false);

      if (_infomation.RoomData != null && _infomation.RoomData.Count > 0) _room_manager.gameObject.SetActive(true);
      else if (_infomation.FriendsData != null && _infomation.FriendsData.Count > 0) _friend_manager.gameObject.SetActive(true);
      else _requst_manager.gameObject.SetActive(true);//
    }

    public void SetMainData(string message)
    {
      _infomation.isFirst = false;
      RNMessenger.SendToRN("SetMainData");
      RNMessenger.SendToRN(message);

      JObject json = JObject.Parse(message);
      _infomation.RoomData = json["RoomData"].ToObject<JArray>();
      _infomation.FriendsData = json["FriendsData"].ToObject<JArray>();
      UpdateVeiew();
    }

    public void UpdateRoomsInfo(string message)
    {
      Debug.Log("UpdateRoomsInfo");
      JObject json = JObject.Parse(message);
      _infomation.RoomData = json["RoomData"].ToObject<JArray>();

      UpdateVeiew();
    }

    public void UpdateUnreadMsgCnt(string message)
    {
      JObject json = JObject.Parse(message);
      JObject roomMessageCnt = json["unreadMessageNum"].ToObject<JObject>();
      _room_manager.UpdateUnreadMsgCnt(roomMessageCnt);
    }

    ////////////////// Dummy Methods //////////////////
    public void SetRoomInfo()
    {
      JObject room = new JObject();
      room["id"] = "4bc6f4a832054a528a7379ffcaecb427";
      room["thumbnail"] = "https://mingle-contents.s3.ap-northeast-2.amazonaws.com/default-room-template-thumbnails/room3.png";
      room["title"] = "룰루랄라,Mskim";

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
      members.Add(member1);
      members.Add(member2);
      members.Add(member1);
      members.Add(member2);

      JObject roomInfo = new JObject();
      roomInfo["room"] = room;
      roomInfo["members"] = members;

      JArray RoomData = new JArray();
      RoomData.Add(roomInfo);
      room["id"] = "4bc6f4a832054a528a7379ffcaecb4271";
      RoomData.Add(roomInfo);

      room["id"] = "4bc6f4a832054a528a7379ffcaecb4272";
      RoomData.Add(roomInfo);
      room["id"] = "4bc6f4a832054a528a7379ffcaecb4273";
      RoomData.Add(roomInfo);
      room["id"] = "4bc6f4a832054a528a7379ffcaecb4274";
      RoomData.Add(roomInfo);
      RoomData.Add(roomInfo);
      RoomData.Add(roomInfo);
      RoomData.Add(roomInfo);
      RoomData.Add(roomInfo);
      RoomData.Add(roomInfo);

      _infomation.isFirst = false;
      _infomation.RoomData = RoomData;
      _room_manager.updateRoomData();
      _room_manager.gameObject.SetActive(true);
    }

    public void SetRoomInfo2()
    {
      JObject room = new JObject();
      room["id"] = "4bc6f4a832054a528a7379ffcaecb427";
      room["thumbnail"] = "https://mingle-contents.s3.ap-northeast-2.amazonaws.com/default-room-template-thumbnails/room3.png";
      room["title"] = "룰루랄라,Mskim";

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
      members.Add(member1);
      members.Add(member2);
      members.Add(member1);
      members.Add(member2);

      JObject roomInfo = new JObject();
      roomInfo["room"] = room;
      roomInfo["members"] = members;

      JArray RoomData = new JArray();
      RoomData.Add(roomInfo);
      room["id"] = "4bc6f4a832054a528a7379ffcaecb4271";
      RoomData.Add(roomInfo);

      room["id"] = "4bc6f4a832054a528a7379ffcaecb4272";
      RoomData.Add(roomInfo);
      room["id"] = "4bc6f4a832054a528a7379ffcaecb4275";
      RoomData.Add(roomInfo);
      room["id"] = "4bc6f4a832054a528a7379ffcaecb4276";
      RoomData.Add(roomInfo);
      RoomData.Add(roomInfo);
      RoomData.Add(roomInfo);
      RoomData.Add(roomInfo);
      RoomData.Add(roomInfo);
      RoomData.Add(roomInfo);

      _infomation.isFirst = false;
      _infomation.RoomData = RoomData;
      _room_manager.updateRoomData();
      _room_manager.gameObject.SetActive(true);
    }

    public void SetFriendInfo()
    {
      JArray array = new JArray();
      JObject friend = new JObject();

      friend["user_id"] = "01e69a0294c84cb7ad5b4a54e91e4e49";
      friend["nickname"] = "sytest1";
      array.Add(friend);
      friend["user_id"] = "01e69a0294c84cb7ad5b4a54e91e4e492";
      friend["nickname"] = "sytest2";
      array.Add(friend);
      friend["user_id"] = "01e69a0294c84cb7ad5b4a54e91e4e493";
      friend["nickname"] = "sytest3";
      array.Add(friend);

      _infomation.isFirst = false;
      _infomation.FriendsData = array;
      UpdateVeiew();
    }

    public void SetFriendInfo2()
    {
      JArray array = new JArray();
      JObject friend = new JObject();

      friend["user_id"] = "01e69a0294c84cb7ad5b4a54e91e4e49";
      friend["nickname"] = "sytest1";
      array.Add(friend);
      friend["user_id"] = "01e69a0294c84cb7ad5b4a54e91e4e491";
      friend["nickname"] = "sytest2";
      array.Add(friend);

      _infomation.isFirst = false;
      _infomation.FriendsData = array;
      UpdateVeiew();
    }

    int msg_cnt = 0;
    public void setUnreadMsgCnt()
    {
      JObject roomMessageCnt = new JObject();
      roomMessageCnt["4bc6f4a832054a528a7379ffcaecb4274"] = ++msg_cnt;
      _room_manager.UpdateUnreadMsgCnt(roomMessageCnt);
    }
  }
}