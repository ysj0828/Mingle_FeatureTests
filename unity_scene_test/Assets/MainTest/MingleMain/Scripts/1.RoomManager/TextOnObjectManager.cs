using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MingleMain;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class TextOnObjectManager : MonoBehaviour
{
  // reference via Inspector if possible
  [SerializeField] private Camera mainCamera;
  [SerializeField] private string LayerToUse;
  private static int Count = 0;
  private static GameObject TextCameraParent = null;
  private Camera TextCamera;
  private Text _title;
  private RoomSphere _room_sphere;

  private void OnDestroy()
  {
    Destroy(TextCamera.gameObject);
  }
  private void Awake()
  {
    // return;
    // 0. make the clone of this and make it a child
    var innerObject = new GameObject(name + "_original", typeof(MeshRenderer)).AddComponent<MeshFilter>();
    innerObject.transform.SetParent(transform);
    innerObject.transform.localPosition = Vector3.zero;
    innerObject.transform.localScale = Vector3.one * 1.01f;
    innerObject.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
    // copy over the mesh
    innerObject.mesh = GetComponent<MeshFilter>().mesh;
    innerObject.name = name + "_textDecal";

    // 1. Create and configure the RenderTexture
    var renderTexture = new RenderTexture(2048, 2048, 24) { name = name + "_RenderTexture" };

    // 2. Create material
    // var textMaterial = new Material(Shader.Find("UI/Unlit/Text"));
    var textMaterial = new Material(Shader.Find("Mobile/Particles/Multiply"));
    // var textMaterial = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default"));

    // assign the new renderTexture as Albedo
    textMaterial.SetTexture("_MainTex", renderTexture);

    // set RenderMode to Fade
    textMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
    textMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
    textMaterial.SetInt("_ZWrite", 0);
    textMaterial.DisableKeyword("_ALPHATEST_ON");
    textMaterial.EnableKeyword("_ALPHABLEND_ON");
    textMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
    textMaterial.renderQueue = 3000;

    // 3. WE CAN'T CREATE A NEW LAYER AT RUNTIME SO CONFIGURE THEM BEFOREHAND AND USE LayerToUse

    // 4. exclude the Layer in the normal camera
    if (!mainCamera) mainCamera = Camera.main;
    mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer(LayerToUse));

    // 5. Add new Camera as child of this object
    TextCamera = new GameObject("TextCamera").AddComponent<Camera>();

    if (TextCameraParent == null)
    {
      TextCameraParent = new GameObject("TextCameraParent");
      TextCameraParent.transform.position = new Vector3(0, -10, 0);
    }
    TextCamera.transform.SetParent(TextCameraParent.transform, false);
    TextCamera.transform.localPosition = new Vector3(3.5f * Count++, 0, 0);
    TextCamera.backgroundColor = new Color(0, 0, 0, 0);
    TextCamera.clearFlags = CameraClearFlags.Color;
    TextCamera.cullingMask = 1 << LayerMask.NameToLayer(LayerToUse);
    TextCamera.farClipPlane = 3f;

    // make it render to the renderTexture
    TextCamera.targetTexture = renderTexture;
    TextCamera.forceIntoRenderTexture = true;

    // 6. add the UI to your scene as child of the camera
    var Canvas = new GameObject("Canvas", typeof(RectTransform)).AddComponent<Canvas>();
    Canvas.transform.SetParent(TextCamera.transform, false);
    Canvas.transform.localScale = new Vector3(1, 2, 1);
    Canvas.gameObject.AddComponent<CanvasScaler>();
    Canvas.renderMode = RenderMode.WorldSpace;
    var canvasRectTransform = Canvas.GetComponent<RectTransform>();
    canvasRectTransform.anchoredPosition3D = new Vector3(0, 0, 3);
    canvasRectTransform.sizeDelta = Vector2.one;

    _title = new GameObject("Text", typeof(RectTransform)).AddComponent<Text>();
    _title.transform.SetParent(Canvas.transform, false);
    var textRectTransform = _title.GetComponent<RectTransform>();
    textRectTransform.localScale = Vector3.one * 0.001f;
    textRectTransform.sizeDelta = new Vector2(2000, 1000);

    _title.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
    _title.fontStyle = FontStyle.Bold;
    _title.alignment = TextAnchor.MiddleCenter;
    _title.color = Color.gray;
    _title.fontSize = 300;
    _title.horizontalOverflow = HorizontalWrapMode.Wrap;
    _title.verticalOverflow = VerticalWrapMode.Overflow;

    Canvas.gameObject.layer = LayerMask.NameToLayer(LayerToUse);
    _title.gameObject.layer = LayerMask.NameToLayer(LayerToUse);

    _room_sphere = gameObject.GetComponent<RoomSphere>();
    // text.text = "채팅방 " + Count;

    // 7. finally assign the material to the child object and hope everything works ;)
    innerObject.GetComponent<MeshRenderer>().material = textMaterial;
  }

  private void FixedUpdate()
  {
    // if (_title.text != _room_sphere.Title) _title.text = _room_sphere.Title;
    var title = _room_sphere.Title;// + "[" + _room_sphere.MsgCnt + "]";
    if (_title.text != title) _title.text = title;
  }

}