using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWrench : MonoBehaviour
{
    [HideInInspector]
    public Transform Target
    {
        get
        {
            return target;
        }

        set
        {
            target = value;
            targetBounds = value.GetComponent<Renderer>().bounds;
            float assetHeight = targetBounds.size.y / 2 + transform.GetComponent<Renderer>().bounds.size.y;
            transform.position = Target.position + new Vector3(0, assetHeight, 0);
        }
    }

    private void OnEnable()
    {
        transformBounds = transform.GetComponent<Renderer>().bounds;
    }

    Bounds transformBounds;
    Bounds targetBounds;
    Transform target;

    void Update()
    {
        if (target != null)
        {
            transform.position = Target.position + new Vector3(0, targetBounds.size.y + transformBounds.size.y, 0);

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 30 * Time.deltaTime, transform.eulerAngles.z);
        }
    }


}
