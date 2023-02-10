using UnityEngine;
using Cinemachine;

//
using UnityEngine.UI;
using TMPro;
//

namespace MingleCamera
{
    public class CameraControl : MonoBehaviour
    {
        float rotationMX;
        float rotationMY;

        float rotationXSensitivity;
        float rotationYSensitivity;
        float zoomScale = 1;
        float moveScale = 1;

        //public GameObject cameraPosition;
        public Transform targetPosition;

        public GameObject cameraBounds;

        float touchX;
        float touchY;

        float touchXX;
        float touchYY;

        float combinedX;
        float combinedY;
        
        Touch touchZero;
        Touch touchOne;

        //Vector2 initTouchPos;
        Vector2 init2TouchPos1;
        Vector2 init2TouchPos2;

        int zoomMax = 60;
        int zoomMin = 5;

        [HideInInspector]
        public bool restrictRotation = false;

        float minX, maxX, minY, maxY, minZ, maxZ;

        internal void InitialOffset()
        {
            //cameraPosition.transform.position = targetPosition.transform.position - cameraPosition.transform.forward * Mathf.Abs((cameraPosition.transform.position - targetPosition.transform.position).magnitude);
            rotationMX = Camera.main.transform.localEulerAngles.x;
            rotationMY = Camera.main.transform.localEulerAngles.y;

            minX = cameraBounds.GetComponent<Collider>().bounds.min.x;
            maxX = cameraBounds.GetComponent<Collider>().bounds.max.x;

            minY = cameraBounds.GetComponent<Collider>().bounds.min.y;
            maxY = cameraBounds.GetComponent<Collider>().bounds.max.y;

            minZ = cameraBounds.GetComponent<Collider>().bounds.min.z;
            maxZ = cameraBounds.GetComponent<Collider>().bounds.max.z;

            //rotationMX = cameraPosition.transform.localEulerAngles.x;
            //rotationMY = cameraPosition.transform.localEulerAngles.y;
        }


        #region Rotation
        public void SingleFingerRotate(Touch touch)
        {
            if (!restrictRotation)
            {
                Vector3 normalisePos = touch.deltaPosition.normalized;

                //rotationXSensitivity = 0.5f * (touch.deltaPosition.magnitude / Time.deltaTime) / 300;
                //rotationYSensitivity = 1f * (touch.deltaPosition.magnitude / Time.deltaTime) / 300;

                //rotationXSensitivity = 0.25f * (touch.deltaPosition.magnitude / Time.deltaTime) / 300;
                //rotationYSensitivity = 0.5f * (touch.deltaPosition.magnitude / Time.deltaTime) / 300;

                rotationXSensitivity = 0.25f * (touch.deltaPosition.magnitude) / 11;
                rotationYSensitivity = 0.5f * (touch.deltaPosition.magnitude) / 11;

                float touchXX = normalisePos.x;
                float touchYY = -normalisePos.y;

                rotationMX += touchYY * rotationXSensitivity;
                rotationMY += touchXX * rotationYSensitivity;

                rotationMX = Mathf.Clamp(rotationMX, 0, 90);

                //cameraPosition.transform.localEulerAngles = new Vector3(rotationMX, rotationMY, 0);

                //cameraPosition.transform.position = targetPosition.transform.position - cameraPosition.transform.forward * Mathf.Abs((cameraPosition.transform.position - targetPosition.transform.position).magnitude);


                //if (!toggle.isOn)
                //{



                //Camera.main.transform.localEulerAngles = new Vector3(rotationMX, rotationMY, 0);

                Camera.main.transform.rotation = Quaternion.Euler(new Vector3(rotationMX, rotationMY, 0));





                //Vector3 camPos = targetPosition.transform.position - Camera.main.transform.forward * Mathf.Abs((Camera.main.transform.position - targetPosition.transform.position).magnitude);
                //Vector3 camPosClamped = new Vector3(Mathf.Clamp(camPos.x, minX, maxX), Mathf.Clamp(camPos.y, minY, maxY), Mathf.Clamp(camPos.z, minZ, maxZ));

                //Camera.main.transform.position = camPosClamped;



                Camera.main.transform.position = targetPosition.transform.position - Camera.main.transform.forward * Mathf.Abs((Camera.main.transform.position - targetPosition.transform.position).magnitude);
                //}

                //Camera.main.transform.LookAt(targetPosition);
            }
        }
        #endregion


