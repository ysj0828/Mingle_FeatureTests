using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialiseCollider : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        var size = transform.GetComponent<BoxCollider>();
        size.size = new Vector3(size.size.x * 1.02f, size.size.y * 1.02f, size.size.z * 1.2f);
    }
}