using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;

public class FileDataHandler
{
    string dataDirPath = "";

    string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        string JsonDataFromServer = "";


        //   string fullPath = "https://drive.google.com/file/d/1qU6-fw5ZPSQzrCGlSPkvVLeNgY8E3tR5/view?usp=sharing";
        //   string fullPath = "https://drive.google.com/uc?export=download&id=1qU6-fw5ZPSQzrCGlSPkvVLeNgY8E3tR5";
        GameData loadedData = null;
        //   if (File.Exists(fullPath))
        //   {
        //       try
        //       {
        //           string dataToLoad = "";

        //           using (FileStream stream = new FileStream(fullPath, FileMode.Open))
        //           {
        //               using (StreamReader reader = new StreamReader(stream))
        //               {
        //                   dataToLoad = reader.ReadToEnd();
        //               }
        //           }

        //           loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
        //       }

        //       catch (Exception e)
        //       {
        //           Debug.LogError("Error when loading" + fullPath + "\n" + e);
        //       }
        //   }
        //   UnityWebRequest request = UnityWebRequest.Get(fullPath);
        //   request.Send();
        //   if (request.result == UnityWebRequest.Result.ConnectionError)
        //   {
        //       Debug.Log("error when loading from url");
        //   }

        //   else
        //   {



        //JsonDataFromServer를 서버에서 받아온 Json으로 넣어주면 끝
        loadedData = JsonUtility.FromJson<GameData>(JsonDataFromServer);

        //   }

        return loadedData;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            //dataToStore : 저장해야 할 에셋 <GUID, Position>, <GUID, Quaternion> Dictionary 값






            // using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            // {
            //     using (StreamWriter writer = new StreamWriter(stream))
            //     {
            //         writer.Write(dataToStore);
            //     }
            // }
        }

        catch (Exception e)
        {
            Debug.LogError("Error when saving" + fullPath + "\n" + e);
        }
    }
}
