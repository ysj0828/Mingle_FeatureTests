using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class LagCompensation : MonoBehaviour, IPunObservable
{

    PhotonView view;

    Vector3 networkPosition;

    float smoothPos;
    float teleportDistance;


    private void OnEnable()
    {
        Debug.Log("on enable");

        view = GetComponent<PhotonView>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);

        }

        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();



            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            // networkPosition += tra.velocity * lag;
        }
    }



    // Update is called once per frame
    void FixedUpdate()
    {

        if (!view.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, smoothPos * Time.fixedDeltaTime);

            if (Vector3.Distance(transform.position, networkPosition) > 5)
            {
                transform.position = networkPosition;
            }
        }




    }
}