        #region Move and Zoom
        public void DoubleFinger(Touch touches1, Touch touches2)
        {
            touches1 = Input.GetTouch(0);
            touches2 = Input.GetTouch(1);

            touchX = touches1.deltaPosition.x;
            touchY = touches1.deltaPosition.y;

            touchXX = touches2.deltaPosition.x;
            touchYY = touches2.deltaPosition.y;

            combinedX = touchX + touchXX;
            combinedY = touchY + touchYY;

            if (!restrictRotation)
            {
                restrictRotation = true;
            }

            if (touchZero.phase == TouchPhase.Began)
            {
                init2TouchPos1 = touches1.position;
            }

            if (touchOne.phase == TouchPhase.Began)
            {
                init2TouchPos2 = touches2.position;
            }

            else if (init2TouchPos1 == new Vector2(9999, 9999) || init2TouchPos2 == new Vector2(9999, 9999))
            {
                init2TouchPos1 = touches1.position;
                init2TouchPos2 = touches2.position;
            }

            //Move

            moveScale = Mathf.Abs(touchX) + Mathf.Abs(touchY);
            moveScale = Mathf.Clamp(moveScale, 0, 45);

            Vector3 newV3 = new Vector3(-combinedX, combinedY, 0);

            //if ((cameraPosition.transform.position - targetPosition.transform.position).magnitude > zoomMax - 2.5f)
            //{
            //targetPosition.transform.position += Quaternion.Euler(0, cameraPosition.transform.eulerAngles.y, 0) * newV3 * moveScale * (0.5f) / 4750;
            //targetPosition.transform.position += Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * newV3 * moveScale * (0.5f) / 4750;
            //}

            //cameraPosition.transform.position += Quaternion.Euler(0, cameraPosition.transform.eulerAngles.y, 0) * newV3 * moveScale * (0.5f) / 4750;


            //Vector3 camPos = Camera.main.transform.position + Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * newV3 * moveScale * (0.5f) / 4750;
            //Vector3 camPosClamped = new Vector3(Mathf.Clamp(camPos.x, minX, maxX), Mathf.Clamp(camPos.y, minY, maxY), Mathf.Clamp(camPos.z, minZ, maxZ));


            //Camera.main.transform.position = camPosClamped;





            Camera.main.transform.position += Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * newV3 * moveScale * (0.5f) / 4750;


            //Zoom

            Vector2 touchZeroPos = touches1.position - touches1.deltaPosition;
            Vector2 touchOnePos = touches2.position - touches2.deltaPosition;

            float prevMagnitude = (touchZeroPos - touchOnePos).magnitude;

            float currentMagnitude = (touches1.position - touches2.position).magnitude;

            float diff = currentMagnitude - prevMagnitude;

            zoomScale = (touches1.deltaPosition.magnitude + touches2.deltaPosition.magnitude) / 700;
            zoomScale = Mathf.Clamp(zoomScale, 0, 0.025f);

            //float posMag = (cameraPosition.transform.position - targetPosition.transform.position).magnitude;
            float posMag = (Camera.main.transform.position - targetPosition.transform.position).magnitude;

            diff = Mathf.Clamp(diff, -150, 150);

            zoom(diff * zoomScale, diff, posMag);
        }
        #endregion


        public void CamFollow(GameObject campos)
        {
            //if (toggle.isOn)
            //{
            //    Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, campos.transform.position, 0.25f);
            //    Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, campos.transform.rotation, 0.25f);
            //}

            adjustCamPos();
        }


