using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionLight : MonoBehaviour
{
  // Start is called before the first frame update
  Material _emission_material;
  Light _light;
  Color _color;
  void Start()
  {
    InitColor();
    InitMaterial();
    InitLight();
  }

  void InitColor()
  {
    _color = Color.HSVToRGB(Random.Range(0f, 1f), 1f, 1f);
  }
  void InitMaterial()
  {
    _emission_material = new Material(new Material(Shader.Find("Universal Render Pipeline/Lit")));
    GetComponent<Renderer>().material = _emission_material;
    // _emission_material.SetColor("_BaseColor", Color.White);
    _emission_material.SetColor("_BaseColor", _color);
    _emission_material.SetFloat("_Smoothness", 1f);
    _emission_material.EnableKeyword("_EMISSION");
    _emission_material.SetColor("_EmissionColor", _color);

    // _emission_material.SetColor("_EmissionColor", _color * 2f);
    // _emission_material.SetColor("_EmissionColor", new Color(_color.r * 2f, _color.g * 2f, _color.b * 2f));
    // _emission_material.SetColor("_EmissionColor", ((Vector4)_color) * Mathf.Exp(2f, 2f));
  }
  void InitLight()
  {
    _light = transform.gameObject.AddComponent<Light>();
    _light.color = _color;
    _light.intensity = transform.localScale.x / 2;
    // Debug.Log(1 << LayerMask.NameToLayer("Profile"));
    _light.cullingMask &= ~(1 << LayerMask.NameToLayer("Profile"));
    _light.cullingMask &= ~(1 << LayerMask.NameToLayer("SelectCharacter"));
    _light.cullingMask &= ~(1 << LayerMask.NameToLayer("SelectRoom"));
    // Debug.Log(_light.cullingMask);

  }

  // Update is called once per frame
  void Update()
  {
    // float emission = Mathf.PingPong(Time.time, 3.0f);
    // Color baseColor = _color;
    // Color finalColor = baseColor * (1 + emission);
    // _emission_material.SetColor("_EmissionColor", finalColor);
  }

  void FixedUpdate()
  {
    UpdateIntensity();
  }

  void UpdateIntensity()
  {
    if (_light.intensity != transform.localScale.x / 2) _light.intensity = transform.localScale.x / 2;
  }
}
