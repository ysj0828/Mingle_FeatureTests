using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using com.unity.photon;
using UnityEngine;

namespace MingleMain
{
  public class RoomManager : MonoBehaviour
  {
    // Start is called before the first frame update

    public float StartDely = 3f;
    public float AnimiationTime = 3f;
    List<RoomSphere> _room_spheres = new List<RoomSphere>();
    RoomSphere _current_selected_room;
    CameraManager _camera_manager;
    Infomation _infomation;
    float SelectedSphereSize = 3f;

    void Start()
    {
      _camera_manager = FindObjectOfType<CameraManager>();

    }

    void FixedUpdate()
    {
      return;
      if (_current_selected_room != null) return;
      // return;
      //1 중심기준으로 정렬
      List<RoomSphere> sort_sphere = new List<RoomSphere>(_room_spheres);
      List<Vector3> target_position = new List<Vector3>();

      for (int i = 0; i < sort_sphere.Count; i++) target_position.Add(sort_sphere[i].transform.localPosition);

      // for (int i = 0; i < sort_sphere.Count; i++) target_position.Add(Vector3.zero);
      sort_sphere.Sort(
        (sphereA, sphereB) =>
          Vector3.Distance(sphereA.transform.localPosition, Vector3.zero).CompareTo(Vector3.Distance(sphereB.transform.localPosition, Vector3.zero))
      );

      //2 중심에서 가까운것부터 다음 구 와 일정거리 떨어지도록 업데이트 (구1스케일+구2스케일)/2
      for (int i = 0; i < sort_sphere.Count; i++)
      {
        for (int j = 0; j < sort_sphere.Count; j++)
        {
          if (j == i) continue;
          if (sort_sphere[j].IsMoving()) continue;
          if (Vector3.Distance(sort_sphere[i].transform.localPosition, sort_sphere[j].transform.localPosition) < (sort_sphere[i].OrignalScale + sort_sphere[j].OrignalScale).x / 2f)
          {
            Debug.Log(i + ":" + j);
            Vector3 updatePosition = getUpdatePosition(sort_sphere[i].transform.localPosition, sort_sphere[j].transform.localPosition, 1);
            Debug.Log(updatePosition);
            target_position[j] += updatePosition;
          }
        }
      }


      for (int i = 0; i < sort_sphere.Count; i++)
      {
        if (sort_sphere[i].IsMoving()) continue;
        Debug.Log(sort_sphere[i] + " : " + target_position[i]);
        Vector3 moving_position = Vector3.LerpUnclamped(sort_sphere[i].transform.localPosition, target_position[i], Time.deltaTime);
        // if (i == 0) Debug.Log(target_position[i] + "  : " + moving_position);
        sort_sphere[i].transform.localPosition = moving_position;
        sort_sphere[i].OrignalPosition = moving_position;
        // sort_sphere[i].UpdateTransformation(
        //   target_position[i],
        //     sort_sphere[i].OrignalRotation,
        //     sort_sphere[i].OrignalScale
        // );
      }

    }

    // Update is called once per frame
    void Update()
    {
    }
    float getDegree(Vector3 from, Vector3 to)
    {
      float degree = Mathf.Atan2(to.y - from.y, to.x - from.x) * 180 / Mathf.PI;
      degree = degree < 0 ? degree + 360 : degree;
      return Mathf.Round(degree);
    }

    Vector2 getUpdatePosition(Vector3 from, Vector3 to, float scale)
    {
      Vector3 val = new Vector3(0, 0, 0);
      float degree = getDegree(from, to);
      if (degree >= 0 && degree < 90) { val.y = degree / 90; val.x = 1 - val.y; }
      if (degree >= 90 && degree < 180) { degree = degree % 90; val.x = degree / -90; val.y = val.x + 1; }
      if (degree >= 180 && degree < 270) { degree = degree % 90; val.y = degree / -90; val.x = -val.y - 1; }
      if (degree >= 270 && degree < 360) { degree = degree % 90; val.x = degree / 90; val.y = val.x - 1; }

      val *= scale;
      return val;
    }

    bool is_join = false;
    public void JoinRoom(RoomSphere select_sphere)
    {
      if (is_join) return;
      is_join = true;
      var paramData = new JObject();
      paramData["id"] = select_sphere.Id;

      var json = new JObject();

      json["cmd"] = "ConnectRoom";
      json["params"] = paramData;

      RNMessenger.SendToRN(JsonConvert.SerializeObject(json, Formatting.Indented));
    }

