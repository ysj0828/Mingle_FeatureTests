using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MingleMain
{
  public class FriendHead : MonoBehaviour
  {
    private FriendManager _parent;
    // Start is called before the first frame update
    void Start()
    {
      _parent = transform.parent.GetComponent<FriendManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseUp()
    {
      _parent.OnClickHead(this.gameObject);
    }
  }
}