using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Photon.Pun;

//
using UnityEngine.Networking;
//

public class DataPersistenceManager : MonoBehaviour
{
    [SerializeField] string fileName;
    GameData gameData = null;
    List<IDataPersistence> dataPersistenceObjects;

    FileDataHandler handler;

    public static DataPersistenceManager instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than 1 Data Persistence Manager");
        }
        instance = this;
    }

    private void Start()
    {
        //Application.persistentDataPath에 저장
        //플랫폼 별 저장경로는 구글 챗 확인
        this.handler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadSavedData();
    }

    List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }


    //저장
    private void OnApplicationQuit()
    {
        if (PhotonNetwork.PlayerList.Length <= 1)
        {
            SaveData();
        }
    }

    public void NewData()
    {
        this.gameData = new GameData();
    }


    public IEnumerator GetFromDrive(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            //error
            Debug.Log("network error");
        }

        else
        {
            //loaded
            gameData = JsonUtility.FromJson<GameData>(request.downloadHandler.text);

            if (gameData != null)
            {
                Debug.Log("not null");
            }
            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            {
                dataPersistenceObj.LoadFromJson(gameData);
            }
        }
    }


    public void LoadSavedData()
    {
        this.gameData = handler.Load();
        //   StartCoroutine(GetFromDrive("https://drive.google.com/uc?export=download&id=1qU6-fw5ZPSQzrCGlSPkvVLeNgY8E3tR5"));

        if (this.gameData == null)
        {
            NewData();
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadFromJson(gameData);
        }
    }

    public void SaveData()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveToJson(ref gameData);
        }

        handler.Save(gameData);
    }
}
