using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MingleMain
{
  public class RequestManager : MonoBehaviour
  {
    // Start is called before the first frame update
    void Start()
    {
      GameObject RoomSpherePrefab = Resources.Load<GameObject>("MingleMain/RequestSphere");
      RequestSphere request_sphere = Instantiate(RoomSpherePrefab, transform).GetComponent<RequestSphere>();
      Debug.Log(request_sphere);

      float scale = 1f;
      request_sphere.transform.localPosition = new Vector3(0f, 0f, scale / -1.4f);


      request_sphere.OrignalPosition = request_sphere.transform.localPosition;
      request_sphere.OrignalRotation = request_sphere.transform.localRotation;
      request_sphere.OrignalScale = Vector3.one * scale;
    }

    // Update is called once per frame
    void Update()
    {

    }
  }
}