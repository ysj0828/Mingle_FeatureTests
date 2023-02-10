using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {

  }

  private void LateUpdate()
  {
    Debug.Log("Update");
    gameObject.transform.position = new Vector3(3f, 7f, 1.5f);
  }
  // Update is called once per frame
  void Update()
  {

  }
}
