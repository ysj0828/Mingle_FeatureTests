using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using System;

namespace MingleCamera
{
    public class PhotonTagChangerScript : MonoBehaviourPunCallbacks, IDataPersistence
    {
        private void Start()
        {
            //load




            // if (UniqueGUID == null || UniqueGUID == "")
            // {
            //     UniqueGUID = Guid.NewGuid().ToString();
            // }
        }

        [SerializeField] string UniqueGUID;

        [ContextMenu("Generate GUID")]
        void GenerateGUID()
        {
            UniqueGUID = Guid.NewGuid().ToString();
        }

        public GameObject selectedObject
        {
            get
            {
                return so;
            }

            set
            {
                so = value;
            }
        }

        [SerializeField] GameObject so;

        // [SerializeField] int viewID;

        [SerializeField] GameObject wrench;
        [SerializeField] RotateWrench RW;

        // private void Start()
        // {
        //     OS = ScriptManager.GetComponent<ObjectSelection>();
        // }

        [PunRPC]
        void ObjectSelectionTagChange(int PVid)
        {
            GameObject temp = PhotonView.Find(PVid).gameObject;

            temp.tag = "Selected";
            temp.layer = 9;
            selectedObject = temp;
            //   viewID = PVid;

            var list = PhotonNetwork.PlayerList;

            for (int i = 0; i < list.Length; i++)
            {
                list = PhotonNetwork.PlayerList;

                if (list[i].CustomProperties[list[i].ActorNumber.ToString()] != null && !GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<ObjectSelection>().PlayerLeaveCheck.ContainsKey(list[i].ActorNumber))
                {
                    GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<ObjectSelection>().PlayerLeaveCheck.Add(list[i].ActorNumber, PVid);
                }

                else
                {
                    continue;
                }
            }
        }

        [PunRPC]
        void ObjectDeselectionTagChange(int PVid)
        {
            GameObject temp = PhotonView.Find(PVid).gameObject;
            temp.tag = "Selectable";
            temp.layer = 7;

            var list = PhotonNetwork.PlayerList;

            for (int i = 0; i < list.Length; i++)
            {
                list = PhotonNetwork.PlayerList;

                if (list[i].CustomProperties[list[i].ActorNumber.ToString()] == null && GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<ObjectSelection>().PlayerLeaveCheck.ContainsKey(list[i].ActorNumber))
                {
                    GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<ObjectSelection>().PlayerLeaveCheck.Remove(list[i].ActorNumber);
                }

                else
                {
                    continue;
                }
            }
        }

        [PunRPC]
        void DisableComponent(int PVid)
        {
            GameObject GO = PhotonView.Find(PVid).gameObject;
            if (GO.name == "ColOne" || GO.name == "ColTwo") return;
            else GO.GetComponent<NavMeshObstacle>().enabled = false;
        }

        [PunRPC]
        void EnableComponent(int PVid)
        {
            GameObject GO = PhotonView.Find(PVid).gameObject;
            if (GO.name == "ColOne" || GO.name == "ColTwo") return;
            else GO.GetComponent<NavMeshObstacle>().enabled = true;
        }



        [PunRPC]
        void EnableWrench(int PVid)
        {
            wrench.SetActive(true);
            GameObject GO = PhotonView.Find(PVid).gameObject;
            RW.Target = GO.transform;
        }

        [PunRPC]
        void DisableWrench()
        {
            wrench.SetActive(false);
        }

        Vector3 tempV;
        Quaternion tempQ;

        public void LoadFromJson(GameData data)
        {
            // this.UniqueGUID = data.AssetID;
            // this.transform.position = data.AssetTransform[this.UniqueGUID].position;
            // this.transform.rotation = data.AssetTransform[this.UniqueGUID].rotation;
            if (data.ExistsSaveFile)
            {
                data.AssetPosition.TryGetValue(UniqueGUID, out tempV);
                data.AssetRotation.TryGetValue(UniqueGUID, out tempQ);

                this.transform.position = tempV;
                this.transform.rotation = tempQ;
            }
        }

        public void SaveToJson(ref GameData data)
        {
            if (data.AssetPosition.ContainsKey(UniqueGUID))
            {
                data.AssetPosition.Remove(UniqueGUID);
            }

            if (data.AssetRotation.ContainsKey(UniqueGUID))
            {
                data.AssetRotation.Remove(UniqueGUID);
            }

            data.AssetPosition.Add(UniqueGUID, transform.position);
            data.AssetRotation.Add(UniqueGUID, transform.rotation);
        }
    }
}