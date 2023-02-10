using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using com.unity.photon;

public class RoomManager : MonoBehaviour
{
  private Dictionary<string, GameObject> _rooms = new Dictionary<string, GameObject>();
  string _current_room = "";
  Infomation _infomation;

  private void Start()
  {
    _infomation = FindObjectOfType<Infomation>();
  }

  private void Update()
  {
    if (_current_room != _infomation.room_preview_uuid)
    {
      if (!_rooms.ContainsKey(_infomation.room_preview_uuid))
      {
        Debug.LogError("SelectRoom id error");
        return;
      }

      foreach (KeyValuePair<string, GameObject> items in _rooms)
      {
        if (items.Key == _infomation.room_preview_uuid)
        {
          items.Value.SetActive(true);
          _current_room = _infomation.room_preview_uuid;
        }
        else items.Value.SetActive(false);
      }
    }
  }

  private void Awake()
  {
    int size = transform.childCount;
    for (int i = 0; i < transform.childCount; i++)
    {
      var room = transform.GetChild(i).gameObject;
      // if (i == 0)
      // {
      //   room.SetActive(true);
      //   _current_room = room.name;
      // }
      // else 
      room.SetActive(false);
      _rooms.Add(room.name, room);
    }
  }

  public void SelectRoom(string message)
  {
    RNMessenger.SendToRN("SelectRoom " + message);
    JObject json = JObject.Parse(message);
    string room_uuid = json["room_uuid"].ToString();
    RNMessenger.SendToRN("SelectRoom " + json["room_uuid"].ToString());
    if (_current_room != room_uuid)
    {
      if (!_rooms.ContainsKey(room_uuid))
      {
        Debug.LogError("SelectRoom id error");
        return;
      }

      foreach (KeyValuePair<string, GameObject> items in _rooms)
      {
        if (items.Key == room_uuid)
        {
          items.Value.SetActive(true);
          _current_room = room_uuid;
        }
        else items.Value.SetActive(false);
      }
    }

  }

}
