using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterController : MonoBehaviour
{

    public List<GameObject> _character;
    int _current_char = 0;
    bool ani_state = false;

    public void updateCha(int index)
    {
        if (_character.Count <= index)
        {
            Debug.LogError("_character.Count error");
            return;
        }

        _current_char = index;
        for (int i=0;i< _character.Count;i++)
        {
            if(_current_char == i)_character[i].SetActive(true);
            else _character[i].SetActive(false);
        }
        
        updateAnime();
    }

    void updateAnime()
    {
        Animator anim =  _character[_current_char].GetComponent<Animator>();
        anim.SetBool("bool_random" , true);
        anim.SetInteger("animation", Random.Range(1, 10));
        //ani_state = true;
    }

    private void Update()
    {
        if (ani_state == true) {
            Animator anim = _character[_current_char].GetComponent<Animator>();
            anim.SetInteger("animation", Random.Range(1, 10));
        //    ani_state = false;
        }

    }
}
