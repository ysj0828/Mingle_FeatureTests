using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MingleCamera
{
    public class CollisionDetection : MonoBehaviour
    {
        public List<Collider> colliderList = new List<Collider>();

        private void OnTriggerEnter(Collider other)
        {
            if ((other.tag == "Selectable" || other.tag == "NotSelectable") && !colliderList.Contains(other))
            {
                colliderList.Add(other);
            }
        }

        private void OnTriggerStay(Collider other)
        {
        }

        private void OnTriggerExit(Collider other)
        {
            if (colliderList.Contains(other))
            {
                colliderList.Remove(other);
            }
        }
    }
}
