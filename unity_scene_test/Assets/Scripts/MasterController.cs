using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MingleCamera;
using UnityEngine.EventSystems;
using Photon.Pun;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
//
using UnityEngine.UI;
using TMPro;
//

[DisallowMultipleComponent]
public class MasterController : MonoBehaviourPunCallbacks, IDataPersistence
{
    [SerializeField] TapToMove AvatarScript;
    [SerializeField] ObjectSelection selector;
    [SerializeField] cinemachine_test CT;
    AssetMove AM;

    Touch touch;

    [SerializeField] Camera mainCam;

    bool tempbool;

    float MaxDoubleTapWaitTime = 0.5f;
    float newTime;
    int NumTap;
    int NumTapJump;

    int NumTapObstacle;

    int switchPOVCounter;

    Touch touches1;
    Touch touches2;

    Vector2 initTouchPos;

    float touchTime;

    [HideInInspector]
    public bool inEditMode;

    public bool Jumping;

    [SerializeField] public GameObject myAvatar;

    Coroutine _temp_Coroutine = null;
    bool coroutineStarted;

    private void Awake()
    {
        //CamControl = go1.GetComponent<CameraControl>();
        AM = GetComponent<AssetMove>();
        selector = GetComponent<ObjectSelection>();
        //CinemachineControl = transform.GetComponent<cinemachine_test>();
        CT = GetComponent<cinemachine_test>();
        SceneManager.sceneLoaded += SceneChanged;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneChanged;
    }

