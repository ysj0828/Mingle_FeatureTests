using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MingleMain;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class ImageOnObjectManager : MonoBehaviour
{
  // reference via Inspector if possible
  [SerializeField] private Camera mainCamera;
  [SerializeField] private string LayerToUse;
  private static GameObject TextCameraParent = null;
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
    var camera = new GameObject("TextCamera").AddComponent<Camera>();

    if (TextCameraParent == null)
    {
      TextCameraParent = new GameObject("TextCameraParent");
      TextCameraParent.transform.position = new Vector3(0, -10, 0);
    }
    camera.transform.SetParent(TextCameraParent.transform, false);
    camera.transform.localPosition = new Vector3(3.5f * -1, 0, 0);
    camera.backgroundColor = new Color(0, 0, 0, 0);
    camera.clearFlags = CameraClearFlags.Color;
    camera.cullingMask = 1 << LayerMask.NameToLayer(LayerToUse);
    camera.farClipPlane = 3f;

    // make it render to the renderTexture
    camera.targetTexture = renderTexture;
    camera.forceIntoRenderTexture = true;

    // 6. add the UI to your scene as child of the camera
    var Canvas = new GameObject("Canvas", typeof(RectTransform)).AddComponent<Canvas>();
    Canvas.transform.SetParent(camera.transform, false);
    Canvas.transform.localScale = new Vector3(1, 2, 1);
    Canvas.gameObject.AddComponent<CanvasScaler>();
    Canvas.renderMode = RenderMode.WorldSpace;
    var canvasRectTransform = Canvas.GetComponent<RectTransform>();
    canvasRectTransform.anchoredPosition3D = new Vector3(0, 0, 3);
    canvasRectTransform.sizeDelta = Vector2.one;

    var Image = new GameObject("Text", typeof(RectTransform)).AddComponent<RawImage>();
    Image.transform.SetParent(Canvas.transform, false);
    var textRectTransform = Image.GetComponent<RectTransform>();
    textRectTransform.localScale = Vector3.one;
    textRectTransform.sizeDelta = new Vector2(1, 1);

    // var Texture = new Texture2D(180, 180);
    // TextAsset bindata = Resources.Load("RequestFriend") as TextAsset;
    // var Bytes = Resources.Load("RequestFriend").bytes;
    // Texture.LoadImage(bindata.bytes);

    Texture2D texture = new Texture2D(0, 0);
    string PATH = "RequestFriend";    // 이미지 파일 패스를 써준다.    //중요한 것은 유니티 프로젝트 Assets/Resource/ 폴더 이후의 경로를 써주는 것이다. 이 폴더는 맘대로 바꿀 수가 없다.  
    texture = Resources.Load(PATH, typeof(Texture2D)) as Texture2D;  // 이미지 로드   
    Image.texture = texture;  // 타겟 오브젝트에 메인 텍스쳐를 넣어준다. 

    // _title.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
    // _title.fontStyle = FontStyle.Bold;
    // _title.alignment = TextAnchor.MiddleCenter;
    // _title.color = Color.gray;
    // _title.fontSize = 300;
    // _title.horizontalOverflow = HorizontalWrapMode.Wrap;
    // _title.verticalOverflow = VerticalWrapMode.Overflow;

    Canvas.gameObject.layer = LayerMask.NameToLayer(LayerToUse);
    Image.gameObject.layer = LayerMask.NameToLayer(LayerToUse);

    // 7. finally assign the material to the child object and hope everything works ;)
    innerObject.GetComponent<MeshRenderer>().material = textMaterial;
  }

}