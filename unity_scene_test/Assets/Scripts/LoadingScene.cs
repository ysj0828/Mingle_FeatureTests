using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{

    [SerializeField] Slider loadingBar;
    [SerializeField] GameObject loadingPanel;

    public void LoadScene(int levelIndex)
    {
        StartCoroutine(AsyncLoadScene(levelIndex));
    }

    IEnumerator AsyncLoadScene(int levelIndex)
    {
        PhotonNetwork.LoadLevel(levelIndex);
        loadingPanel.SetActive(true);

        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            loadingBar.value = PhotonNetwork.LevelLoadingProgress;
            yield return null;
        }
    }
}
