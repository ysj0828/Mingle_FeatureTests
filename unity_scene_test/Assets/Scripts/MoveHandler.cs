using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MingleCamera
{
    public class MoveHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public GameObject selectedObj;

        [HideInInspector] public GameObject collisionObj;

        [HideInInspector] public float selectedObjHeight;

        MasterController MC;

        int groundLayer = 6;
        int NPCLayer = 8;
        int ignoreRaycast = 2;
        int mainUserLayer = 3;

        int groundLayerMask;
        int ignoreAvatarLayerMask;

        int overlapBoxLayerMask;

        [HideInInspector]
        public Vector3 otherPosition;
        [HideInInspector]
        public Bounds otherBounds;

        [HideInInspector]
        public Bounds selectedObjBounds;

        bool isHeightNull;

        Plane plane = new Plane(Vector3.down, 0);

        [HideInInspector]
        public GameObject[] GOArray;

        [HideInInspector]
        public Bounds[] GOBounds;

        float highestY;

        //[HideInInspector]
        //public float minX, maxX, minY, maxY;

        //[HideInInspector]
        //public float PlaneminX, PlanemaxX, PlaneminY, PlanemaxY;



        //initialise layermask using bit operator
        private void OnEnable()
        {
            MC = Camera.main.GetComponent<MasterController>();
            
            groundLayerMask = 1 << groundLayer;
            //layerMask = ~((1 << NPCLayer) | (1 << ignoreRaycast));

            selectedObjHeight = selectedObjBounds.size.y;

            ignoreAvatarLayerMask = ~((1 << NPCLayer) | (1 << 0));

            overlapBoxLayerMask = ~((1 << ignoreRaycast) | (1 << mainUserLayer) | (1 << NPCLayer));
        }


        //Tell MasterController.cs if currently dragging or not
        #region Implement Pointer Up n Down Interface
        public void OnPointerDown(PointerEventData eventData)
        {
            //TODO - send bool value to Master Controller to disable rotation
            //MC.inEditMode = true;
            //MC.whileDragging = true;

            selectedObjBounds = selectedObj.GetComponent<Renderer>().bounds;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //TODO - send bool value to Master Controller to enable rotation
            //MC.inEditMode = false;
            //MC.whileDragging = false;
        }
        #endregion




        






        //where all the calculation of the position of the selected object takes place
        public void OnDrag(PointerEventData eventData)
        {
            Touch touch = Input.touches[0];
            RaycastHit hit;

            Collider[] results = new Collider[12];

            int overlapCount = Physics.OverlapBoxNonAlloc(selectedObj.transform.position, selectedObjBounds.size / 2, results, selectedObj.transform.rotation, overlapBoxLayerMask);


            if (overlapCount == 0)
            {
                highestY -= selectedObjBounds.size.y;
            }

            else if (overlapCount > 1)
            {
                for (int i = 0; i < overlapCount; i++)
                {
                    if (results[i].gameObject.tag == "Walkable")
                    {
                        continue;
                    }

                    else
                    {
                        if (highestY < results[i].gameObject.transform.position.y + results[i].gameObject.GetComponent<Renderer>().bounds.size.y / 2)
                        {
                            highestY = results[i].gameObject.transform.position.y + results[i].gameObject.GetComponent<Renderer>().bounds.size.y / 2;
                        }
                    }
                }
            }

            selectedObjHeight = highestY + selectedObjBounds.size.y / 2;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position + new Vector2(0, 80)), out hit, maxDistance: 100, layerMask: groundLayerMask))
            {
                selectedObj.transform.position = new Vector3(hit.point.x, selectedObjHeight, hit.point.z);
            }




            //(Deprecated)
            //Fixme - position based raycast
            //if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hit, maxDistance: 100, layerMask: layerMask))
            //{
            //    selectedObj.transform.position = new Vector3(hit.point.x, selectedObjHeight, hit.point.z);
            //}

            //if ((otherPosition - selectedObj.transform.position).magnitude > selectedObjBounds.size.x * Mathf.Sqrt(3))
            //{
            //    selectedObjHeight = selectedObjBounds.size.y/2;
            //}


            //(Deprecated)
            //original code
            //fixme - find the maxY of handler button
            //Ray ray = Camera.main.ScreenPointToRay(touch.position + new Vector2(0, 80));
            //if (Physics.Raycast(ray, out hit, maxDistance: 100, layerMask: layerMask))
            //{
            //    Debug.Log("collisionObj" + collisionObj);

            //    float yPosition = hit.transform.position.y + hit.transform.GetComponent<Renderer>().bounds.size.y / 2 + selectedObjBounds.size.y / 2;
            //    if (collisionObj == null)
            //    {
            //        selectedObj.transform.position = new Vector3(hit.point.x, Mathf.Clamp(yPosition, 0.5f, 100), hit.point.z);
            //    }

            //    else if (collisionObj != null)
            //    {
            //        //RaycastHit down;
            //        //Physics.Raycast(origin: selectedObj.transform.position, direction: -selectedObj.transform.up, out down);

            //        selectedObj.transform.position = new Vector3(hit.point.x, Mathf.Clamp((height.collisionObject.transform.GetComponent<Renderer>().bounds.size.y + hit.transform.position.y + hit.transform.GetComponent<Renderer>().bounds.size.y / 2 + selectedObjBounds.size.y / 2), 0.5f, 100), hit.point.z);
            //    }
            //}

            transform.position = touch.position;
        }
    }
}
