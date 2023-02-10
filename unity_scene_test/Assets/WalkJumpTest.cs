using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WalkJumpTest : MonoBehaviour
{
    public string sceneSelect;

    // Start is called before the first frame update
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
