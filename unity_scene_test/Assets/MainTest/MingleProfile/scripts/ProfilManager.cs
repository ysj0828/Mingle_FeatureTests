using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
  CharacterManager _character_manager;
  RoomManager _room_manager;
  private void Awake()
  {
    _character_manager = transform.GetChild(0).GetComponent<CharacterManager>();
    _room_manager = transform.GetChild(1).GetComponent<RoomManager>();
  }

  public void UpdateProfile(string message)
  {
    _character_manager.SelectCharacter(message);
    _room_manager.SelectRoom(message);
  }

}