        #region Zoom Logic
        void zoom(float increment, float diff, float posMag)
        {
            if (posMag > zoomMax)
            {
                if (diff < 0)
                {
                    //Vector3 camPos = Camera.main.transform.position + Camera.main.transform.forward * increment / (posMag * posMag);
                    //Vector3 camPosClamped = new Vector3(Mathf.Clamp(camPos.x, minX, maxX), Mathf.Clamp(camPos.y, minY, maxY), Mathf.Clamp(camPos.z, minZ, maxZ));
                    ////cameraPosition.transform.position += cameraPosition.transform.forward * increment / (posMag * posMag);


                    //Camera.main.transform.position = camPosClamped;



                    Camera.main.transform.position += Camera.main.transform.forward * increment / (posMag * posMag);
                }

                if (diff > 0)
                {
                    //Vector3 camPos = Camera.main.transform.position + Camera.main.transform.forward * increment;
                    //Vector3 camPosClamped = new Vector3(Mathf.Clamp(camPos.x, minX, maxX), Mathf.Clamp(camPos.y, minY, maxY), Mathf.Clamp(camPos.z, minZ, maxZ));
                    ////cameraPosition.transform.position += cameraPosition.transform.forward * increment;


                    //Camera.main.transform.position = camPosClamped;




                    Camera.main.transform.position += Camera.main.transform.forward * increment;
                }
            }

            if (posMag < zoomMin)
            {
                if (diff < 0)
                {
                    //Vector3 camPos = Camera.main.transform.position + Camera.main.transform.forward * increment;
                    //Vector3 camPosClamped = new Vector3(Mathf.Clamp(camPos.x, minX, maxX), Mathf.Clamp(camPos.y, minY, maxY), Mathf.Clamp(camPos.z, minZ, maxZ));
                    ////cameraPosition.transform.position += cameraPosition.transform.forward * increment;


                    //Camera.main.transform.position = camPosClamped;



                    Camera.main.transform.position += Camera.main.transform.forward * increment;
                }

                if (diff > 0)
                {
                    //Vector3 camPos = Camera.main.transform.position + Camera.main.transform.forward * increment / (posMag * posMag);
                    //Vector3 camPosClamped = new Vector3(Mathf.Clamp(camPos.x, minX, maxX), Mathf.Clamp(camPos.y, minY, maxY), Mathf.Clamp(camPos.z, minZ, maxZ));
                    ////cameraPosition.transform.position += cameraPosition.transform.forward * increment / (posMag * posMag);


                    //Camera.main.transform.position = camPosClamped;




                    Camera.main.transform.position += Camera.main.transform.forward * increment / (posMag * posMag);
                }
            }

            if (posMag >= zoomMin && posMag <= zoomMax)
            {
                //Vector3 camPos = Camera.main.transform.position + Camera.main.transform.forward * increment;
                //Vector3 camPosClamped = new Vector3(Mathf.Clamp(camPos.x, minX, maxX), Mathf.Clamp(camPos.y, minY, maxY), Mathf.Clamp(camPos.z, minZ, maxZ));
                ////cameraPosition.transform.position += cameraPosition.transform.forward * increment;


                //Camera.main.transform.position = camPosClamped;



                Camera.main.transform.position += Camera.main.transform.forward * increment;
            }
        }

