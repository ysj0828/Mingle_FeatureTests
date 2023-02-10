using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gard : MonoBehaviour
{
    List<GameObject> _gards = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        Vector3 start = Camera.main.ScreenToWorldPoint( new Vector3( 0,0,0) );
        Vector3 end = Camera.main.ScreenToWorldPoint( new Vector3( Screen.width, Screen.height,0 ) );
        
        GameObject cube;
        float offset = 0.7f;

        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.layer = LayerMask.NameToLayer("Invisible");
		cube.transform.position = new Vector3(start.x-offset, 1, 0);
        cube.transform.localScale = new Vector3(1,2,start.z*2);
        _gards.Add(cube);
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.layer = LayerMask.NameToLayer("Invisible");
		cube.transform.position = new Vector3(0, 1, start.z-offset);
        cube.transform.localScale = new Vector3(start.x*2-2,2,1);
        _gards.Add(cube);
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.layer = LayerMask.NameToLayer("Invisible");
		cube.transform.position = new Vector3(end.x+offset, 1, 0);
        cube.transform.localScale = new Vector3(1,2,start.z*2);
        _gards.Add(cube);
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.layer = LayerMask.NameToLayer("Invisible");
		cube.transform.position = new Vector3(0, 1, end.z+offset);
        cube.transform.localScale = new Vector3(end.x*2+2,2,1);
        _gards.Add(cube);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
