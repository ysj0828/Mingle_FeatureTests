using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Photon.Pun;
using MingleCamera;
using com.unity.photon;

public class screenregionload : MonoBehaviour
{
    [SerializeField] GameObject[] avatarPrefabs;

    cinemachine_test CT;
    MasterController MC;
    TapToMove TTM;



    private void Start()
    {
        // buttonToggle.onValueChanged.AddListener(delegate
        // {
        //   ToggleValueChanged(buttonToggle);
        // });

        CT = GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<cinemachine_test>();
        MC = GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<MasterController>();
        // // TTM = GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<TapToMove>();

        // foreach (GameObject go in GameObject.FindGameObjectsWithTag("PlayerAvatar"))
        // {
        //   go.transform.position = FindObjectOfType<Infomation>().InitialPosition[SceneManager.GetActiveScene().name] + new Vector3(Random.Range(-3, -3), 0, Random.Range(-3, -3));
        // }
    }


    public void LoadScene(string message)
    {
        RNMessenger.SendToRN("LoadScene");
        RNMessenger.SendToRN(message);

        JObject json = JObject.Parse(message);

        GameObject.FindGameObjectWithTag("PlayerAvatar").GetComponent<PhotonView>().RPC("ShowLoadingScreen", RpcTarget.All);

        if (PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel(json["room_uuid"].ToString());

        else GameObject.FindGameObjectWithTag("PlayerAvatar").GetComponent<PhotonView>().RPC("ChangeSceneRPCToMasterClient", RpcTarget.MasterClient, json["room_uuid"].ToString());
    }

    public void LoadCharacter(string message)
    {
        RNMessenger.SendToRN("LoadCharacter");
        RNMessenger.SendToRN(message);

        JObject json = JObject.Parse(message);

        FindPositionAndRotation();

        var temp = PhotonNetwork.Instantiate(json["character_uuid"].ToString(), InstantiatePosition, InstantiateRotation);
        GameManager.PrefabIndex = 1;
        GameManager.MyAvatar = temp;

        CT.refTarget = temp.transform;
        CT.SetAvatarAsMain();
        // TTM.ava = temp;
        StartCoroutine(MC.FindMyAvatar());

    }

    Vector3 InstantiatePosition;
    Quaternion InstantiateRotation;

    void FindPositionAndRotation()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("PlayerAvatar"))
        {
            if (go.GetComponent<PhotonView>().IsMine)
            {
                InstantiatePosition = go.transform.position;
                InstantiateRotation = go.transform.rotation;
                PhotonNetwork.Destroy(go);
                break;
            }
            else continue;
        }
    }
}
