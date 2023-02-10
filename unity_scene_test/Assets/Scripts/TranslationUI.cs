using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MingleCamera
{
    public class TranslationUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [HideInInspector] public cinemachine_test CT;
        // [HideInInspector] public AssetMove AM;

        [HideInInspector] public Transform Target;

        [HideInInspector]
        internal Vector3 AssetPosition
        {
            get
            {
                return assetposition;
            }

            set
            {
                assetposition = value;
                //transform.position = cam.WorldToScreenPoint(assetposition + offset);
            }
        }
        Vector3 assetposition;

        private void Update()
        {
            transform.position = cam.WorldToScreenPoint(Target.position + offset);
        }

        [HideInInspector] internal bool isMoving;

        public Vector3 offset;

        [SerializeField] Camera cam;

        //[HideInInspector] internal float AlphaValue
        //{
        //    get
        //    {
        //        return alphavalue;
        //    }

        //    set
        //    {
        //        alphavalue = value;
        //        var color = transform.GetComponent<RawImage>().material.color;
        //        color = new Color(color.r, color.g, color.b, alphavalue);
        //    }
        //}

        //float alphavalue;

        public void OnPointerUp(PointerEventData eventData)
        {
            CT.enableRotation = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            CT.enableRotation = false;
        }
    }
}