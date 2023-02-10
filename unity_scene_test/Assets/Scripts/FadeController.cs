using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeController : MonoBehaviour
{
    [SerializeField] Toggle toggle;

    [SerializeField] LayerMask layermask;

    [SerializeField] Transform target;

    [SerializeField] Camera camera;
    [SerializeField] Transform originT;

    [SerializeField] [Range(0, 1f)] float fadeAlpha = 0.33f;

    [SerializeField] bool retainShadows = true;

    [SerializeField] Vector3 targetPosOffset = Vector3.up;

    [SerializeField] float fadeSpeed = 1f;

    [Header("Read only")]
    [SerializeField]
    List<FadeObjects> objectsBlockingView = new List<FadeObjects>();
    Dictionary<FadeObjects, Coroutine> runningCoroutines = new Dictionary<FadeObjects, Coroutine>();

    RaycastHit[] Hits = new RaycastHit[10];

    RaycastHit[] Hits2 = new RaycastHit[10];

    bool coroutineStart;

    // Start is called before the first frame update
    //void Start()
    //{
    //    StartCoroutine(CheckForObjects());
    //}

    private void OnEnable()
    {
        StartCoroutine(CheckForObjects());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        ResetAlpha();
    }

    void ResetAlpha()
    {
        
        foreach(FadeObjects objects in objectsBlockingView)
        {
            foreach(Material mat in objects.materials)
            {
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt("_ZWrite", 1);
                mat.SetInt("_Surface", 0);

                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

                mat.SetShaderPassEnabled("DepthOnly", true);
                mat.SetShaderPassEnabled("SHADOWCASTER", true);

                mat.SetOverrideTag("RenderType", "Opaque");

                mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");

                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, objects.initialAlpha);
            }
        }
    }

    IEnumerator CheckForObjects()
    {
        while (toggle.isOn)
        {
            int hits = Physics.RaycastNonAlloc(
                //origin: camera.transform.position,
                origin: originT.position,
                direction: (target.transform.position + targetPosOffset - originT.position).normalized,
                //direction: (target.transform.position + targetPosOffset - camera.transform.position).normalized,
                results: Hits,
                maxDistance: Vector3.Distance(originT.position, target.transform.position + targetPosOffset),
                //maxDistance: Vector3.Distance(camera.transform.position, target.transform.position + targetPosOffset),
                layerMask: layermask
                );

            if (hits > 0)
            {
                for (int i = 0; i < hits; i++)
                {
                    FadeObjects fadeObject = GetFadingObjectFromHit(Hits[i]);

                    if (fadeObject != null && !objectsBlockingView.Contains(fadeObject))
                    {
                        if (runningCoroutines.ContainsKey(fadeObject))
                        {
                            if (runningCoroutines[fadeObject] != null)
                            {
                                StopCoroutine(runningCoroutines[fadeObject]);
                            }

                            runningCoroutines.Remove(fadeObject);
                            //Debug.Log("CheckForObject() remove");
                        }

                        runningCoroutines.Add(fadeObject, StartCoroutine(FadeObjectOut(fadeObject)));
                        objectsBlockingView.Add(fadeObject);
                    }
                }
            }

            FadeObjectsNoLongerBeingHit();

            ClearHits();

            yield return null;
        }
    }

    private void FadeObjectsNoLongerBeingHit()
    {
        List<FadeObjects> objectsToRemove = new List<FadeObjects>(objectsBlockingView.Count);

        foreach(FadeObjects fadeObjects in objectsBlockingView)
        {
            bool objectBeingHit = false;

            for (int i = 0; i < Hits.Length; i++)
            {
                FadeObjects hitObject = GetFadingObjectFromHit(Hits[i]);
                if (hitObject != null && fadeObjects == hitObject)
                {
                    objectBeingHit = true;
                    break;
                }
            }

            if (!objectBeingHit)
            {
                if (runningCoroutines.ContainsKey(fadeObjects))
                {
                    if (runningCoroutines[fadeObjects] != null)
                    {
                        StopCoroutine(runningCoroutines[fadeObjects]);
                    }
                    runningCoroutines.Remove(fadeObjects);

                    //Debug.Log("FadeObjectsNoLongerBeingHit() remove");
                }

                runningCoroutines.Add(fadeObjects, StartCoroutine(FadeObjectIn(fadeObjects)));
                objectsToRemove.Add(fadeObjects);
            }
        }

        foreach(FadeObjects removeObject in objectsToRemove)
        {
            objectsBlockingView.Remove(removeObject);
        }
    }

    private void ClearHits()
    {
        System.Array.Clear(Hits, 0, Hits.Length);
    }

    private FadeObjects GetFadingObjectFromHit(RaycastHit hit)
    {
        return hit.collider != null ? hit.collider.GetComponent<FadeObjects>() : null;
    }


    IEnumerator FadeObjectOut(FadeObjects fadeObjects)
    {
        foreach (Material mat in fadeObjects.materials)
        {
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.SetInt("_Surface", 1);

            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            mat.SetShaderPassEnabled("DepthOnly", false);
            mat.SetShaderPassEnabled("SHADOWCASTER", retainShadows);

            mat.SetOverrideTag("RenderType", "Transparent");

            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

            mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        float time = 0;

        while(fadeObjects.materials[0].color.a > fadeAlpha)
        {
            foreach(Material mat in fadeObjects.materials)
            {
                if (mat.HasProperty("_BaseColor"))
                {
                    mat.color = new Color(
                        mat.color.r,
                        mat.color.g,
                        mat.color.b,
                        Mathf.Lerp(fadeObjects.initialAlpha, fadeAlpha, time * fadeSpeed)
                        );
                }
            }
            
            time += Time.deltaTime;

            yield return null;
        }

        if (runningCoroutines.ContainsKey(fadeObjects))
        {
            StopCoroutine(runningCoroutines[fadeObjects]);
            runningCoroutines.Remove(fadeObjects);
            //Debug.Log("FadeObjectOut() remove");
        }
    }


    IEnumerator FadeObjectIn(FadeObjects fadeObjects)
    {
        float time = 0;
        while (fadeObjects.materials[0].color.a < fadeObjects.initialAlpha)
        {
            foreach (Material mat in fadeObjects.materials)
            {
                if (mat.HasProperty("_BaseColor"))
                {
                    mat.color = new Color(
                        mat.color.r,
                        mat.color.g,
                        mat.color.b,
                        Mathf.Lerp(fadeAlpha, fadeObjects.initialAlpha, time * fadeSpeed)
                        );
                }
            }

            time += Time.deltaTime;

            yield return null;
        }

        foreach (Material mat in fadeObjects.materials)
        {
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.SetInt("_Surface", 0);

            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

            mat.SetShaderPassEnabled("DepthOnly", true);
            mat.SetShaderPassEnabled("SHADOWCASTER", true);

            mat.SetOverrideTag("RenderType", "Opaque");

            mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");

            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        if (runningCoroutines.ContainsKey(fadeObjects))
        {
            StopCoroutine(runningCoroutines[fadeObjects]);
            runningCoroutines.Remove(fadeObjects);
            //Debug.Log("FadeObjectIn() remove");
        }
    }
}
