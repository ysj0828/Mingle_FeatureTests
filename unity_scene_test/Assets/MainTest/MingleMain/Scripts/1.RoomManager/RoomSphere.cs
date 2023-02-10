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
  public class RoomSphere : AnimateObject
  {
    private RoomManager _parent;
    private float _mouse_down_time;
    private List<GameObject> _head = new List<GameObject>();
    public bool draggable = true;
    private bool _isDrag = false;
    private JArray _members = new JArray();

    public string Title { get; set; }
    public string Id { get; set; }
    public int MsgCnt { get; set; }
    private Ray _startRay;
    public bool IsMoving()
    {
      return _animation_enabled;
    }
    public void OnMouseDown()
    {
      _mouse_down_time = Time.time;
      _startRay = Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    public void OnMouseUp()
    {

      if ((Time.time - _mouse_down_time) < 0.1f)
      {
        if (!_isDrag) _parent.JoinRoom(this);
        //if (!_isDrag) _parent.SelectRoom(this);
      }
      else
      {
        if (!draggable) return;
        _parent.UpdateRoomPosition();
      }
      _isDrag = false;
    }

    private void OnMouseDrag()
    {
      if (!draggable) return;
      Ray R;
      if (!_isDrag)
      {
        R = Camera.main.ScreenPointToRay(Input.mousePosition);
        _isDrag = Vector3.Distance(_startRay.origin, R.origin) > 0.1f;
        return;
      }

      // R = Camera.main.ScreenPointToRay(Input.mousePosition);
      // transform.position -= (_startRay.origin - R.origin);
      // _startRay = R;

      R = Camera.main.ScreenPointToRay(Input.mousePosition); // Get the ray from mouse position
      Vector3 PO = transform.position; // Take current position of this draggable object as Plane's Origin
      Vector3 PN = -Camera.main.transform.forward; // Take current negative camera's forward as Plane's Normal
      float t = Vector3.Dot(PO - R.origin, PN) / Vector3.Dot(R.direction, PN); // plane vs. line intersection in algebric form. It find t as distance from the camera of the new point in the ray's direction.
      Vector3 P = R.origin + R.direction * t; // Find the new point.
      transform.position = P;
    }
    // Start is called before the first frame update
    void Start()
    {
      _parent = transform.parent.GetComponent<RoomManager>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    // void FixedUpdate()
    // {
    //   // Debug.Log("Update");
    //   if ((Time.time - _mouse_down_time) > 0.1f) UpdateOrignalPosition();
    // }

    // void UpdateOrignalPosition()
    // {

    // }


    void MakeHeads()
    {
      GameObject prefab = Resources.Load<GameObject>("MingleMain/Head_Test");

      _head = new List<GameObject>(_members.Count);
      for (int i = 0; i < _members.Count; i++)
      {
        JObject member = _members[i].ToObject<JObject>();

        GameObject head = Instantiate(prefab, transform);
        head.transform.Find("NickName").GetComponent<TextMesh>().text = member["nickname"].ToString();

        _head.Add(head);
      }
    }

    public void ShowHeads()
    {
      if (_head.Count == 0) MakeHeads();
      for (int i = 0; i < _head.Count; i++)
      {
        GameObject head = _head[i];

        RoomHead head_script = head.GetComponent<RoomHead>();

        head.transform.localPosition = new Vector3(0, 0.7f, 0);
        head.transform.RotateAround(transform.position, Vector3.up, i * 40);

        Vector3 moving_position = new Vector3(head.transform.localPosition.x, head.transform.localPosition.y, head.transform.localPosition.z);

        head.transform.localPosition = new Vector3(0, 0, 0);
        head.transform.localRotation = Quaternion.Euler(0, 0, 0);

        head_script.ShowHead(moving_position);
        // head_script.UpdateTransformation(moving_position,
        //           head.transform.localRotation,
        //           head.transform.localScale);
      }
    }

    public void HideHeads()
    {
      if (_head.Count == 0) return;
      for (int i = 0; i < _head.Count; i++)
      {
        GameObject head = _head[i];
        if (!head.activeSelf) continue;
        // head.SetActive(false);
        RoomHead head_script = head.GetComponent<RoomHead>();
        head_script.HideHead();
      }
      _head.Clear();
    }

    private void OnCollisionEnter(Collision collision)
    {
      // if (!GameObject.ReferenceEquals(collision.gameObject, gameObject)) return;
      // Handheld.Vibrate();
    }

    public void UpdateRoomInfo(JObject json, JArray memvers)
    {
      Title = json["title"].ToString();
      Id = json["id"].ToString();
      _members = memvers;
    }

    public void UpdateMsgCnt(int msgcnt)
    {
      if (MsgCnt == msgcnt) return;
      MsgCnt = msgcnt;
    }
  }
}