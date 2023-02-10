using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using com.unity.photon;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MingleCamera
{
    [DisallowMultipleComponent]
    public class ObjectSelection : MonoBehaviourPunCallbacks
    {
        internal Dictionary<int, int> PlayerLeaveCheck = new Dictionary<int, int>();

        // static ObjectSelection instance = null;

        public GameObject GO;
        MasterController MC;
        AssetMove AM;
        cinemachine_test CT;
        RotationUI RUI;
        TranslationUI TUI;


        Bounds GOBounds;

        Vector3[] corners;

        GameObject tempGO;

        //public Canvas editPopUpCanvas;
        public Slider holdIndicatorSlider;
        public Button exitButton;

        [SerializeField] Canvas dynamicCanvas;
        [SerializeField] GameObject rotationAsset;

        public bool floorLongPress = true;


        [HideInInspector] public float minX, minY, maxX, maxY;

        //bool handlerActive;
        int selectedLayer = 9;
        int movableLayer = 7;

        private void Awake()
        {
            MC = GetComponent<MasterController>();
            AM = GetComponent<AssetMove>();
            CT = GetComponent<cinemachine_test>();
            RUI = rotationAsset.GetComponent<RotationUI>();
            TUI = dynamicCanvas.GetComponentInChildren<TranslationUI>();

            holdIndicatorSlider.gameObject.SetActive(false);
        }

        private void Start()
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
            dynamicCanvas.gameObject.SetActive(false);
            rotationAsset.SetActive(false);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            int otherPlayerActorNumber = otherPlayer.ActorNumber;

            if (PlayerLeaveCheck.ContainsKey(otherPlayerActorNumber))
            {
                PhotonView pView = PhotonView.Find(PlayerLeaveCheck[otherPlayerActorNumber]);
                pView.RPC("ObjectDeselectionTagChange", RpcTarget.AllBuffered, pView.ViewID);
                pView.RPC("DisableWrench", RpcTarget.AllBuffered);
                pView.RPC("EnableComponent", RpcTarget.AllBuffered, pView.ViewID);

                PlayerLeaveCheck.Remove(otherPlayer.ActorNumber);
            }

            // if (otherPlayer.CustomProperties.ContainsKey(otherPlayerActorNumber))
            // {
            //       PhotonView pView = PhotonView.Find((int)otherPlayer.CustomProperties[otherPlayerActorNumber]).gameObject.GetComponent<PhotonView>();

            //       pView.RPC("ObjectDeselectionTagChange", RpcTarget.AllBuffered, pView.ViewID);
            //       pView.RPC("DisableWrench", RpcTarget.Others);
            //       pView.RPC("EnableComponent", RpcTarget.AllBuffered, pView.ViewID);
            // }

            //   Debug.Log("temp player custom property" + tempPlayer.CustomProperties);

            //   if (tempPlayer.CustomProperties[tempPlayer.ActorNumber] == null)
            //   {
            //         Debug.Log("null property");
            //   }

            //   Debug.Log(otherPlayer.ActorNumber);
            //   Debug.Log(PlayerLeaveCheck);

            //   if (PlayerLeaveCheck.ContainsKey(otherPlayer.ActorNumber))
            //   {
            //         PhotonView pView = PhotonView.Find(PlayerLeaveCheck[otherPlayer.ActorNumber]).gameObject.GetComponent<PhotonView>();

            //         pView.RPC("ObjectDeselectionTagChange", RpcTarget.AllBuffered, pView.ViewID);
            //         pView.RPC("DisableWrench", RpcTarget.Others);
            //         pView.RPC("EnableComponent", RpcTarget.AllBuffered, pView.ViewID);
            //   }
        }

        float touchTime;


        private void FixedUpdate()
        {
            // if (Input.GetKeyDown(KeyCode.Space))
            // {
            //     foreach (KeyValuePair<int, int> kvp in PlayerLeaveCheck)
            //     {
            //         Debug.LogFormat("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            //     }
            // }

            // if (Input.GetKeyDown(KeyCode.T))
            // {
            //     GameObject[] temp = GameObject.FindGameObjectsWithTag("PlayerAvatar");
            //     int i = 0;
            //     foreach (Player p in PhotonNetwork.PlayerList)
            //     {
            //         PhotonView pv = temp[i].GetComponent<PhotonView>();
            //         // Debug.Log("actor num : " + p.ActorNumber + "dict value" + p.CustomProperties[p.ActorNumber]);
            //         Debug.Log("actor num : " + p.ActorNumber + "dict value : " + p.CustomProperties[p.ActorNumber.ToString()]);
            //         // Debug.Log()
            //         i++;
            //     }
            // }

            if (GO != null)
            {
                FindScreenSpace(GO);
            }

            if (Input.touchCount != 0)
            {
                touchTime += Time.deltaTime;
            }

            if (Input.touchCount == 0 && touchTime != 0)
            {
                if (touchTime != 0)
                {
                    touchTime = 0;
                }

                if (holdIndicatorSlider.IsActive())
                {
                    holdIndicatorSlider.gameObject.SetActive(false);
                }

                if (tempGO != null)
                {
                    tempGO = null;
                }

                if (!floorLongPress) floorLongPress = true;
            }

            if (holdIndicatorSlider.IsActive())
            {
                FindScreenSpace(tempGO);
                holdIndicatorSlider.transform.position = new Vector2((minX + maxX) / 2, maxY + 50);
            }
        }



        public void selectFloor()
        {
            if (touchTime >= 0.3f && touchTime < 0.6f)
            {
                return;
            }

            else if (touchTime >= 0.6f && floorLongPress)
            {
                //! Floor long press event
                floorLongPress = false;

                // Debug.Log("floor long press");

                var json = new JObject();

                json["cmd"] = "OnLongPressEmptySpace";

                RNMessenger.SendToRN(JsonConvert.SerializeObject(json, Formatting.Indented));
            }
        }

        public void selectObject(RaycastHit hit)
        {
            if (touchTime >= 0.3f)
            {
                holdIndicatorSlider.gameObject.SetActive(true);
                tempGO = hit.transform.gameObject;

                FindScreenSpace(tempGO);
                tempGO = hit.transform.gameObject;

                holdIndicatorSlider.transform.position = new Vector2((minX + maxX) / 2, maxY + 50);
                holdIndicatorSlider.value = touchTime - 0.3f;
            }

            if (touchTime >= 0.6f)
            {
                //GO = hit;
                GO = hit.transform.gameObject;
                //GO.tag = "Selected";

                PhotonView pView = GO.GetComponent<PhotonView>();

                ExitGames.Client.Photon.Hashtable assetEditHashtable = new ExitGames.Client.Photon.Hashtable();
                assetEditHashtable[PhotonNetwork.LocalPlayer.ActorNumber.ToString()] = pView.ViewID;
                PhotonNetwork.LocalPlayer.SetCustomProperties(assetEditHashtable);
                // PhotonNetwork.LocalPlayer.CustomProperties = assetEditHashtable;

                GO.layer = selectedLayer;
                GO.GetComponent<Outline>().enabled = true;
                GO.GetComponent<PhotonTagChangerScript>().selectedObject = GO;
                pView.RPC("ObjectSelectionTagChange", RpcTarget.AllBuffered, pView.ViewID);
                //DisableComponent(GO);
                if (GO.name != "ColOne" || GO.name != "ColTwo") pView.RPC("DisableComponent", RpcTarget.AllBuffered, pView.ViewID);
                pView.RPC("EnableWrench", RpcTarget.OthersBuffered, pView.ViewID);
                pView.TransferOwnership(PhotonNetwork.LocalPlayer);

                holdIndicatorSlider.value = 0;
                holdIndicatorSlider.gameObject.SetActive(false);

                FindScreenSpace(GO);

                dynamicCanvas.gameObject.SetActive(true);
                rotationAsset.SetActive(true);

                exitButton.gameObject.SetActive(true);

                MC.inEditMode = true;

                AM.IsMoving = true;
                AM.selectedObj = GO;
                //AM.isMoving = true;

                GO.AddComponent<CollisionDetection>();



                CT.inEditMode = true;

                AM.CD = GO.GetComponent<CollisionDetection>();
                AM.initialY = GO.transform.position.y;

                RUI.Target = GO.transform;
                // RUI.CT = CT;
                // RUI.AM = AM;
                TUI.Target = GO.transform;
                TUI.CT = CT;
                // TUI.AM = AM;

                CT.SetObjectAsMain(hit);
            }
        }



        public void SelectObstacle(RaycastHit hit)
        {
            //GO = hit;
            GO = hit.transform.gameObject;
            //GO.tag = "Selected";

            PhotonView pView = GO.GetComponent<PhotonView>();

            ExitGames.Client.Photon.Hashtable assetEditHashtable = new ExitGames.Client.Photon.Hashtable();
            assetEditHashtable[PhotonNetwork.LocalPlayer.ActorNumber.ToString()] = pView.ViewID;
            PhotonNetwork.LocalPlayer.SetCustomProperties(assetEditHashtable);
            // PhotonNetwork.LocalPlayer.CustomProperties = assetEditHashtable;

            GO.layer = selectedLayer;
            GO.GetComponent<Outline>().enabled = true;
            GO.GetComponent<PhotonTagChangerScript>().selectedObject = GO;
            pView.RPC("ObjectSelectionTagChange", RpcTarget.AllBuffered, pView.ViewID);
            //DisableComponent(GO);
            if (GO.name != "ColOne" || GO.name != "ColTwo") pView.RPC("DisableComponent", RpcTarget.AllBuffered, pView.ViewID);
            pView.RPC("EnableWrench", RpcTarget.OthersBuffered, pView.ViewID);
            pView.TransferOwnership(PhotonNetwork.LocalPlayer);

            FindScreenSpace(GO);

            dynamicCanvas.gameObject.SetActive(true);
            rotationAsset.SetActive(true);

            exitButton.gameObject.SetActive(true);

            MC.inEditMode = true;

            AM.IsMoving = true;
            AM.selectedObj = GO;
            //AM.isMoving = true;

            GO.AddComponent<CollisionDetection>();



            CT.inEditMode = true;

            AM.CD = GO.GetComponent<CollisionDetection>();
            AM.initialY = GO.transform.position.y;

            RUI.Target = GO.transform;
            // RUI.CT = CT;
            // RUI.AM = AM;
            TUI.Target = GO.transform;
            TUI.CT = CT;
            // TUI.AM = AM;

            CT.SetObjectAsMain(hit);

        }







        //change selected object
        public void changeSelectedObject(RaycastHit hit)
        {
            PhotonView pView = GO.GetComponent<PhotonView>();
            // ExitGames.Client.Photon.Hashtable assetEditHashtable = new ExitGames.Client.Photon.Hashtable();
            // assetEditHashtable.Remove(PhotonNetwork.LocalPlayer.ActorNumber);
            // PhotonNetwork.LocalPlayer.CustomProperties.Remove(PhotonNetwork.LocalPlayer.ActorNumber.ToString());
            PhotonNetwork.SetPlayerCustomProperties(null);

            GO.GetComponent<Outline>().enabled = false;
            GO.GetComponent<PhotonTagChangerScript>().selectedObject = null;
            //EnableComponent(GO);
            pView.RPC("EnableComponent", RpcTarget.AllBuffered, pView.ViewID);



            GO.layer = movableLayer;
            //GO.tag = "Selectable";
            GO.GetComponent<PhotonView>().RPC("ObjectDeselectionTagChange", RpcTarget.AllBuffered, pView.ViewID);
            Destroy(GO.GetComponent<CollisionDetection>());



            GO = hit.transform.gameObject;

            PhotonView NewpView = GO.GetComponent<PhotonView>();
            ExitGames.Client.Photon.Hashtable newHashtable = new ExitGames.Client.Photon.Hashtable();
            newHashtable[PhotonNetwork.LocalPlayer.ActorNumber.ToString()] = NewpView.ViewID;
            // PhotonNetwork.LocalPlayer.CustomProperties = assetEditHashtable;
            PhotonNetwork.LocalPlayer.SetCustomProperties(newHashtable);

            GO.AddComponent<CollisionDetection>();
            FindScreenSpace(GO);
            GO.GetComponent<Outline>().enabled = true;
            GO.GetComponent<PhotonTagChangerScript>().selectedObject = GO;
            //GO.GetComponent<NavMeshObstacle>().enabled = false;
            if (GO.name != "ColOne" || GO.name != "ColTwo") NewpView.RPC("DisableComponent", RpcTarget.AllBuffered, NewpView.ViewID);
            NewpView.RPC("EnableWrench", RpcTarget.OthersBuffered, NewpView.ViewID);
            NewpView.TransferOwnership(PhotonNetwork.LocalPlayer);
            // GO.layer = selectedLayer;
            //GO.tag = "Selected";

            NewpView.RPC("ObjectSelectionTagChange", RpcTarget.AllBuffered, NewpView.ViewID);
            AM.IsMoving = true;
            AM.selectedObj = GO;
            AM.CD = GO.GetComponent<CollisionDetection>();
            AM.initialY = GO.transform.position.y;
            RUI.Target = GO.transform;
            // RUI.CT = CT;
            // RUI.AM = AM;
            TUI.Target = GO.transform;
            TUI.CT = CT;
            // TUI.AM = AM;

            CT.SetObjectAsMain(hit);
        }

        //reset all
        public void ExitEditMode()
        {
            PhotonView pView = GO.GetComponent<PhotonView>();
            // PhotonNetwork.LocalPlayer.CustomProperties.Remove(PhotonNetwork.LocalPlayer.ActorNumber.ToString());
            PhotonNetwork.SetPlayerCustomProperties(null);
            //GO.tag = "Selectable";
            pView.RPC("ObjectDeselectionTagChange", RpcTarget.AllBuffered, pView.ViewID);
            pView.RPC("DisableWrench", RpcTarget.OthersBuffered);
            // GO.layer = movableLayer;
            GO.GetComponent<Outline>().enabled = false;
            GO.GetComponent<PhotonTagChangerScript>().selectedObject = null;
            if (GO.name != "ColOne" || GO.name != "ColTwo") pView.RPC("EnableComponent", RpcTarget.AllBuffered, pView.ViewID);
            //GO.GetComponent<NavMeshObstacle>().enabled = true;



            GO = null;

            corners = null;

            minX = 0;
            maxX = 0;
            minY = 0;
            maxY = 0;

            exitButton.gameObject.SetActive(false);

            MC.inEditMode = false;
            AM.enableAssetMove = false;

            dynamicCanvas.gameObject.SetActive(false);
            rotationAsset.SetActive(false);

            CT.SetAvatarAsMain();
        }

        public void ChangeAssetMoveBoolean()
        {
            AM.enableAssetMove = true;
        }

        public void FindScreenSpace(GameObject paramGO)
        {

            if (paramGO.GetComponent<Renderer>() != null) GOBounds = paramGO.GetComponent<Renderer>().bounds;
            else GOBounds = paramGO.transform.GetChild(0).GetComponent<Renderer>().bounds;

            corners = new Vector3[8];

            corners[0] = Camera.main.WorldToScreenPoint(new Vector3(GOBounds.center.x + GOBounds.extents.x, GOBounds.center.y + GOBounds.extents.y, GOBounds.center.z + GOBounds.extents.z));
            corners[1] = Camera.main.WorldToScreenPoint(new Vector3(GOBounds.center.x + GOBounds.extents.x, GOBounds.center.y + GOBounds.extents.y, GOBounds.center.z - GOBounds.extents.z));
            corners[2] = Camera.main.WorldToScreenPoint(new Vector3(GOBounds.center.x + GOBounds.extents.x, GOBounds.center.y - GOBounds.extents.y, GOBounds.center.z + GOBounds.extents.z));
            corners[3] = Camera.main.WorldToScreenPoint(new Vector3(GOBounds.center.x + GOBounds.extents.x, GOBounds.center.y - GOBounds.extents.y, GOBounds.center.z - GOBounds.extents.z));

            corners[4] = Camera.main.WorldToScreenPoint(new Vector3(GOBounds.center.x - GOBounds.extents.x, GOBounds.center.y + GOBounds.extents.y, GOBounds.center.z + GOBounds.extents.z));
            corners[5] = Camera.main.WorldToScreenPoint(new Vector3(GOBounds.center.x - GOBounds.extents.x, GOBounds.center.y + GOBounds.extents.y, GOBounds.center.z - GOBounds.extents.z));
            corners[6] = Camera.main.WorldToScreenPoint(new Vector3(GOBounds.center.x - GOBounds.extents.x, GOBounds.center.y - GOBounds.extents.y, GOBounds.center.z + GOBounds.extents.z));
            corners[7] = Camera.main.WorldToScreenPoint(new Vector3(GOBounds.center.x - GOBounds.extents.x, GOBounds.center.y - GOBounds.extents.y, GOBounds.center.z - GOBounds.extents.z));

            minX = corners[0].x;
            minY = corners[0].y;
            maxX = corners[0].x;
            maxY = corners[0].y;

            for (int i = 1; i < 8; i++)
            {
                if (corners[i].x < minX)
                {
                    minX = corners[i].x;
                }
                if (corners[i].y < minY)
                {
                    minY = corners[i].y;
                }
                if (corners[i].x > maxX)
                {
                    maxX = corners[i].x;
                }
                if (corners[i].y > maxY)
                {
                    maxY = corners[i].y;
                }
            }
        }
    }
}