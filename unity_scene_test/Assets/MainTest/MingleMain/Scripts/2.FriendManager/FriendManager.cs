using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using com.unity.photon;

namespace MingleMain
{
  public class FriendManager : MonoBehaviour
  {
    // Dictionary<GameObject, JObject> _heads = new Dictionary<GameObject, JObject>();
    List<GameObject> _heads = new List<GameObject>();
    List<JObject> _info = new List<JObject>();
    Infomation _infomation;
    // Start is called before the first frame update
    void Start()
    {
      _infomation = FindObjectOfType<Infomation>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateFriendData()
    {
      _infomation = FindObjectOfType<Infomation>();
      if (_infomation.FriendsData == null) return;
      for (int i = 0; i < _heads.Count; i++)
      {
        GameObject head = _heads[i];
        JObject info = _info[i];

        bool is_removed = true;
        foreach (JObject friend in _infomation.FriendsData)
        {
          // Debug.Log(i + " : " + info["user_id"].ToString() + " : " + friend["user_id"].ToString() + " : " + (info["user_id"].ToString() == friend["user_id"].ToString()));
          if (info["user_id"].ToString() == friend["user_id"].ToString())
          {
            is_removed = false;
            break;
          }
        }
        if (is_removed)
        {
          _heads.Remove(head);
          _info.Remove(info);
          Destroy(head);
          i--;
        }
      }

      GameObject prefab = Resources.Load<GameObject>("MingleMain/Head_Test");
      for (int i = 0; i < _infomation.FriendsData.Count; i++)
      {
        JObject friend = _infomation.FriendsData[i].ToObject<JObject>();
        bool isNew = true;
        for (int j = 0; j < _info.Count; j++)
        {
          JObject info = _info[j];
          if (info["user_id"].ToString() == friend["user_id"].ToString())
          {
            Debug.Log("not new " + info["nickname"].ToString());
            isNew = false;
            // sphere.UpdateRoomInfo(roomInfo["room"].ToObject<JObject>(), roomInfo["members"].ToObject<JArray>()); ;
            break;
          }
        }
        if (isNew)
        {
          GameObject head = Instantiate(prefab, transform);
          SphereCollider colider = head.GetComponent<SphereCollider>();
          colider.enabled = true;

          head.transform.localPosition = new Vector3(
            Random.RandomRange(-0.1f, 0.1f),
            Random.RandomRange(-0.1f, 0.1f),
            -1.4f
          );
          Debug.Log("Add new " + friend["nickname"].ToString());
          head.transform.localScale = Vector3.one * 5;
          head.transform.Find("NickName").GetComponent<TextMesh>().text = friend["nickname"].ToString();

          _heads.Add(head);
          _info.Add(friend);
        }
      }
    }

    public void OnClickHead(GameObject head)
    {
      for (int i = 0; i < _heads.Count; i++)
      {
        if (_heads[i] != head) continue;
        JObject friend = _info[i];


        JObject param = new JObject();
        param["id"] = friend["user_id"].ToString();
        param["nickname"] = friend["nickname"].ToString();

        JObject json = new JObject();
        json["cmd"] = "CreateChattingFriend";
        json["params"] = param;

        RNMessenger.SendToRN(JsonConvert.SerializeObject(json, Formatting.Indented));
      }
      // if (_heads.ContainsKey(head))
      // {
      //   JObject friend = _heads[head];


      //   JObject param = new JObject();
      //   param["id"] = friend["user_id"].ToString();
      //   param["nickname"] = friend["nickname"].ToString();

      //   JObject json = new JObject();
      //   json["cmd"] = "CreateChattingFriend";
      //   json["params"] = param;

      //   RNMessenger.SendToRN(JsonConvert.SerializeObject(json, Formatting.Indented));
      // }
    }
  }
}