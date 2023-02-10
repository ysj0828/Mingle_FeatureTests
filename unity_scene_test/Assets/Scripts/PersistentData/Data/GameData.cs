using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



[Serializable]
public class GameData
{
    //     public string AssetID;
    //     public CustomDictionary<string, AssetInfo> AssetTransform;

    public CustomDictionary<string, Vector3> AssetPosition;
    public CustomDictionary<string, Quaternion> AssetRotation;


    public bool ExistsSaveFile;


    //     public class AssetInfo
    //     {
    //         public AssetInfo(Vector3 p, Quaternion r)
    //         {
    //             position = p;
    //             rotation = r;
    //         }

    //         public Vector3 position;
    //         public Quaternion rotation;
    //     }


    //Initial values if there is no saved data
    //저장된 데이터가 없을 때 로드되는 값
    public GameData()
    {
        ExistsSaveFile = false;
        AssetPosition = new CustomDictionary<string, Vector3>();
        AssetRotation = new CustomDictionary<string, Quaternion>();
        //   AssetTransform = new CustomDictionary<string, AssetInfo>();
    }
}