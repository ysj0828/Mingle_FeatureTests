using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using com.unity.photon;
using Photon.Pun;

namespace MingleCamera
{
  public class cinemachine_test : MonoBehaviour
  {
    public GameObject virtualCamFraming;


    public CinemachineFramingTransposer vcamFraming;

    public CinemachineVirtualCamera vcamFramingVC;

    CinemachinePOV vcamFramingPOV;

    public CinemachineVirtualCamera firstPerson;

    public bool inEditMode;

    public Transform refTarget;
    [SerializeField] Transform camPos;

    [HideInInspector] public GameObject AvatarNullCheck;

    [HideInInspector] public bool isCameraFirstPerson;

    bool isRecentering;

    public bool switchCams = true;

    [HideInInspector]
    public GameObject currentActiveAvatar
    {
      get
      {
        return activeAvatar;
      }

      set
      {
        activeAvatar = value;

      }
    }

    GameObject activeAvatar;


    float touchX, touchY, touchXX, touchYY, combinedX, combinedY, moveScale;
    float zoomMax = 13;
    float zoomMin = 0.7f;
    float firstPersonMagnitude;

    float initialCamDistance;

    Vector2 InputTwoTouchOne;
    Vector2 InputTwoTouchTwo;

    Vector2 firstPersonInitialOne;
    Vector2 firstPersonInitialTwo;

    Vector2 InputTwoInitialOne;
    Vector2 InputTwoInitialTwo;

    //

    //

    Vector2 initPos;

    bool enableCamMove = true;
    public bool enableRotation = true;

    //GyroControl GC;

    public void SetAvatarAsMain()
    {
      vcamFramingVC.LookAt = refTarget;
      vcamFramingVC.Follow = refTarget;
      enableCamMove = true;

      vcamFraming.m_DeadZoneHeight = 0;
      vcamFraming.m_DeadZoneWidth = 0;

      vcamFramingPOV.m_VerticalAxis.m_MinValue = 0;
    }

    public void SetObjectAsMain(RaycastHit hit)
    {
      vcamFramingVC.LookAt = hit.transform;
      vcamFramingVC.Follow = hit.transform;
      vcamFraming.m_CameraDistance = 10;
      vcamFramingPOV.m_VerticalAxis.Value = 45;
      enableCamMove = false;

      //vcamFraming.m_DeadZoneHeight = 0.6f;
      //vcamFraming.m_DeadZoneWidth = 0.4f;

      vcamFraming.m_XDamping = 1;
      vcamFraming.m_YDamping = 1;
      vcamFraming.m_ZDamping = 1;

      vcamFramingPOV.m_VerticalAxis.m_MinValue = 10;
    }

    // static cinemachine_test instance = null;


    //start()
    // public void InitialiseCinemachine()
    // {
    //     if (virtualCamFraming == null) virtualCamFraming = Camera.main.gameObject;
    //     vcamFramingVC = virtualCamFraming.transform.GetComponent<CinemachineVirtualCamera>();

    //     vcamFraming = vcamFramingVC.GetCinemachineComponent<CinemachineFramingTransposer>();
    //     vcamFramingPOV = vcamFramingVC.GetCinemachineComponent<CinemachinePOV>();
    //     //GC = GameObject.FindGameObjectWithTag("PlayerAvatar").GetComponentInChildren<GyroControl>();

    //     vcamFramingPOV.m_HorizontalAxis.m_InputAxisName = "";
    //     vcamFramingPOV.m_VerticalAxis.m_InputAxisName = "";
    // }

