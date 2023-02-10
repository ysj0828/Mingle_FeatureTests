using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushMagnetic : MonoBehaviour
{
  public float magnetForce = -1;

  List<Rigidbody> _ridibodies = new List<Rigidbody>();
  List<Rigidbody> _counter = new List<Rigidbody>();

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  // void FixedUpdate()
  // {
  //     for (int i = 0; i < _ridibodies.Count; i++)
  //     {
  //         if(Vector3.Distance(this.transform.position,Vector3.zero) <= Vector3.Distance(_ridibodies[i].transform.position,Vector3.zero))
  //             _ridibodies[i].velocity = (transform.position - (_ridibodies[i].transform.position + _ridibodies[i].centerOfMass)) * magnetForce * Time.deltaTime;
  //     }
  // }


  // private void OnTriggerEnter(Collider other) {
  //     if(other.name != "Magnetic") return;
  //     // if(Vector3.Distance(this.transform.position,Vector3.zero) <= Vector3.Distance(other.transform.position,Vector3.zero))
  //     Rigidbody r = other.transform.parent.GetComponent<Rigidbody>();
  //     if(!_ridibodies.Contains(r))
  //     {
  //         //Add Rigidbody
  //         _ridibodies.Add(r);
  //     }
  // }

  // private void OnTriggerExit(Collider other)
  // {
  //     if(other.name != "Magnetic") return;
  //     Rigidbody r = other.transform.parent.GetComponent<Rigidbody>();
  //     Debug.Log("AAAAAAAAAAAAAA : " + _ridibodies.Count);
  //     for (int i = 0; i < _ridibodies.Count; i++)
  //     {
  //         Debug.Log("AAAAAAAAAAAAAA3 : " + _ridibodies.Count);
  //         if(_ridibodies.Contains(r))
  //         {
  //             Debug.Log("AAAAAAAAAAAAAA2222 : " + _ridibodies.Count);
  //             //Add Rigidbody
  //             _ridibodies.Remove(r);
  //             r.velocity = Vector3.zero;
  //             r.angularVelocity = Vector3.zero;
  //             Debug.Log(transform.parent.name + " : " + other.transform.name);
  //         }
  //     }
  // }
}
