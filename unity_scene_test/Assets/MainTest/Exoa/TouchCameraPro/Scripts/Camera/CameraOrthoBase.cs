﻿using Exoa.Designer;
using Exoa.Events;
using Exoa.Touch;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Exoa.Cameras
{
    public class CameraOrthoBase : CameraBase, ITouchOrthoCamera
    {
        [Header("DISTANCE")]
        public Vector2 sizeMinMax = new Vector2(1, 12);
        protected float finalSize = 5.0f;

        protected const float distanceToSize = .4f;

        protected float initSize = 6f;
        protected float initDistance = 10f;

        [Header("Z DISTANCE")]
        public float fixedDistance = 30f;

        /// <summary>
        /// returns the camera orthographic size
        /// </summary>
        public float FinalSize
        {
            get
            {
                return finalSize;
            }
        }

        /// <summary>
        /// Init some camera parameters
        /// </summary>
        override protected void Init()
        {
            base.Init();
            finalSize = initSize;
        }


        void Update()
        {
            if (disableMoves)
                return;

            List<TouchFinger> twoFingers = Inputs.TwoFingerFilter.UpdateAndGetFingers();
            List<TouchFinger> oneFinger = Inputs.OneFingerFilter.UpdateAndGetFingers();
            float zoomRatio = 1;
            bool anyInteraction = false;
            Vector2 screenCenter = cam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));

            worldPointCameraCenter = ClampInCameraBoundaries(HeightScreenDepth.Convert(screenCenter));
            worldPointFingersDelta = Vector3.zero;
            worldPointFingersCenter = ClampInCameraBoundaries(HeightScreenDepth.Convert(Inputs.screenPointAnyFingerCountCenter));
            twistRot = Quaternion.identity;
            Log("finalOffset:" + finalOffset);
            Log("FinalPosition:" + finalPosition);
            Log("FinalRotation:" + FinalRotation);
            Log("FinalDistance:" + FinalDistance);

            if (IsInputMatching(InputMapFingerDrag.RotateAround))
            {
                RotateFromVector(Inputs.GetAnyPixelScaledDelta());
                finalRotation = GetRotationFromPitchYaw();
                finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);
                anyInteraction = true;
            }

            if (IsInputMatching(InputMapFingerDrag.RotateHead))
            {
                RotateFromVector(Inputs.GetAnyPixelScaledDelta());
                finalRotation = GetRotationFromPitchYaw();
                anyInteraction = true;
            }

            if (IsInputMatching(InputMapFingerPinch.ZoomAndRotate) || IsInputMatching(InputMapFingerPinch.ZoomOnly))
            {
                zoomRatio = Inputs.pinchRatio;
                anyInteraction = true;
            }

            if (IsInputMatching(InputMapScrollWheel.ZoomInCenter))
            {
                zoomRatio = Inputs.GetScroll();
                worldPointFingersCenter = ClampInCameraBoundaries(HeightScreenDepth.Convert(screenCenter));
                anyInteraction = true;
            }

            if (IsInputMatching(InputMapScrollWheel.ZoomUnderMouse))
            {
                zoomRatio = Inputs.GetScroll();
                worldPointFingersCenter = ClampInCameraBoundaries(HeightScreenDepth.Convert(Input.mousePosition));
                anyInteraction = true;

            }

            finalSize = Mathf.Clamp(finalSize * zoomRatio, sizeMinMax.x, sizeMinMax.y);

            if (sizeMinMax.y == finalSize && zoomRatio > 1 || sizeMinMax.x == finalSize && zoomRatio < 1)
            {
                zoomRatio = 1;
            }

            if (IsInputMatching(InputMapFingerPinch.ZoomAndRotate) || IsInputMatching(InputMapFingerPinch.RotateOnly))
            {
                twistRot = Quaternion.AngleAxis(allowYawRotation ? Inputs.twistDegrees : 0, GetRotateAroundVector());
                anyInteraction = true;
            }
            if (!isFocusingOrFollowing && IsInputMatching(InputMapFingerDrag.Translate))
            {
                worldPointFingersDelta = Vector3.ClampMagnitude(HeightScreenDepth.ConvertDelta(Inputs.lastScreenPointAnyFingerCountCenter,
                Inputs.screenPointAnyFingerCountCenter, gameObject), maxTranslationSpeed);
                anyInteraction = true;
            }
            Log("worldPointFingersCenter:" + worldPointFingersCenter);
            Log("worldPointFingersDelta:" + worldPointFingersDelta);

            Vector3 vecFingersCenterToCamera = (finalPosition - worldPointFingersCenter);
            float vecFingersCenterToCameraDistance = vecFingersCenterToCamera.magnitude * zoomRatio;
            vecFingersCenterToCamera = vecFingersCenterToCamera.normalized * vecFingersCenterToCameraDistance;

            Vector3 targetPosition = worldPointFingersCenter + vecFingersCenterToCamera;
            Log("vecFingersCenterToCamera:" + vecFingersCenterToCamera);
            Log("targetPosition:" + targetPosition);

            Vector3 offsetFromFingerCenter = worldPointFingersCenter - worldPointFingersDelta;

            finalPosition = twistRot * (targetPosition - worldPointFingersCenter) + offsetFromFingerCenter;
            finalRotation = twistRot * finalRotation;

            Vector2 pitchYaw = GetRotationToPitchYaw(finalRotation);
            currentPitch = pitchYaw.x;
            currentYaw = pitchYaw.y;

            Vector3 newWorldPointCameraCenter = CalculateOffset(finalPosition, finalRotation);
            Vector3 newWorldPointCameraCenterClamped = ClampInCameraBoundaries(newWorldPointCameraCenter);

            Log("finalPosition:" + finalPosition);
            Log("finalRotation:" + finalRotation);
            Log("newWorldPointCameraCenter:" + newWorldPointCameraCenter);
            Log("newWorldPointCameraCenterClamped:" + newWorldPointCameraCenterClamped);

            finalOffset = newWorldPointCameraCenterClamped;

            Log("finalOffset:" + finalOffset);
            Log("FinalPosition:" + finalPosition);
            Log("FinalRotation:" + FinalRotation);
            Log("FinalDistance:" + FinalDistance);


            if (isFocusingOrFollowing)
            {
                HandleFocusAndFollow();
            }
            if (anyInteraction)
            {
                CalculateInertia();
            }
            else
            {
                ApplyInertia();
            }
            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);

            // Apply Edge Boundaries
            if (IsUsingCameraEdgeBoundaries())
            {
                finalPosition = ClampCameraCorners(finalPosition, out bool clampApplied);
                finalOffset = CalculateOffset(finalPosition, finalRotation, finalDistance, groundHeight);
                if (clampApplied) CalculateInertia();
            }

            Log("finalOffset:" + finalOffset);
            Log("FinalPosition:" + finalPosition);
            Log("FinalRotation:" + FinalRotation);
            Log("FinalDistance:" + FinalDistance);


            ApplyToCamera();

        }



        /// <summary>
        /// Converts a distance from ground to a camera orthographic size
        /// </summary>
        /// <param name="d"></param>
        public void SetSizeByDistance(float d)
        {
            finalSize = Mathf.Clamp(d * distanceToSize, sizeMinMax.x, sizeMinMax.y);
        }

        /// <summary>
        /// Converts the orthographic size to a distance from ground
        /// </summary>
        /// <returns></returns>
        public float GetDistanceFromSize()
        {
            return finalSize / distanceToSize;
        }

        /// <summary>
        /// In case the camera is standalone (no CameraModeSwitcher) then this is apply 
        /// the position and rotation to the camera
        /// </summary>
        override protected void ApplyToCamera()
        {
            if (!standalone)
                return;

            base.ApplyToCamera();

            cam.orthographicSize = finalSize;
        }

        /// <summary>
        /// Return the matrix of the camera transform, in order to blend it when switching modes
        /// </summary>
        /// <returns></returns>
        override public Matrix4x4 GetMatrix()
        {
            float aspect = cam.aspect;
            float near = cam.nearClipPlane, far = cam.farClipPlane;
            finalSize = Mathf.Clamp(finalSize, sizeMinMax.x, sizeMinMax.y);
            return Matrix4x4.Ortho(-finalSize * aspect, finalSize * aspect, -finalSize, finalSize, near, far);
        }

        /// <summary>
        /// Converts a camera offset on ground to a camera position
        /// </summary>
        virtual public void SetPositionByOffset()
        {
            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);
        }

        /// <summary>
        /// Set the initial values for the reset function
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="rotation"></param>
        /// <param name="distanceOrSize"></param>
        override public void SetResetValues(Vector3 offset, Quaternion rotation, float size)
        {
            initOffset = offset;
            initRotation = rotation;
            initSize = size;
        }




        #region FOLLOW & FOCUS
        /// <summary>
        /// Setup the camera move animation
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="changeDistance"></param>
        /// <param name="targetDistanceOrSize"></param>
        /// <param name="changeRotation"></param>
        /// <param name="targetRotation"></param>
        /// <param name="allowYOffsetFromGround"></param>
        /// <param name="instant"></param>
        override protected void FocusCamera(Vector3 targetPosition,
           bool changeDistance, float targetDistanceOrSize,
           bool changeRotation, Quaternion targetRotation,
           bool allowYOffsetFromGround = false,
           bool instant = false)
        {

            if (!instant)
            {
                followMoveOffset.Reset(finalOffset);
                followMoveDistanceOrSize.Reset(finalSize);
                followMoveRotation.Reset(finalRotation);
            }
            else
            {
                followMoveOffset.Reset(targetPosition);
                followMoveDistanceOrSize.Reset(targetDistanceOrSize);
                followMoveRotation.Reset(targetRotation);
            }

            base.FocusCamera(targetPosition, changeDistance, targetDistanceOrSize,
                changeRotation, targetRotation, allowYOffsetFromGround, instant);
        }


        /// <summary>
        /// Focus the camera on a GameObject (distance animation)
        /// </summary>
        /// <param name="go">The gameObject to get closer to</param>
        /// <param name="allowYOffsetFromGround">Allow offseting the camera from the ground to match the object's pivot y position and height</param>
        override public void FocusCameraOnGameObject(GameObject go, bool allowYOffsetFromGround = false)
        {
            followMoveOffset.Reset(finalOffset);
            followMoveDistanceOrSize.Reset(finalSize);

            base.FocusCameraOnGameObject(go, allowYOffsetFromGround);
        }

        /// <summary>
        /// Follow a game object
        /// </summary>
        /// <param name="go">The game object to follow</param>
        /// <param name="doFocus">Also focus on it (distance animation)</param>
        /// <param name="allowYOffsetFromGround">Allow offseting the camera from the ground to match the object's pivot y position and height</param>
        override public void FollowGameObject(GameObject go, bool doFocus, bool allowYOffsetFromGround = false)
        {
            followMoveOffset.Reset(finalOffset);
            followMoveDistanceOrSize.Reset(finalSize);

            base.FollowGameObject(go, doFocus, allowYOffsetFromGround);
        }


        /// <summary>
        /// Handle the camera focus/follow/moveto
        /// </summary>
        virtual protected void HandleFocusAndFollow()
        {
            if (!isFocusingOrFollowing)
                return;

            if (focusTargetGo != null)
            {
                Bounds b = focusTargetGo.GetBoundsRecursive();

                if (b.size == Vector3.zero && b.center == Vector3.zero)
                    return;

                // offseting the bounding box
                if (allowYOffsetFromGround)
                {
                    float yOffset = b.center.y;
                    b.extents = b.extents.SetY(b.extents.y + yOffset);
                    b.center = b.center.SetY(groundHeight);
                }
                Vector3 max = b.size;
                // Get the radius of a sphere circumscribing the bounds
                float radius = max.magnitude * followRadiusMultiplier;

                focusTargetPosition = b.center;
                focusTargetDistanceOrSize = Mathf.Clamp(radius, sizeMinMax.x, sizeMinMax.y);

            }

            if (enableFocusing)
            {
                finalSize = followMove.Update(ref followMoveDistanceOrSize, focusTargetDistanceOrSize);
            }
            if (enableRotationChange)
            {
                finalRotation = followMove.Update(ref followMoveRotation, focusTargetRotation);
                currentPitch = finalRotation.eulerAngles.x;
                currentYaw = finalRotation.eulerAngles.y;
            }
            finalOffset = worldPointCameraCenter = followMove.Update(ref followMoveOffset, focusTargetPosition, OnFollowFocusCompleted);

            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);

        }

        protected void OnFollowFocusCompleted()
        {
            StopFollow();
            Log("OnFollowFocusCompleted");
            CameraEvents.OnFocusComplete?.Invoke(focusTargetGo);
        }
        #endregion

    }
}
