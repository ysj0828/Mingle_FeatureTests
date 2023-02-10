using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Photon_randomjump : MonoBehaviour
{
    [SerializeField] ParabolaController Parabolacontrol;

    private void OnEnable()
    {
        Parabolacontrol.ParabolaRoot = GameObject.Find("ParaRoot");

        InvokeRepeating("StartJump", 0, 2f);
    }

    void StartJump()
    {
        Parabolacontrol.FollowParabola();
    }
}
