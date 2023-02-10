using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MingleCamera
{
    [DisallowMultipleComponent]
    public class AssetMove : MonoBehaviour
    {
        [HideInInspector] public float collisionMaxY;
        [HideInInspector] public float overlapPositionMaxY;
        [HideInInspector] public float initialY;

        [HideInInspector] public CollisionDetection CD;

        public bool enableAssetMove;

        public Touch touch;

        GameObject selectedObjPrivate;
        [HideInInspector]
        public GameObject selectedObj
        {
            get
            {
                return selectedObjPrivate;
            }

            set
            {
                selectedObjPrivate = value;
                fireRaycastBool = false;
                //prevPos = touch.position;
                prevTouchPosIsNull = true;
            }
        }

        bool fireRaycastBool;

        internal bool prevTouchPosIsNull = true;

        bool ismoving = false;
        internal bool IsMoving
        {
            get
            {
                return ismoving;
            }

            set
            {
                ismoving = value;
                if (value)
                {
                    rotationAsset.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0.4f);
                    translationImage.GetComponent<RawImage>().color = new Color(1, 1, 1, 1);
                    rotationAsset.GetComponent<Outline>().enabled = false;
                }

                else if (!value)
                {
                    rotationAsset.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1);
                    translationImage.GetComponent<RawImage>().color = new Color(1, 1, 1, 0.4f);
                    rotationAsset.GetComponent<Outline>().enabled = true;
                }
            }
        }

        [HideInInspector] public GameObject collisionObj;

        cinemachine_test CT;
        RotationUI RUI;
        [SerializeField] GameObject rotationAsset;
        TranslationUI TUI;
        [SerializeField] GameObject translationImage;


        [SerializeField] Camera mainCam;
        [SerializeField] GameObject mainCamGO;

        int movableLayer = 7;
        int groundLayer = 6;
        int NPCLayer = 8;
        int ignoreRaycast = 2;
        int mainUserLayer = 3;
        int selectedLayer = 9;

        int groundLayerMask;
        int ignoreAvatarLayerMask;

        [HideInInspector]
        public Vector3 otherPosition;
        [HideInInspector]
        public Bounds otherBounds;
        [HideInInspector]
        public Bounds selectedObjBounds;

        [HideInInspector]
        public GameObject[] GOArray;

        [HideInInspector]
        public Bounds[] GOBounds;

        RaycastHit initialHit;

        Vector2 doubleCheckTouch;

        internal Vector2 prevPos;

        // static AssetMove instance = null;

        private void Awake()
        {
            CT = GetComponent<cinemachine_test>();
            RUI = rotationAsset.GetComponent<RotationUI>();
            TUI = translationImage.GetComponent<TranslationUI>();

            groundLayerMask = 1 << groundLayer;

            ignoreAvatarLayerMask = ~((1 << NPCLayer) | (1 << 0));

            //overlapBoxLayerMask = ~((1 << ignoreRaycast) | (1 << mainUserLayer) | (1 << NPCLayer));
        }

        private void Start()
        {
            // if (instance == null)
            // {
            //     instance = this;
            // }
            // else
            // {
            //     Destroy(this.gameObject);
            // }
            // DontDestroyOnLoad(this.gameObject);
            // if (instance == null)
            // {
            //     instance = this;
            //     DontDestroyOnLoad(this);
            // }

            // else
            // {
            //     Destroy(this);
            // }
        }

        float AssetYPosition()
        {
            float tempMagY = 0;
            float tempPosY = 0;

            if (CD.colliderList != null)
            {
                foreach (Collider col in CD.colliderList)
                {
                    if (tempMagY < col.bounds.size.y - 0.1f)
                    {
                        tempMagY = col.bounds.size.y / 2f + col.bounds.center.y - 0.1f;
                    }

                    if (tempPosY < col.transform.position.y)
                    {
                        tempPosY = col.transform.position.y;
                    }
                }

                return tempMagY + tempPosY - 0.1f;
            }

            else
            {
                return initialY;
            }
        }

        private void Update()
        {
            // if (CD != null && CD.colliderList.Count != 0)
            // {
            //     foreach (Collider col in CD.colliderList)
            //     {
            //         if (collisionMaxY < col.bounds.size.y)
            //         {
            //             if (overlapPositionMaxY < col.transform.position.y - 0.1f)
            //             {
            //                 overlapPositionMaxY = col.transform.position.y / 2f + col.bounds.center.y - 0.1f;
            //             }

            //             // collisionMaxY = col.bounds.size.y - selectedObj.GetComponent<Renderer>().bounds.size.y * 0.02f + overlapPositionMaxY;
            //             collisionMaxY = col.bounds.size.y - selectedObj.GetComponent<Renderer>().bounds.size.y * 0.02f + overlapPositionMaxY;
            //             //collisionMaxY = col.bounds.size.y + col.transform.position.y;
            //         }
            //     }
            // }

            if (CD != null && CD.colliderList.Count == 0 && collisionMaxY != 0)
            {
                collisionMaxY = initialY;
            }

            if (Input.touchCount == 0)
            {
                if (doubleCheckTouch != new Vector2(999, 999))
                {
                    doubleCheckTouch = new Vector2(999, 999);
                }

                fireRaycastBool = false;
            }

            if (Input.touchCount == 1 && enableAssetMove)
            {
                if (touch.phase == TouchPhase.Began || doubleCheckTouch == new Vector2(999, 999))
                {
                    fireRaycastBool = Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out initialHit, maxDistance: Mathf.Infinity, layerMask: 1 << selectedLayer);
                    //if (GameObjectOverUI(touch))
                    //{
                    //    CT.enableRotation = false;
                    //    RaycastCheckMove = false;
                    //}

                    //else if (!GameObjectOverUI(touch))
                    //{
                    //    //RaycastCheckMove = true;

                    //    RaycastHit checkMove;

                    //    if (Physics.Raycast(mainCam.ScreenPointToRay(touch.position), out checkMove, maxDistance: Mathf.Infinity, layerMask: 1 << selectedLayer))
                    //    {
                    //        RaycastCheckMove = true;
                    //    }
                    //}

                    //if (GameObjectOverUI(touch) && Physics.Raycast(mainCam.ScreenPointToRay(touch.position), out checkMove, maxDistance: Mathf.Infinity, layerMask: 1 << selectedLayer))
                    //{
                    //    RaycastCheckMove = true;
                    //}

                    //if (GameObjectOverUI(touch) && !Physics.Raycast(mainCam.ScreenPointToRay(touch.position), out checkMove, maxDistance: Mathf.Infinity, layerMask: 1 << selectedLayer))
                    //{
                    //    RaycastCheckMove = false;
                    //}

                    //if (!GameObjectOverUI(touch))
                    //{
                    //    RaycastCheckMove = true;
                    //}

                    doubleCheckTouch = touch.position;
                }

                if (IsMoving)
                {
                    if (!fireRaycastBool)
                    {
                        return;
                    }

                    if (rotationAsset.GetComponent<Renderer>().material.color != new Color(0, 0, 0, 0.4f) || translationImage.GetComponent<RawImage>().color != new Color(1, 1, 1, 1))
                    {
                        rotationAsset.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0.4f);
                        translationImage.GetComponent<RawImage>().color = new Color(1, 1, 1, 1);
                        rotationAsset.GetComponent<Outline>().enabled = false;
                    }

                    if (initialHit.transform.gameObject == selectedObj)
                    {
                        RaycastHit hit;
                        CT.enableRotation = false;

                        Physics.Raycast(mainCam.ScreenPointToRay(touch.position), out hit, maxDistance: Mathf.Infinity, layerMask: 1 << groundLayer);
                        //Physics.Raycast(mainCam.ScreenPointToRay(touch.position), out hit, maxDistance: Mathf.Infinity, layerMask: (1 << groundLayer) | (1 << movableLayer));

                        if (touch.position.x < Screen.width * 0.7 && touch.position.x > Screen.width * 0.3 && touch.position.y < Screen.height * 0.8 && touch.position.y > Screen.height * 0.2)
                        {
                            //CT.vcamFraming.m_CameraDistance = (CT.virtualCamFraming.transform.position - hit.point).magnitude;
                        }

                        selectedObj.transform.position += Quaternion.Euler(0, mainCamGO.transform.eulerAngles.y, 0) * new Vector3(touch.deltaPosition.x, 0, touch.deltaPosition.y) * 0.005f;
                        // selectedObj.transform.position = new Vector3(selectedObj.transform.position.x, collisionMaxY, selectedObj.transform.position.z);
                        selectedObj.transform.position = new Vector3(selectedObj.transform.position.x, AssetYPosition(), selectedObj.transform.position.z);
                        TUI.AssetPosition = selectedObj.transform.position;
                    }
                }

                else if (!IsMoving)
                {
                    RaycastHit checkUIHit;

                    if (rotationAsset.GetComponent<Renderer>().material.color != new Color(0, 0, 0, 1) || translationImage.GetComponent<RawImage>().color != new Color(1, 1, 1, 0.4f))
                    {
                        rotationAsset.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1);
                        translationImage.GetComponent<RawImage>().color = new Color(1, 1, 1, 0.4f);
                        rotationAsset.GetComponent<Outline>().enabled = true;
                    }

                    if (Physics.Raycast(mainCam.ScreenPointToRay(touch.position), out checkUIHit, 100, 1 << 31))
                    {
                        if (checkUIHit.transform.tag == "RotationUI")
                        {
                            if (prevTouchPosIsNull)
                            {
                                prevPos = touch.position;
                                prevTouchPosIsNull = false;
                            }

                            CT.enableRotation = false;

                            Vector3 ObjOrigin = mainCam.WorldToScreenPoint(selectedObj.transform.position);
                            Vector2 PointA = prevPos - new Vector2(ObjOrigin.x, ObjOrigin.y);
                            Vector2 PointB = touch.position - new Vector2(ObjOrigin.x, ObjOrigin.y);

                            float testangle = Mathf.Atan2(PointA.x * PointB.y - PointA.y * PointB.x, PointA.x * PointB.x + PointA.y * PointB.y);

                            RUI.angle = testangle;

                            selectedObj.transform.Rotate(0, -testangle * 50, 0);
                        }
                    }
                }

                prevPos = touch.position;
            }
        }

        bool GameObjectOverUI(Touch touch)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);

            eventData.position = new Vector2(touch.position.x, touch.position.y);

            List<RaycastResult> results = new List<RaycastResult>();

            EventSystem.current.RaycastAll(eventData, results);

            foreach (var temp in results)
            {
                if (temp.gameObject.tag == "RotationUI")
                {
                    return true;
                }
                Debug.Log(temp.gameObject.name);
            }
            return false;
        }

        // void FireRaycast(Touch touch)
        // {
        //     fireRaycastBool = Physics.Raycast(mainCam.ScreenPointToRay(touch.position), out initialHit, maxDistance: Mathf.Infinity, layerMask: 1 << selectedLayer);
        //     if (!fireRaycastBool) return;
        // }
    }
}