    public void SelectRoom(RoomSphere select_sphere)
    {

      if (GameObject.ReferenceEquals(_current_selected_room, select_sphere))
      {
        ClearSelect();
        return;
      }
      _current_selected_room = select_sphere;

      for (int i = 0; i < _room_spheres.Count; i++)
      {
        RoomSphere sphere = _room_spheres[i];
        sphere.draggable = false;
        if (GameObject.ReferenceEquals(sphere, select_sphere)) continue;


        Rigidbody sphere_rigdi = sphere.GetComponent<Rigidbody>();
        sphere_rigdi.freezeRotation = false;

        sphere.OrignalPosition = sphere.transform.localPosition;
        sphere.OrignalScale = sphere.transform.localScale;

        Vector3 update_position = getUpdatePosition(select_sphere.OrignalPosition, sphere.OrignalPosition, SelectedSphereSize - select_sphere.OrignalScale.x);

        sphere.UpdatePositionScale(
            sphere.OrignalPosition + update_position,
            sphere.OrignalScale
          );
        sphere.HideHeads();
      }

      Rigidbody select_sphere_rigdi = select_sphere.GetComponent<Rigidbody>();
      select_sphere_rigdi.velocity = Vector3.zero;
      select_sphere_rigdi.angularVelocity = Vector3.zero;
      select_sphere_rigdi.freezeRotation = true;

      select_sphere.transform.localRotation = Quaternion.Euler(0, 0, 0);

      select_sphere.UpdatePositionScale(
            select_sphere.OrignalPosition + new Vector3(0f, 0f, -SelectedSphereSize / 2 - select_sphere.OrignalPosition.z),
            Vector3.one * SelectedSphereSize
      );
      select_sphere.ShowHeads();

      _camera_manager.UpdateTransformation(
        new Vector3(select_sphere.OrignalPosition.x, 10f, select_sphere.OrignalPosition.y),
        Quaternion.Euler(90f, 0f, 0f),
        new Vector3(0f, 0f, 0f)
      );
    }
    public void ClearSelect()
    {
      if (_current_selected_room == null) return;

      _current_selected_room = null;

      for (int i = 0; i < _room_spheres.Count; i++)
      {
        RoomSphere sphere = _room_spheres[i];
        sphere.draggable = true;
        sphere.UpdatePositionScale(
            sphere.OrignalPosition,
            sphere.OrignalScale
          );
        sphere.HideHeads();
        sphere.GetComponent<Rigidbody>().freezeRotation = false;
      }

      // CameraModeSwitcher.Instance.DisableMoves = false;

      // CameraModeSwitcher.Instance.MoveCameraTo(
      //   new Vector3(0f, Camera.main.transform.position.z, 0f),
      //   5f
      // );
      // Debug.Log(Camera.main.transform.position);
      _camera_manager.UpdateTransformation(
        new Vector3(0f, 10f, 0f),
        Quaternion.Euler(90f, 0f, 0f),
        new Vector3(0f, 0f, 0f)
      );
    }


    public void UpdateRoomPosition()
    {
      for (int i = 0; i < _room_spheres.Count; i++)
      {
        RoomSphere room_sphere = _room_spheres[i];
        room_sphere.OrignalPosition = room_sphere.transform.localPosition;
      }
    }

    public void updateRoomData()
    {
      _infomation = FindObjectOfType<Infomation>();
      if (_infomation.RoomData == null) return;

      for (int i = 0; i < _room_spheres.Count; i++)
      {
        RoomSphere sphere = _room_spheres[i];
        bool is_removed = true;
        foreach (JObject roomInfo in _infomation.RoomData)
        {
          if (sphere.Id == roomInfo["room"]["id"].ToString())
          {
            is_removed = false;
            break;
          }
        }
        if (is_removed)
        {
          _room_spheres.Remove(sphere);
          Destroy(sphere.gameObject);
          i--;
        }
      }

      for (int i = 0; i < _infomation.RoomData.Count; i++)
      {
        JObject roomInfo = _infomation.RoomData[i].ToObject<JObject>();
        bool isNewRoom = true;
        for (int j = 0; j < _room_spheres.Count; j++)
        {
          RoomSphere sphere = _room_spheres[j];
          if (sphere.Id == roomInfo["room"]["id"].ToString())
          {
            isNewRoom = false;
            sphere.UpdateRoomInfo(roomInfo["room"].ToObject<JObject>(), roomInfo["members"].ToObject<JArray>()); ;
            break;
          }
        }

        if (isNewRoom) addRoomInfo(roomInfo);

      }
    }

    private void addRoomInfo(JObject roomInfo)
    {
      GameObject RoomSpherePrefab = Resources.Load<GameObject>("MingleMain/RoomSphere");
      RoomSphere room_sphere = Instantiate(RoomSpherePrefab, transform).GetComponent<RoomSphere>();

      float scale = 1;

      room_sphere.transform.localPosition = new Vector3(
        Random.RandomRange(-0.1f, 0.1f),
        Random.RandomRange(-0.1f, 0.1f),
        scale / -1.4f
      );

      room_sphere.OrignalPosition = new Vector3(room_sphere.transform.localPosition.x, room_sphere.transform.localPosition.y, scale / -1.4f);
      room_sphere.OrignalRotation = room_sphere.transform.localRotation;
      room_sphere.OrignalScale = Vector3.one * scale;

      room_sphere.UpdateRoomInfo(roomInfo["room"].ToObject<JObject>(), roomInfo["members"].ToObject<JArray>()); ;

      _room_spheres.Add(room_sphere);
    }

    public void UpdateUnreadMsgCnt(JObject roomMessageCnt)
    {
      for (int i = 0; i < _room_spheres.Count; i++)
      {
        RoomSphere sphere = _room_spheres[i];
        sphere.UpdateMsgCnt(roomMessageCnt[sphere.Id].ToObject<int>());
      }
    }
  }
}