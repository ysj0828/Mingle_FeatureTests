using System.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : AnimateObject
{
  Timer timer;
  static int _radom_cnt = 0;
  Vector3 _rotate_val = Vector3.zero;
  float _rotate_x;
  float _rotate_y;
  float _rotate_z;
  void Start()
  {
    // transform.localRotation = Quaternion.Euler(-90, 0, 180);
    // transform.localRotation = Quaternion.Euler(0, 0, 0);
    _rotate_x = Random.Range(0f, 50f);
    _rotate_y = Random.Range(0f, 50f);
    _rotate_z = Random.Range(50f, 200f);
  }
  void FixedUpdate()
  {
    // transform.localRotation* Quaternion.Euler(
    //     Time.deltaTime * Random.Range(00.0f, 50.0f),
    //     Time.deltaTime * Random.Range(00.0f, 50.0f),
    //     Time.deltaTime * Random.Range(00.0f, 200.0f)
    // );

    transform.localRotation = Quaternion.Lerp(
      transform.localRotation,
      transform.localRotation * Quaternion.Euler(
          _rotate_x,
          _rotate_y,
          _rotate_z
        ),
      Time.deltaTime
    );
  }

  void Update()
  {
  }
}