        public void adjustCamPos()
        {
            if (Input.touchCount == 0 && (Camera.main.transform.position - targetPosition.transform.position).magnitude > zoomMax - 2.5f)
            {
                //Vector3 camPos = targetPosition.transform.position - Camera.main.transform.forward * Mathf.Abs((Camera.main.transform.position - targetPosition.transform.position).magnitude);
                //Vector3 camPosClamped = new Vector3(Mathf.Clamp(camPos.x, minX, maxX), Mathf.Clamp(camPos.y, minY, maxY), Mathf.Clamp(camPos.z, minZ, maxZ));
                ////cameraPosition.transform.position = targetPosition.transform.position - cameraPosition.transform.forward * Mathf.Abs((cameraPosition.transform.position - targetPosition.transform.position).magnitude);


                //Camera.main.transform.position = camPosClamped;



                Camera.main.transform.position = targetPosition.transform.position - Camera.main.transform.forward * Mathf.Abs((Camera.main.transform.position - targetPosition.transform.position).magnitude);
            }

            //if (Input.touchCount == 0 && (cameraPosition.transform.position - targetPosition.transform.position).magnitude > zoomMax - 2.5f)
            //{
            //    //cameraPosition.transform.position = targetPosition.transform.position - cameraPosition.transform.forward * Mathf.Abs((cameraPosition.transform.position - targetPosition.transform.position).magnitude);
            //    Camera.main.transform.position = targetPosition.transform.position - Camera.main.transform.forward * Mathf.Abs((Camera.main.transform.position - targetPosition.transform.position).magnitude);
            //}

            //if (((cameraPosition.transform.position - targetPosition.transform.position).magnitude < zoomMin + 2 && Input.touchCount < 2 && cameraPosition.transform.position.y > 0))
            //{
            //    //cameraPosition.transform.position -= cameraPosition.transform.forward;
            //    Camera.main.transform.position -= Camera.main.transform.forward;

            //}

            if (((Camera.main.transform.position - targetPosition.transform.position).magnitude < zoomMin + 2 && Input.touchCount < 2 && Camera.main.transform.position.y > 0))
            {
                //Vector3 camPos = Camera.main.transform.position - Camera.main.transform.forward;
                //Vector3 camPosClamped = new Vector3(Mathf.Clamp(camPos.x, minX, maxX), Mathf.Clamp(camPos.y, minY, maxY), Mathf.Clamp(camPos.z, minZ, maxZ));
                ////cameraPosition.transform.position -= cameraPosition.transform.forward;


                //Camera.main.transform.position = camPosClamped;



                Camera.main.transform.position -= Camera.main.transform.forward;

            }

            //else if ((cameraPosition.transform.position - targetPosition.transform.position).magnitude > zoomMax - 2 && Input.touchCount < 2)
            //{
            //    //cameraPosition.transform.position += cameraPosition.transform.forward;
            //    Camera.main.transform.position += Camera.main.transform.forward;
            //}

            else if ((Camera.main.transform.position - targetPosition.transform.position).magnitude > zoomMax - 2 && Input.touchCount < 2)
            {
                //Vector3 camPos = Camera.main.transform.position + Camera.main.transform.forward;
                //Vector3 camPosClamped = new Vector3(Mathf.Clamp(camPos.x, minX, maxX), Mathf.Clamp(camPos.y, minY, maxY), Mathf.Clamp(camPos.z, minZ, maxZ));
                ////cameraPosition.transform.position += cameraPosition.transform.forward;


                //Camera.main.transform.position = camPosClamped;



                Camera.main.transform.position += Camera.main.transform.forward;
            }
        }
        #endregion



        #region Initialisation
        public void InitialiseObjects()
        {
            //CinemachineCore.GetInputAxis = InputDelegate;




            //cameraPosition = GameObject.Find("camPos");

            //if (cameraPosition == null)
            //{
            //    cameraPosition = new GameObject("CameraFollowPosition");

            //    cameraPosition.transform.position = Camera.main.transform.position;
            //    cameraPosition.transform.rotation = Camera.main.transform.rotation;
            //}

            //TODO - set target position to room/avatar (need feedback)
            //int i = GameObject.FindGameObjectWithTag("PlayerAvatar").transform.childCount;
            //targetPosition = GameObject.FindGameObjectWithTag("PlayerAvatar").transform.GetChild(i-1);
            //if (targetPosition == null)
            //{
            //    targetPosition = new GameObject("TargetPosition");
            //    targetPosition.transform.position = new Vector3(0, 0, 0);
            //}
        }
        #endregion





        #region Cinemachine Input

        //float InputDelegate(string axisName)
        //{
        //    switch (axisName)
        //    {
        //        case "Mouse X":
        //            if (Input.touchCount == 1)
        //            {
        //                return Input.touches[0].deltaPosition.x;
        //            }

        //            else
        //            {
        //                return Input.GetAxis(axisName);
        //            }

        //        case "Mouse Y":
        //            if (Input.touchCount == 1)
        //            {
        //                return Input.touches[0].deltaPosition.y;
        //            }

        //            else
        //            {
        //                return Input.GetAxis(axisName);
        //            }
        //    }

        //    return 0f;
        //}



        #endregion









        #region Calculate 










        #endregion

        #region Change POV

        public void ChangePOVFirstPerson()
        {

        }




        #endregion
    }
}

//TODO - breh