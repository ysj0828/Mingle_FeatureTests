using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Exoa.Cameras;
using com.unity.photon;

namespace MingleMain
{
  public class RequestSphere : AnimateObject
  {
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnMouseUp()
    {
      JObject json = new JObject();
      json["cmd"] = "ShowRequestFriend";
      RNMessenger.SendToRN(JsonConvert.SerializeObject(json, Formatting.Indented));
    }
  }
}