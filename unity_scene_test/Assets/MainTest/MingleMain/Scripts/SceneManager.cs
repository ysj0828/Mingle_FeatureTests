using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using com.unity.photon;

//
using UnityEngine.UI;
//

namespace MingleMain
{
  public class SceneManager : MonoBehaviour
  {
    [SerializeField] Slider laodSlider;
    [SerializeField] GameObject panel;

    bool _isLoad_Main = false;
    bool _isLoad_SelectCharacter = false;
    bool _isLoad_SelectRoom = false;
    bool _isLoad_Profil = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeScene(string message)
    {
      RNMessenger.SendToRN("ChangeScene " + message);
      JObject json = JObject.Parse(message);
      RNMessenger.SendToRN("ChangeScene " + json["scene"].ToString());
      switch (json["scene"].ToString())
      {
        // case "Menu":
        //   UnLoadLoadMainScene();
        //   UnLoadRoomeScene();
        //   UnLoadPrefileScene();
        // case "Main":
        //   // LoadMainScene();
        //   break;
        case "SelectCharacter":
          // LoadCharacterScene();
          StartCoroutine(LoadCharacterScene());
          break;
        case "SelectRoom":
        case "SelectMyRoom":
          // LoadRoomeScene();
          StartCoroutine(LoadRoomeScene());
          break;
        case "Profile":
          // LoadPrefileScene();
          StartCoroutine(LoadPrefileScene());
          break;
        default:
          UnLoadLoadMainScene();
          UnLoadRoomeScene();
          UnLoadPrefileScene();
          break;
      }
    }



    public IEnumerator LoadMainScene()
    {
      if (_isLoad_Main) yield break;
      _isLoad_Main = true;

      panel.SetActive(true);

      AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MingleMain", UnityEngine.SceneManagement.LoadSceneMode.Additive);

      while (!operation.isDone)
      {
        laodSlider.value = operation.progress;
        yield return null;
      }

      if (operation.isDone)
      {
        UnLoadCharacterScene();
        UnLoadRoomeScene();
        UnLoadPrefileScene();
        yield return new WaitForSeconds(2f);
        panel.SetActive(false);
        yield break;
      }

      // UnLoadCharacterScene();
      // UnLoadRoomeScene();
      // UnLoadPrefileScene();
      // UnityEngine.SceneManagement.SceneManager.LoadScene("MingleCharacter", UnityEngine.SceneManagement.LoadSceneMode.Additive);
      // UnityEngine.SceneManagement.SceneManager.LoadScene("MingleMain");
    }

    public void UnLoadLoadMainScene()
    {
      // if (!_isLoad_Main) return;
      // _isLoad_Main = false;
      // UnityEngine.SceneManagement.SceneManager.UnloadScene("MingleMain");
    }
    public IEnumerator LoadCharacterScene()
    {
      if (_isLoad_SelectCharacter) yield break;
      _isLoad_SelectCharacter = true;

      panel.SetActive(true);
      AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MingleCharacter", UnityEngine.SceneManagement.LoadSceneMode.Additive);


      while (!operation.isDone)
      {
        laodSlider.value = operation.progress;
        yield return null;
      }

      if (operation.isDone)
      {
        UnLoadLoadMainScene();
        UnLoadRoomeScene();
        UnLoadPrefileScene();
        yield return new WaitForSeconds(2f);
        panel.SetActive(false);
        yield break;
      }

      // StartCoroutine(test("MingleCharacter"));
      // UnityEngine.SceneManagement.SceneManager.LoadScene("MingleCharacter", UnityEngine.SceneManagement.LoadSceneMode.Additive);
      // UnLoadLoadMainScene();
      // UnLoadRoomeScene();
      // UnLoadPrefileScene();
      // UnityEngine.SceneManagement.SceneManager.LoadScene("MingleCharacter");
    }

    public void UnLoadCharacterScene()
    {
      if (!_isLoad_SelectCharacter) return;
      _isLoad_SelectCharacter = false;
      UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("MingleCharacter");
    }


    public void VoidLoadRoomScene()
    {
      StartCoroutine(LoadRoomeScene());
    }

    public IEnumerator LoadRoomeScene()
    {
      if (_isLoad_SelectRoom) yield break;

      panel.SetActive(true);
      AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MingleRoom", UnityEngine.SceneManagement.LoadSceneMode.Additive);
      _isLoad_SelectRoom = true;
      // StartCoroutine(test("MingleRoom"));

      while (!operation.isDone)
      {
        laodSlider.value = operation.progress;
        yield return null;
      }

      if (operation.isDone)
      {
        UnLoadLoadMainScene();
        UnLoadCharacterScene();
        UnLoadPrefileScene();
        // yield return new WaitForSeconds(2f);
        panel.SetActive(false);
        yield break;
      }

      // UnityEngine.SceneManagement.SceneManager.LoadScene("MingleRoom", UnityEngine.SceneManagement.LoadSceneMode.Additive);

      // UnityEngine.SceneManagement.SceneManager.LoadScene("MingleRoom");

    }

    public void UnLoadRoomeScene()
    {
      if (!_isLoad_SelectRoom) return;
      _isLoad_SelectRoom = false;
      UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("MingleRoom");
    }
    public IEnumerator LoadPrefileScene()
    {
      if (_isLoad_Profil) yield break;
      _isLoad_Profil = true;

      panel.SetActive(true);

      AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MingleProfile", UnityEngine.SceneManagement.LoadSceneMode.Additive);

      while (!operation.isDone)
      {
        laodSlider.value = operation.progress;
        yield return null;
      }

      if (operation.isDone)
      {
        UnLoadLoadMainScene();
        UnLoadCharacterScene();
        UnLoadRoomeScene();
        yield return new WaitForSeconds(2f);
        panel.SetActive(false);
        yield break;
      }

      // StartCoroutine(test("MingleProfil"));
      // UnityEngine.SceneManagement.SceneManager.LoadScene("MingleProfil", UnityEngine.SceneManagement.LoadSceneMode.Additive);
      // UnLoadLoadMainScene();
      // UnLoadCharacterScene();
      // UnLoadRoomeScene();
      // UnityEngine.SceneManagement.SceneManager.LoadScene("MingleProfil");
    }

    public void UnLoadPrefileScene()
    {
      if (!_isLoad_Profil) return;
      _isLoad_Profil = false;
      UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("MingleProfile");
    }
  }
}