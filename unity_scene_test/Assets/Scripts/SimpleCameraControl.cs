using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimpleCameraControl : MonoBehaviour
{
    [SerializeField] Camera mainCam;

    [SerializeField] GameObject Avatar;
    Animator animator;
    
    NavMeshAgent agent;

    [SerializeField] TMP_Dropdown Animdropdown;
    [SerializeField] TMP_Dropdown Emodropdown;

    [SerializeField] AnimatorOverrideController[] overrideControllers;

    [SerializeField] TextMeshProUGUI t1;

    int sensitivity = 5;

    float cameramovesens = 0.05f;



    bool startAnim;
    bool startEmo;

    bool anim;
    bool emo;

    bool playOnce;

    string currentAnim;
    string currentEmo;

    bool customanim;

    private void Start()
    {
        animator = Avatar.GetComponent<Animator>();
        agent = Avatar.GetComponent<NavMeshAgent>();
        agent.ResetPath();

        Animdropdown.onValueChanged.AddListener(delegate { AnimValueChanged(Animdropdown); });
        Emodropdown.onValueChanged.AddListener(delegate { EmoValueChanged(Emodropdown);	});
    }

    void AnimValueChanged(TMP_Dropdown drop)
    {
        startAnim = true;
    }

    void EmoValueChanged(TMP_Dropdown drop)
    {
        startEmo = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            startAnim = false;
            startEmo = false;
            anim = false;
            emo = false;
            customanim = true;

            t1.text = "Custom";
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            startAnim = true;
            startEmo = false;
            anim = true;
            emo = false;
            customanim = false;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            startEmo = true;
            startAnim = false;
            emo = true;
            anim = false;
            customanim = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            startAnim = false;
            startEmo = false;
            emo = false;
            anim = false;
            customanim = false;
            animator.Play("StopAll");
            t1.text = "stopped";
        }

        if (startAnim)
        {
            t1.text = "animation";

            switch (Animdropdown.value)
            {
                case 0:
                    animator.Play("idle01");
                    currentAnim = "idle01";
                    startAnim = false;
                    break;

                case 1:
                    animator.Play("idle02");
                    currentAnim = "idle02";
                    startAnim = false;
                    break;

                case 2:
                    animator.Play("idle03");
                    currentAnim = "idle03";
                    startAnim = false;
                    break;

                case 3:
                    animator.Play("idle_B");
                    currentAnim = "idle_B";
                    startAnim = false;
                    break;

                case 4:
                    animator.Play("walk_B");
                    currentAnim = "walk_B";
                    startAnim = false;
                    break;

                case 5:
                    animator.Play("interaction01");
                    currentAnim = "interaction01";
                    startAnim = false;
                    break;
            }
        }

        if (startEmo)
        {
            t1.text = "emotion";

            switch (Emodropdown.value)
            {
                case 0:
                    animator.Play("E_Crying_L");
                    currentEmo = "E_Crying_L";
                    startEmo = false;
                    break;

                case 1:
                    animator.Play("E_Cool_O");
                    currentEmo = "E_Cool_O";
                    startEmo = false;
                    break;

                case 2:
                    animator.Play("E_Blushing_O");
                    currentEmo = "E_Blushing_O";
                    startEmo = false;
                    break;

                case 3:
                    animator.Play("E_Angry_O");
                    currentEmo = "E_Angry_O";
                    startEmo = false;
                    break;

                case 4:
                    animator.Play("E_Cussing_O");
                    currentEmo = "E_Cussing_O";
                    startEmo = false;
                    break;

                case 5:
                    animator.Play("E_HeartEyes_O");
                    currentEmo = "E_HeartEyes_O";
                    startEmo = false;
                    break;

                case 6:
                    animator.Play("E_Laughing_L");
                    currentEmo = "E_Laughing_L";
                    startEmo = false;
                    break;

                case 7:
                    animator.Play("E_NoseStream_O");
                    currentEmo = "E_NoseStream_O";
                    startEmo = false;
                    break;

                case 8:
                    animator.Play("E_Silly_L");
                    currentEmo = "E_Silly_L";
                    startEmo = false;
                    break;

                case 9:
                    animator.Play("E_Sleeping_L");
                    currentEmo = "E_Sleeping_L";
                    startEmo = false;
                    break;

                case 10:
                    animator.Play("E_UwU_O");
                    currentEmo = "E_UwU_O";
                    startEmo = false;
                    break;
            }
        }

        if (agent.remainingDistance <= agent.stoppingDistance + 0.01f)
        {
            agent.ResetPath();

            if (!anim && !emo && !customanim)
            {
                animator.Play("StopAll");
            }

            else if (anim)
            {
                animator.Play(currentAnim);
            }

            else if (emo)
            {
                animator.Play(currentEmo);
            }

            else if (customanim)
            {
                animator.Play("Custom");
            }
        }

        if (agent.remainingDistance > agent.stoppingDistance + 0.01f)
        {
            animator.Play("walk");
        }

        if (Input.GetMouseButtonDown(0))
        {
            //move avatar
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, maxDistance: Mathf.Infinity, layerMask: 1 << 6) && !isPointerOverUI())
            {
                agent.SetDestination(hit.point);
            }
        }

        if (Input.GetMouseButton(1))
        {
            //rotate camera
            float Y = Input.GetAxis("Mouse Y") * sensitivity;
            float X = Input.GetAxis("Mouse X") * sensitivity;

            mainCam.transform.eulerAngles += new Vector3(-Y, X, 0);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            cameramovesens = 0.15f;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            cameramovesens = 0.05f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            mainCam.transform.position += mainCam.transform.forward * cameramovesens;
        }

        if (Input.GetKey(KeyCode.A))
        {
            mainCam.transform.position -= mainCam.transform.right * cameramovesens;
        }

        if (Input.GetKey(KeyCode.S))
        {
            mainCam.transform.position -= mainCam.transform.forward * cameramovesens;
        }

        if (Input.GetKey(KeyCode.D))
        {
            mainCam.transform.position += mainCam.transform.right * cameramovesens;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            mainCam.transform.position -= mainCam.transform.up * cameramovesens;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            mainCam.transform.position += mainCam.transform.up * cameramovesens;
        }
    }

    bool isPointerOverUI()
    {
        Vector2 mousePos = Input.mousePosition;

        PointerEventData eventData = new PointerEventData(EventSystem.current);

        eventData.position = new Vector2(mousePos.x, mousePos.y);

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