    void Start()
    {
      // if (instance == null)
      // {
      //     instance = this;
      // }
      // else
      // {
      //     Destroy(this.gameObject);
      // }
      // DontDestroyOnLoad(this.gameObject);
      GameObject[] avatarArray = GameObject.FindGameObjectsWithTag("PlayerAvatar");

      vcamFramingVC = virtualCamFraming.transform.GetComponent<CinemachineVirtualCamera>();

      vcamFraming = vcamFramingVC.GetCinemachineComponent<CinemachineFramingTransposer>();
      vcamFramingPOV = vcamFramingVC.GetCinemachineComponent<CinemachinePOV>();
      //GC = GameObject.FindGameObjectWithTag("PlayerAvatar").GetComponentInChildren<GyroControl>();

      vcamFramingPOV.m_HorizontalAxis.m_InputAxisName = "";
      vcamFramingPOV.m_VerticalAxis.m_InputAxisName = "";

      if (vcamFramingVC.Follow == null || vcamFramingVC.LookAt == null)
      {
        foreach (GameObject go in avatarArray)
        {

          if (go.GetComponent<PhotonView>().IsMine && go.activeInHierarchy)
          {
            refTarget = go.transform;
            vcamFramingVC.Follow = refTarget;
            vcamFramingVC.LookAt = refTarget;
          }

          else
          {
            continue;
          }
        }
      }
      //transposer = virtualCam.transform.GetComponent<CinemachineFramingTransposer>();
      //CinemachineCore.GetInputAxis = HandleAxisInputDelegate;
    }

    float currentDistance;
    float prevDistance;

    float touchTime;

    bool goBackToFreeLook;

    float timeForZoomToFirstPersonChange;


    public void Rotate()
    {
      if (Input.touchCount == 1 && switchCams)
      {
        if (vcamFramingVC.LookAt == null)
        {
          vcamFramingVC.LookAt = refTarget;
        }

        Touch touch = Input.touches[0];

        if (touch.phase == TouchPhase.Began || initPos == new Vector2(999, 999))
        {
          initPos = touch.position;
        }

        if ((initPos - touch.position).magnitude > 100)
        {
          if (vcamFraming.m_TrackedObjectOffset != Vector3.zero)
          {
            vcamFraming.m_TrackedObjectOffset = Vector3.Lerp(vcamFraming.m_TrackedObjectOffset, Vector3.zero, (vcamFraming.transform.position - refTarget.position).magnitude * Time.deltaTime);
          }

          vcamFramingPOV.m_HorizontalAxis.Value += touch.deltaPosition.x * 0.05f;

          vcamFramingPOV.m_VerticalAxis.Value -= touch.deltaPosition.y * 0.04f;
        }
      }
    }

    public void MoveAndZoom()
    {

    }

