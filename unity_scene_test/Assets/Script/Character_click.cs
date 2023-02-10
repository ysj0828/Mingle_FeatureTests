using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Character_click : MonoBehaviour
{
    public List<GameObject> Character;
    public List<Button> Character_button;
    Animator anim;
    List<AnimationClip> clips = new List<AnimationClip>();

    string[] current_animation = { " " };
    string[] last_animation = { " " };
    int _current_char = 0;

    private void Awake()
    {

        Character[0].SetActive(true);
        for (int i = 1; i < Character.Count; i++)
        {
            Character[i].SetActive(false);
        }
        for (int i = 0; i < Character_button.Count; i++)
        {
            Character_button[i].gameObject.SetActive(false);
        }
    }
    
    private void Update()
    {
        OutClick();
        AnimReset();
    }
    private void Start()
    {
        
        anim = Character[_current_char].GetComponent<Animator>();

        for (int i = 0; i < anim.runtimeAnimatorController.animationClips.Length; i++)
        {
            //Clip배열의 순서는 animator에 넣는 순서로 정해짐 . idle은 무조건 Clip[0](첫번째)여야함 
            clips.Add(anim.runtimeAnimatorController.animationClips[i]);
        }
        Debug.Log(anim.runtimeAnimatorController.animationClips.Length);
    }

    void OutClick()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //마우스 포인트 근처 좌표를 만든다.

        if (Input.GetMouseButtonDown(0))
        {
            if ((EventSystem.current.IsPointerOverGameObject(-1) == false)) //mobile = 0 window = -1
            {
                if (false == (Physics.Raycast(origin: ray.origin, direction: ray.direction * 10, hitInfo: out hit, maxDistance: 15, layerMask: 1 << 7)) || hit.transform.tag == "ground")
                {
                    Debug.Log("Out Click ! ");
                    for (int i = 0; i < Character_button.Count; i++)
                    {
                        Character_button[i].gameObject.SetActive(false);
                    }
                }

                if (Physics.Raycast(origin: ray.origin, direction: ray.direction * 10, hitInfo: out hit, maxDistance: 15, layerMask: 1 << 6))
                {
                    for (int i = 0; i < Character_button.Count; i++)
                    {
                        Character_button[i].gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    public void updateCha(int index)
    {
        if (Character.Count <= index)
        {
            Debug.LogError("_character.Count error");
            return;
        }
        _current_char = index;

        for (int i = 0; i < Character.Count; i++)
        {
            if (_current_char == i)
            {
                Character[i].SetActive(true);
            }
            else
            {
                Character[i].SetActive(false);
            }

        }
        updateAnime();
    }

    public void Ani_SetInteger(int change_num)
    {
        anim.SetInteger("animation", change_num);
    }

    void updateAnime()
    {
        anim = Character[_current_char].GetComponent<Animator>();
        Ani_SetInteger(Random.Range(1, anim.runtimeAnimatorController.animationClips.Length));

        for (int i = 0; i < anim.runtimeAnimatorController.animationClips.Length; i++)
        {
            if (anim.GetInteger("animation") == i)
            {
                char split_text = ' ';
                current_animation = clips[i].ToString().Split(split_text);

                if (current_animation[0] != last_animation[0])
                {
                    last_animation[0] = current_animation[0];
                    anim.Play(current_animation[0]);
                    if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f) anim.CrossFade(current_animation[0], 0.1f);
                }
                else
                {
                    updateAnime();
                }
            }
        }
    }

    void AnimReset()
    {
        float length = 1.0f;
        anim = Character[_current_char].GetComponent<Animator>();
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= length && !anim.GetCurrentAnimatorStateInfo(0).IsName("idle_B"))
        {
            anim.Play("idle_B");
            Debug.Log("AnimReset ");
        }
    }

}