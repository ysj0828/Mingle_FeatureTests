using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class screenregion : MonoBehaviour
{
    public TextMeshProUGUI t1;
    public TextMeshProUGUI t2;

    public Image image;

    int buildId;

    private void Start()
    {
        buildId = SceneManager.GetActiveScene().buildIndex;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.touchCount == 1)
        {
            Touch touch = Input.touches[0];

            t1.text = touch.position.ToString();

            if (buildId == 2)
            {
                if (touch.position.y < Screen.height * 0.24 || touch.position.y > Screen.height * 0.73)
                {
                    t2.text = "disable touch";
                }

                else
                {
                    t2.text = "enable touch";
                }
            }

            else if (buildId == 3)
            {
                if (touch.position.y < Screen.height * 0.5566 || touch.position.y > Screen.height * (1 - 0.28f))
                {
                    t2.text = "disable touch";
                }

                else
                {
                    t2.text = "enable touch";
                }
            }
            
        }
    }

    public void BackToMainScene()
    {
        SceneManager.LoadScene(1);
    }
}