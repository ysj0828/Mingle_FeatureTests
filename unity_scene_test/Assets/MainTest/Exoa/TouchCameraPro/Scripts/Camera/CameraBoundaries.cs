﻿using UnityEngine;

namespace Exoa.Cameras
{
    public class CameraBoundaries : MonoBehaviour
    {
        public enum Type { Rectangle, Circle };
        public enum Mode { CameraCenter, CameraEdges };
        public Type type;

        public Collider bounderiesCollider;
        public SphereCollider sphereCollider;
        public BoxCollider boxCollider;
        public bool drawGizmos;
        //private MeshRenderer debugMr;
        private Bounds bounds;

        public Mode mode;
        [Range(0.001f, .999f)]
        public float edgeElasticity = .9f;

        /// <summary>
        /// Clamp any given points inside the defined boundaries
        /// </summary>
        /// <param name="p"></param>
        /// <param name="isInBoundaries"></param>
        /// <param name="groundHeight"></param>
        /// <returns></returns>
        public Vector3 ClampInBoundsXZ(Vector3 p, out bool isInBoundaries, float groundHeight)
        {
            isInBoundaries = true;

            if (type == Type.Rectangle && boxCollider == null)
                return p;

            if (type == Type.Circle && sphereCollider == null)
                return p;


            if (type == Type.Rectangle)
            {
                if (boxCollider.enabled)
                {
                    bounds = boxCollider.bounds;
                    boxCollider.enabled = false;
                }
                bounds.center = bounds.center.SetY(groundHeight);
                bounds.size = bounds.size.SetY(0);


                if (bounds.Contains(p.SetY(groundHeight)))
                    return p;
                isInBoundaries = false;
                return bounds.ClosestPoint(p).SetY(p.y);
            }
            else if (type == Type.Circle)
            {
                Vector3 globalCenter = sphereCollider.transform.TransformPoint(sphereCollider.center).SetY(groundHeight);
                float globalMagnitude = sphereCollider.transform.TransformVector(sphereCollider.radius * Vector3.up).magnitude;
                Vector3 centerToPoint = (p - globalCenter).SetY(0);
                if (centerToPoint.magnitude <= globalMagnitude)
                    return p;
                isInBoundaries = false;
                return (globalCenter + centerToPoint.normalized * globalMagnitude).SetY(p.y);
            }
            return p;
        }

        /// <summary>
        /// Clamp any given points inside the defined boundaries
        /// </summary>
        /// <param name="p"></param>
        /// <param name="isInBoundaries"></param>
        /// <param name="groundHeight"></param>
        /// <returns></returns>
        public Vector3 ClampInBoundsXY(Vector3 p, out bool isInBoundaries, float groundHeight)
        {
            isInBoundaries = true;

            if (type == Type.Rectangle && boxCollider == null)
                return p;

            if (type == Type.Circle && sphereCollider == null)
                return p;


            if (type == Type.Rectangle)
            {
                if (boxCollider.enabled)
                {
                    bounds = boxCollider.bounds;
                    boxCollider.enabled = false;
                }
                bounds.center = bounds.center.SetZ(groundHeight);
                bounds.size = bounds.size.SetZ(0);


                if (bounds.Contains(p.SetZ(groundHeight)))
                    return p;
                isInBoundaries = false;
                return bounds.ClosestPoint(p).SetZ(p.z);
            }
            else if (type == Type.Circle)
            {
                Vector3 globalCenter = sphereCollider.transform.TransformPoint(sphereCollider.center).SetZ(groundHeight);
                float globalMagnitude = sphereCollider.transform.TransformVector(sphereCollider.radius * Vector3.up).magnitude;
                Vector3 centerToPoint = (p - globalCenter).SetZ(0);
                if (centerToPoint.magnitude <= globalMagnitude)
                    return p;
                isInBoundaries = false;
                return (globalCenter + centerToPoint.normalized * globalMagnitude).SetZ(p.z);
            }
            return p;
        }



        void OnDrawGizmos()
        {
            //print("bounderiesCollider:" + bounderiesCollider);
            if (!drawGizmos)
                return;

            if (type == Type.Rectangle && boxCollider == null)
                return;

            if (type == Type.Circle && sphereCollider == null)
                return;


            Color c = Color.yellow;
            c.a = .5f;
            Gizmos.color = c;

            if (type == Type.Rectangle && bounderiesCollider is BoxCollider)
            {
                Bounds b = boxCollider.bounds;
                b.center = bounds.center.SetY(0);
                b.size = bounds.size.SetY(0);
                Gizmos.matrix = boxCollider.transform.localToWorldMatrix;
                Gizmos.DrawCube(b.center, b.size);
            }
            else if (type == Type.Circle && bounderiesCollider is SphereCollider)
            {
                Gizmos.matrix = sphereCollider.transform.localToWorldMatrix;
                Gizmos.DrawSphere(sphereCollider.center, sphereCollider.radius);
            }
            /*if (debugMr != null)
            {
                debugMr.transform.position = ClampPointsXZ(debugMr.transform.position, out bool isInBoundaries);
                debugMr.sharedMaterial.color = isInBoundaries ? Color.green : Color.red;
            }*/
        }
    }
}
