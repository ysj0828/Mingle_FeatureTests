using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomHead : AnimateObject
{
  // Start is called before the first frame update
  GameObject _mesh = null;
  float _hide_time = -1f;

  void Start()
  {
  }

  // Update is called once per frame
  void Update()
  {
    if (_hide_time != -1f && _hide_time < Time.realtimeSinceStartup)
    {
      _hide_time = -1f;
      transform.gameObject.SetActive(false);
      Destroy(gameObject);
    }
  }

  public void ShowHead(Vector3 target_position)
  {
    _hide_time = -1f;
    transform.gameObject.SetActive(true);
    if (_mesh == null) _mesh = transform.Find("Head_Mesh").gameObject;
    _mesh.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 120));
    UpdateTransformation(target_position,
              transform.localRotation,
              transform.localScale);
  }
  public void HideHead()
  {
    UpdateTransformation(Vector3.zero,
              transform.localRotation,
              transform.localScale);
    _hide_time = Time.realtimeSinceStartup + 0.5f;
  }

}
