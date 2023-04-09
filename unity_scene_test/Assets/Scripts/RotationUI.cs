using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MingleCamera
{
    public class RotationUI : MonoBehaviour
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
                var BoundaryComponent = value.GetComponent<Renderer>().bounds;
                var MaxBoundary = Mathf.Clamp(Mathf.Max(BoundaryComponent.size.x, BoundaryComponent.size.z) * 1.1f, 0.8f, 10f);
                transform.localScale = new Vector3(MaxBoundary, MaxBoundary, 0.001f);
                transform.position = Target.position;
            }
        }
        Transform target;

        // public Vector3 offset;

        // [SerializeField] Camera cam;

        // [HideInInspector] public cinemachine_test CT;
        // [HideInInspector] public AssetMove AM;

        [HideInInspector] public float angle;
        float prevangle;

        // [HideInInspector] public bool isMoving;


        // private void OnEnable()
        // {
        //     AM = GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<AssetMove>();
        // }

        //public void OnPointerDown(PointerEventData eventData)
        //{
        //    CT.enableRotation = false;
        //    AM.RaycastCheckMove = false;
        //    Debug.Log("down");
        //}

        //public void OnPointerUp(PointerEventData eventData)
        //{
        //    CT.enableRotation = true;
        //    AM.RaycastCheckMove = true;
        //}


        private void Update()
        {
            //    if (prevangle > 0 && angle < 0 || prevangle < 0 && angle > 0)
            //    {
            //        flip = false;
            //    }

            //if (target != null)
            //{
            //    Vector3 pos = cam.WorldToScreenPoint(target.position + offset);

            //    //Debug.Log(pos);

            //    if (transform.position != pos)
            //    {
            //        transform.position = pos;
            //    }
            //}
            if (target != null)
            {
                transform.position = Target.position + new Vector3(0, 0.2f, 0);

                //transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x - 90, transform.eulerAngles.y, transform.eulerAngles.z - 10 * Time.deltaTime);

                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 10 * Time.deltaTime);

                prevangle = angle;
            }
        }
    }
}