    public void SceneChanged(Scene scene, LoadSceneMode mode)
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("PlayerAvatar"))
        {
            if (go.GetComponent<PhotonView>().IsMine)
            {
                // Debug.Log("find avatar");
                myAvatar = go;
            }
            else continue;
        }
    }

    private void Start()
    {
        // GameObject go1 = GameObject.Find("ScriptManager");
        //   AvatarScript = GetComponent<TapToMove>();
        // GameObject[] avatarArray = GameObject.FindGameObjectsWithTag("PlayerAvatar");
        // foreach (GameObject go in avatarArray)
        // {
        //     if (!go.GetComponent<PhotonView>().IsMine)
        //     {
        //         continue;
        //     }

        //     else if (go.GetComponent<PhotonView>().IsMine)
        //     {
        //         myAvatar = go;
        //         AvatarScript = go.GetComponent<TapToMove>();
        //     }
        // }
        // StartCoroutine(FindMyAvatar());

        //// AvatarScript = GameObject.FindGameObjectWithTag("PlayerAvatar").GetComponent<TapToMove>();
        Application.targetFrameRate = 60;
        // AvatarScript.InitialiseTapToMove();
        // StartCoroutine(FindMyAvatar());
    }

    float deltaSum;

    bool objSelectionSingleRaycast = true;

    RaycastHit objSelectionRaycastHit;

    public IEnumerator FindMyAvatar()
    {
        while (true)
        {
            coroutineStarted = true;
            GameObject[] avatarArray = GameObject.FindGameObjectsWithTag("PlayerAvatar");
            if (avatarArray == null)
            {
                yield return new WaitForSeconds(0.1f);
                avatarArray = GameObject.FindGameObjectsWithTag("PlayerAvatar");
            }

            foreach (GameObject go in avatarArray)
            {
                go.GetComponent<PhotonTransformView>().enabled = false;
                if (go.GetComponent<PhotonView>().IsMine)
                {
                    myAvatar = go;
                    AvatarScript = go.GetComponent<TapToMove>();
                    AvatarScript.NMA.updateRotation = true;
                    // yield break;
                }

                else if (!go.GetComponent<PhotonView>().IsMine)
                {
                    continue;
                }
            }

            yield break;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            int random = Random.Range(0, 3);
            var temp = myAvatar.GetComponent<Animator>();
            temp.SetFloat("HeartThreshhold", random);
            temp.Play("HeartBlendTree");
        }

        if ((!coroutineStarted && _temp_Coroutine == null))
        {
            _temp_Coroutine = StartCoroutine(FindMyAvatar());
        }

        if (Input.touchCount == 1)
        {
            touch = Input.touches[0];
            AM.touch = touch;
        }

        if (Input.touchCount == 2)
        {
            touches1 = Input.GetTouch(0);
            touches2 = Input.GetTouch(1);
        }

        if (inEditMode && selector.GO != null)
        {
            selector.FindScreenSpace(selector.GO);
            //selector.moveHandler.transform.position = new Vector2((selector.minX + selector.maxX) / 2, selector.minY - 50);
        }
        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            AvatarScript.RotateAvatar();
        }

        switch (Input.touchCount)
        {
            case 0:
                if (initTouchPos != new Vector2(9999, 9999))
                {
                    initTouchPos = new Vector2(9999, 9999);
                }

                if (touchTime != 0)
                {
                    touchTime = 0;
                }

                deltaSum = 0;

                break;

            case 1:
                RaycastHit hit;

                if (touch.phase == TouchPhase.Began || initTouchPos == new Vector2(9999, 9999))
                {
                    initTouchPos = touch.position;
                    if (!AM.IsMoving)
                    {
                        AM.prevPos = touch.position;
                    }
                }

                touchTime += Time.deltaTime;
                deltaSum += touch.deltaPosition.magnitude;

                if ((initTouchPos - touch.position).magnitude < 30)
                {
                    objSelectionSingleRaycast = Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out objSelectionRaycastHit);

                    if (objSelectionSingleRaycast)
                    {
                        if (objSelectionRaycastHit.transform.tag == "Selectable" && !inEditMode)
                        {
                            selector.selectObject(objSelectionRaycastHit);
                        }

                        else if (objSelectionRaycastHit.transform.tag == "Walkable" && !inEditMode)
                        {
                            selector.selectFloor();
                        }

                        else if (objSelectionRaycastHit.transform.tag == "PlayerAvatar" && objSelectionRaycastHit.transform.GetComponent<PhotonView>().IsMine)
                        {
                            //todo - my avatar long press event
                            objSelectionRaycastHit.transform.GetComponent<TapToMove>().IsMineLongPressEvent(touchTime, CT);
                        }

                        else if (objSelectionRaycastHit.transform.tag == "PlayerAvatar" && !objSelectionRaycastHit.transform.GetComponent<PhotonView>().IsMine)
                        {
                            //todo - other's avatar long press event
                            objSelectionRaycastHit.transform.GetComponent<TapToMove>().NotMineLongPressEvent(touchTime);
                        }
                    }
                }

                if (touch.phase == TouchPhase.Ended && selector.floorLongPress)
                {
                    // AvatarScript.LongPressEventBool = false;
                    foreach (var avatars in GameObject.FindGameObjectsWithTag("PlayerAvatar"))
                    {
                        avatars.GetComponent<TapToMove>().LongPressEventBool = false;
                    }
                    AM.prevTouchPosIsNull = true;


                    CT.enableRotation = true;
                    if (deltaSum < 100)
                    {
                        bool raycast = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, layerMask: (1 << 6) | (1 << 7) | (1 << 3));
                        if (raycast)
                        {
                            if (inEditMode && hit.transform.tag == "Selectable")
                            {
                                selector.changeSelectedObject(hit);
                                CT.SetObjectAsMain(hit);
                            }

                            else if (inEditMode && (hit.transform.tag == "Selected" || GameObjectOverUI(touch)))
                            {
                                if (AM.IsMoving)
                                {
                                    AM.IsMoving = false;
                                }

                                else if (!AM.IsMoving)
                                {
                                    AM.IsMoving = true;
                                }
                            }
                            //
                            else if (!inEditMode && hit.transform.tag == "Walkable" && !isPointerOverUI(touch) && !CT.isCameraFirstPerson && CT.switchCams)
                            {
                                // PhotonView pView = GameObject.FindGameObjectWithTag("Myself").GetComponent<PhotonView>();
                                if (myAvatar == null)
                                {
                                    GameObject[] avatarArray = GameObject.FindGameObjectsWithTag("PlayerAvatar");
                                    foreach (GameObject go in avatarArray)
                                    {
                                        if (go.GetComponent<PhotonView>().IsMine)
                                        {
                                            myAvatar = go;
                                            AvatarScript = go.GetComponent<TapToMove>();
                                            AvatarScript.NMA.updateRotation = true;
                                            break;
                                        }

                                        else if (!go.GetComponent<PhotonView>().IsMine)
                                        {
                                            continue;
                                        }
                                    }
                                    if (myAvatar == null) break;
                                }
                                AvatarScript.StopRunningCoroutines();
                                PhotonView pView = myAvatar.GetComponent<PhotonView>();

                                NumTapJump++;

                                //   AvatarScript.Tap(hit);
                                // pView.RPC("Tap", RpcTarget.All, hit.point, hit.transform.tag);

                                CT.recenterCameraOnMove();

                                if (NumTapJump == 1)
                                {
                                    newTime = Time.time + MaxDoubleTapWaitTime;
                                    pView.RPC("Tap", RpcTarget.All, hit.point, hit.transform.tag);
                                }

                                else if (NumTapJump == 2 && Time.time <= newTime && !Jumping)
                                {
                                    // pView.RPC("UpdateParabolaStart", RpcTarget.All, hit.point);

                                    pView.RPC("DoubleTap", RpcTarget.All, hit.point);

                                    // AvatarScript.DoubleTap(hit);
                                    NumTapJump = 0;
                                }
                            }

                            else if (hit.transform.tag == "PlayerAvatar" && !Jumping && hit.transform.GetComponent<PhotonView>().IsMine)
                            {
                                //todo - long press / tap / double tap event
                                NumTap++;

                                // var hitTTM = hit.transform.GetComponent<TapToMove>();

                                if (NumTap == 1)
                                {
                                    newTime = Time.time + MaxDoubleTapWaitTime;

                                    //! single tap event
                                    float dotProduct = Vector3.Dot(hit.transform.forward, Camera.main.transform.forward);
                                    if (dotProduct >= -1 && dotProduct <= 0.4f)
                                    {
                                        AvatarScript.EnableIKLook = true;
                                        AvatarScript.PhotonIKPosition = Camera.main.transform.position;
                                        // hitTTM.EnableIKLook = true;
                                    }

                                    else
                                    {
                                        AvatarScript.StopRunningCoroutines();
                                        // hitTTM.StopRunningCoroutines();
                                        //todo - surprised animation
                                        hit.transform.GetComponent<PhotonView>().RPC("SurpriseAvatar", RpcTarget.All);
                                    }

                                }

                                else if (NumTap == 2 && Time.time <= newTime)
                                {
                                    AvatarScript.StopRunningCoroutines();
                                    // hitTTM.StopRunningCoroutines();
                                    // Debug.Log("double tap");
                                    //todo - cancel single tap event
                                    //! cancel single tap event


                                    //todo - jump at current position
                                    //! double tap event
                                    // hit.transform.GetComponent<PhotonView>().RPC("DoubleTapSelf", RpcTarget.All);

                                    //todo - change switching to 1st person to long press instead of double tap
                                    // CT.changeToFirstPerson();
                                    NumTap = 0;
                                }

                                //switchPOVCounter++;

                                //Debug.Log(switchPOVCounter);

                                //if (switchPOVCounter % 2 == 0)
                                //{
                                //    CT.changeToFirstPerson();
                                //}
                            }

                            else if (hit.transform.tag == "PlayerAvatar" && !Jumping && !hit.transform.GetComponent<PhotonView>().IsMine)
                            {
                                //todo - other's avatar tap

                                float dotProduct = Vector3.Dot((hit.transform.position - myAvatar.transform.position).normalized, myAvatar.transform.forward);

                                if (dotProduct <= 1 && dotProduct >= 0.3f)
                                {
                                    AvatarScript.EnableIKLook = true;
                                    AvatarScript.PhotonIKPosition = hit.transform.position + new Vector3(0, 1.35f, 0);
                                }
                            }
                        }

                        else
                        {
                            Debug.Log("null raycast");
                            myAvatar.GetComponent<PhotonView>().RPC("IdleAnimation", RpcTarget.All);
                        }
                    }

                    if (selector.GO != null)
                    {
                        selector.ChangeAssetMoveBoolean();
                    }

                    objSelectionSingleRaycast = true;
                }

                if ((initTouchPos - touch.position).magnitude >= 10)
                {
                    //if (!whileDragging)
                    //{
                    //    //CamControl.SingleFingerRotate(touch);
                    //}
                }
                break;

            case 2:
                //if (!whileDragging)
                //{
                //    //CamControl.DoubleFinger(touches1, touches2);
                //}
                break;
        }
    }

    private void LateUpdate()
    {
        if (Time.time > newTime)
        {
            NumTap = 0;
            NumTapJump = 0;
        }

        //if (AvatarScript.checkArrival)
        //{
        AvatarScript.ArriveAtDestination();
        //}
    }

    bool isPointerOverUI(Touch touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);

        eventData.position = new Vector2(touch.position.x, touch.position.y);

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    bool GameObjectOverUI(Touch touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);

        eventData.position = new Vector2(touch.position.x, touch.position.y);

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, results);

        foreach (var temp in results)
        {
            if (temp.gameObject.tag == "TranslationUI")
            {
                return true;
            }
        }
        return false;
    }

    bool ExistsSaveFile = true;

    public void LoadFromJson(GameData data)
    {

    }

    public void SaveToJson(ref GameData data)
    {
        data.ExistsSaveFile = this.ExistsSaveFile;
    }
}