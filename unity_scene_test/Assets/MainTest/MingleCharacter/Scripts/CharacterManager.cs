using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.unity.photon;

public class CharacterManager : MonoBehaviour
{
  private Dictionary<string, GameObject> _characters = new Dictionary<string, GameObject>();
  Animator _current_character_animator;
  List<AnimationClip> clips = new List<AnimationClip>();

  string[] current_animation = { " " };
  string[] last_animation = { " " };
  string _current_character = "";
  Infomation _infomation;

  private void Awake()
  {
    int size = transform.childCount;
    for (int i = 0; i < transform.childCount; i++)
    {
      var character = transform.GetChild(i).gameObject;
      // if (i == 0)
      // {
      //   character.SetActive(true);
      //   _current_character = character.name;
      //   _current_character_animator = character.GetComponent<Animator>();
      // }
      // else 
      character.SetActive(false);
      _characters.Add(character.name, character);
    }
  }

  private void Update()
  {
    if (_current_character_animator != null) AnimReset();

    if (_current_character != _infomation.character_preview_uuid)
    {
      if (!_characters.ContainsKey(_infomation.character_preview_uuid))
      {
        Debug.LogError("SelectCharacter id error");
        return;
      }

      foreach (KeyValuePair<string, GameObject> items in _characters)
      {
        if (items.Key == _infomation.character_preview_uuid)
        {
          items.Value.SetActive(true);
          _current_character = _infomation.character_preview_uuid;
          _current_character_animator = _characters[_current_character].GetComponent<Animator>();
        }
        else items.Value.SetActive(false);
      }
    }
  }
  private void Start()
  {
    var _current_character_animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
    for (int i = 0; i < _current_character_animator.runtimeAnimatorController.animationClips.Length; i++)
    {
      //Clip배열의 순서는 animator에 넣는 순서로 정해짐 . idle은 무조건 Clip[0](첫번째)여야함 
      clips.Add(_current_character_animator.runtimeAnimatorController.animationClips[i]);
    }
    _infomation = FindObjectOfType<Infomation>();
  }

  public void SelectCharacter(string message)
  {
    RNMessenger.SendToRN("SelectCharacter " + message);
    JObject json = JObject.Parse(message);
    string character_uuid = json["character_uuid"].ToString();
    RNMessenger.SendToRN("SelectCharacter " + json["character_uuid"].ToString());
    if (_current_character != character_uuid)
    {
      if (!_characters.ContainsKey(character_uuid))
      {
        Debug.LogError("SelectCharacter id error");
        return;
      }

      foreach (KeyValuePair<string, GameObject> items in _characters)
      {
        if (items.Key == character_uuid)
        {
          items.Value.SetActive(true);
          _current_character = character_uuid;
          _current_character_animator = _characters[_current_character].GetComponent<Animator>();
        }
        else items.Value.SetActive(false);
      }
    }

    updateAnime();
  }


  public void SelectRoom(string message)
  {
    JObject json = JObject.Parse(message);
    RNMessenger.SendToRN("CharacterManager SelectRoom " + message);
    string room_uuid = json["room_uuid"].ToString();
    RNMessenger.SendToRN("CharacterManager SelectRoom " + json["room_uuid"].ToString());

  }

  public void Ani_SetInteger(int change_num)
  {
    _current_character_animator.SetInteger("animation", change_num);
  }

  public void updateAnime()
  {
    _current_character_animator = _characters[_current_character].GetComponent<Animator>();

    Ani_SetInteger(Random.Range(1, _current_character_animator.runtimeAnimatorController.animationClips.Length));

    for (int i = 0; i < _current_character_animator.runtimeAnimatorController.animationClips.Length; i++)
    {
      if (_current_character_animator.GetInteger("animation") == i)
      {
        char split_text = ' ';
        current_animation = clips[i].ToString().Split(split_text);

        if (current_animation[0] != last_animation[0])
        {
          last_animation[0] = current_animation[0];
          _current_character_animator.Play(current_animation[0]);
          if (_current_character_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f) _current_character_animator.CrossFade(current_animation[0], 0.1f);
        }
        else
        {
          updateAnime();
        }
      }
    }
  }

  void AnimReset()
  {
    if (_current_character_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !_current_character_animator.GetCurrentAnimatorStateInfo(0).IsName("idle_B"))
    {
      _current_character_animator.Play("idle_B");
      Debug.Log("AnimReset");
    }
  }

}