    private void Update()
    {
      if (AvatarNullCheck != null && vcamFramingVC.LookAt == null && vcamFramingVC.Follow == null)
      {
        var temp_Camera_Head = AvatarNullCheck.transform.Find("Camera_Head");

        GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<cinemachine_test>().refTarget = temp_Camera_Head;

        GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<cinemachine_test>().vcamFramingVC.LookAt = temp_Camera_Head;
        GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<cinemachine_test>().vcamFramingVC.Follow = temp_Camera_Head;

        firstPerson = AvatarNullCheck.transform.Find("FirstPerson").GetComponent<CinemachineVirtualCamera>();

        //GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<TapToMove>().avatarGO = temp;
      }

      //if (vcamFraming.m_DeadZoneWidth != 0.5f)
      //{
      //    vcamFraming.m_DeadZoneWidth = 0.5f;
      //}

      if (vcamFraming.m_ScreenX != 0.5f || vcamFraming.m_ScreenY != 0.5f)
      {
        vcamFraming.m_ScreenX = 0.5f;
        vcamFraming.m_ScreenY = 0.5f;
      }

      //if (vcamFraming.m_DeadZoneWidth != 0.5f)
      //{
      //    vcamFraming.m_DeadZoneWidth = 0.5f;
      //}

      if (Input.touchCount == 0)
      {
        if (touchTime != 0)
        {
          touchTime = 0;
        }

        if (InputTwoTouchOne != new Vector2(999, 999) || InputTwoTouchTwo != new Vector2(999, 999) || initPos != new Vector2(999, 999) || InputTwoInitialOne != new Vector2(999, 999) || InputTwoInitialTwo != new Vector2(999, 999))
        {
          InputTwoTouchOne = new Vector2(999, 999);
          InputTwoTouchTwo = new Vector2(999, 999);
          initPos = new Vector2(999, 999);

          InputTwoInitialOne = new Vector2(999, 999);
          InputTwoInitialTwo = new Vector2(999, 999);
        }

        if (currentDistance != 0 || prevDistance != 0)
        {
          currentDistance = 0;
          prevDistance = 0;
        }

        if (!switchCams)
        {
          switchCams = true;
        }

        if (isCameraFirstPerson)
        {
          if (goBackToFreeLook)
          {
            firstPerson.Priority = 10;
            //firstPerson.transform.rotation = Quaternion.identity;
            firstPerson.transform.localEulerAngles = Vector3.zero;
            //GC.enabled = false;
            goBackToFreeLook = false;
            isCameraFirstPerson = false;
          }
        }
      }

      if (Input.touchCount == 1 && switchCams && enableRotation)
      {
        if (vcamFramingVC.LookAt == null)
        {
          vcamFramingVC.LookAt = refTarget;
        }

        Touch touch = Input.touches[0];

        if (touch.phase == TouchPhase.Began || initPos == new Vector2(999, 999))
        {
          initPos = touch.position;
        }

        if ((initPos - touch.position).magnitude > 100)
        {
          if (vcamFraming.m_TrackedObjectOffset != Vector3.zero)
          {
            vcamFraming.m_TrackedObjectOffset = Vector3.Lerp(vcamFraming.m_TrackedObjectOffset, Vector3.zero, (vcamFraming.transform.position - refTarget.position).magnitude * Time.deltaTime);
          }

          vcamFramingPOV.m_HorizontalAxis.Value += touch.deltaPosition.x * 0.05f;

          vcamFramingPOV.m_VerticalAxis.Value -= touch.deltaPosition.y * 0.04f;
        }
      }

      if (Input.touchCount == 2)
      {
        Touch touches1 = Input.GetTouch(0);
        Touch touches2 = Input.GetTouch(1);

        if (InputTwoInitialOne == new Vector2(999, 999) || InputTwoInitialTwo == new Vector2(999, 999))
        {
          InputTwoInitialOne = touches1.position;
          InputTwoInitialTwo = touches2.position;
          initialCamDistance = vcamFraming.m_CameraDistance;
        }

        if (switchCams)
        {
          switchCams = false;
        }

        if (vcamFramingVC.LookAt != null)
        {
          vcamFramingVC.LookAt = null;
        }

        touchX = touches1.deltaPosition.x;
        touchY = touches1.deltaPosition.y;

        touchXX = touches2.deltaPosition.x;
        touchYY = touches2.deltaPosition.y;

        combinedX = touchX + touchXX;
        combinedY = touchY + touchYY;

        float dotProduct = Vector2.Dot(touches1.position - InputTwoInitialOne, touches2.position - InputTwoInitialTwo);

        if (dotProduct < 0)
        {
          Vector2 touchZeroPos = touches1.position - touches1.deltaPosition;
          Vector2 touchOnePos = touches2.position - touches2.deltaPosition;

          float prevMagnitude = (touchZeroPos - touchOnePos).magnitude;

          float currentMagnitude = (touches1.position - touches2.position).magnitude;

          float diff = currentMagnitude - prevMagnitude;

          if (diff < 0)
          {
            //zoom out

            if (isCameraFirstPerson)
            {
              if (InputTwoTouchOne == new Vector2(999, 999) || InputTwoTouchTwo == new Vector2(999, 999))
              {
                firstPersonInitialOne = touches1.position;
                firstPersonInitialTwo = touches2.position;
              }

              InputTwoTouchOne = touches1.position;
              InputTwoTouchTwo = touches2.position;

              float prevFirstPersonMagnitude = (firstPersonInitialOne - firstPersonInitialTwo).magnitude;

              float currentFirstPersonMagnitude = (touches1.position - touches2.position).magnitude;

              firstPersonMagnitude = currentFirstPersonMagnitude - prevFirstPersonMagnitude;

              if (firstPersonMagnitude <= -500)
              {
                goBackToFreeLook = true;

                if (vcamFraming.m_CameraDistance < 3)
                {
                  vcamFraming.m_CameraDistance = 5;
                }

                //vcamFraming.m_CameraDistance = 5;
                //vcamFramingPOV.m_HorizontalAxis.Value = 180;
                //vcamFramingPOV.m_VerticalAxis.Value = 25;
              }

              Debug.Log("change pov");
            }

            else
            {
              vcamFraming.m_CameraDistance -= diff * 0.01f;
              vcamFraming.m_CameraDistance = Mathf.Clamp(vcamFraming.m_CameraDistance, zoomMin, zoomMax);
            }
          }

          else if (diff > 0)
          {
            //zoom in
            vcamFraming.m_CameraDistance -= diff * 0.01f;
            vcamFraming.m_CameraDistance = Mathf.Clamp(vcamFraming.m_CameraDistance, zoomMin, zoomMax);

            if (initialCamDistance <= 0.75)
            {
              touchTime += Time.deltaTime;
              if (touchTime >= 2)
              {
                //change pov
                changeToFirstPerson();
              }
            }
          }
        }

        if (dotProduct > 0 && enableCamMove)
        {
          //move
          moveScale = Mathf.Abs(touchX) + Mathf.Abs(touchY);
          moveScale = Mathf.Clamp(moveScale, 0, 45);

          Vector3 newV = Quaternion.Euler(0, virtualCamFraming.transform.eulerAngles.y, 0) * new Vector3(-combinedX, -combinedY, 0);

          //vcamFraming.m_TrackedObjectOffset += Quaternion.Euler(0, virtualCamFraming.transform.eulerAngles.y, 0) * newV * moveScale * 0.0001f;
          vcamFraming.m_TrackedObjectOffset += newV * moveScale * 0.0001f;

          //float minY = camPos.transform.position.y - refTarget.position.y;
          //vcamFraming.m_TrackedObjectOffset = new Vector3(vcamFraming.m_TrackedObjectOffset.x, Mathf.Clamp(vcamFraming.m_TrackedObjectOffset.y, -minY - 1f, 20), vcamFraming.m_TrackedObjectOffset.z);
          vcamFraming.m_TrackedObjectOffset = new Vector3(vcamFraming.m_TrackedObjectOffset.x, Mathf.Clamp(vcamFraming.m_TrackedObjectOffset.y, -1, 20), vcamFraming.m_TrackedObjectOffset.z);
        }
      }
    }

    public void changeToFirstPerson()
    {
      firstPerson.Priority = 30;

      isCameraFirstPerson = true;

      //GC.enabled = true;
    }

    public void recenterCameraOnMove()
    {
      if (vcamFraming.m_TrackedObjectOffset.magnitude >= 0.1f)
      {
        vcamFraming.m_TrackedObjectOffset = Vector3.Lerp(vcamFraming.m_TrackedObjectOffset, Vector3.zero, (vcamFraming.transform.position - refTarget.position).magnitude * 0.8f * Time.deltaTime);

        if (vcamFraming.m_TrackedObjectOffset.magnitude <= 0.11f)
        {
          isRecentering = false;
          return;
        }

        else
        {
          recenterCameraOnMove();
        }
      }
    }


    bool isPointerOverUI(Touch touch)
    {
      PointerEventData eventData = new PointerEventData(EventSystem.current);

      eventData.position = new Vector2(touch.position.x, touch.position.y);

      List<RaycastResult> results = new List<RaycastResult>();

      EventSystem.current.RaycastAll(eventData, results);
      return results.Count > 0;
    }
  }
}
