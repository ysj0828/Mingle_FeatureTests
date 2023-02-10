using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MingleMain
{
  public class MainBackground : MonoBehaviour
  {
    private RoomManager _room_manager;
    private float _down_time;
    // Start is called before the first frame update
    void Start()
    {
      _room_manager = FindObjectOfType<RoomManager>();
      Debug.Log(_room_manager);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnMouseDown()
    {
      //Output the name of the GameObject that is being clicked
      _down_time = Time.time;
    }

    //Detect if clicks are no longer registering
    public void OnMouseUp()
    {
      if (_room_manager == null)
      {
        _room_manager = FindObjectOfType<RoomManager>();
        if (_room_manager == null) return;
      }
      if ((Time.time - _down_time) < 0.1f)
      {
        _room_manager.ClearSelect();
      }
    }
  }
}