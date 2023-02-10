using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObjects : MonoBehaviour, IEquatable<FadeObjects>
{
    public List<Renderer> renderers = new List<Renderer>();
    public Vector3 position;
    public List<Material> materials = new List<Material>();

    [HideInInspector]
    public float initialAlpha;


    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;

        if (renderers.Count == 0)
        {
            renderers.AddRange(GetComponentsInChildren<Renderer>());
        }

        foreach (Renderer renderer in renderers)
        {
            materials.AddRange(renderer.materials);
        }

        for (int i = 0; i < materials.Count; i++)
        {
            initialAlpha = materials[0].color.a;
        }

        
    }


    public bool Equals(FadeObjects other)
    {
        return position.Equals(other.position);
    }

    public override int GetHashCode()
    {
        return position.GetHashCode();
    }
}
