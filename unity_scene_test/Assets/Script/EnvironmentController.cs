using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    public List<GameObject> _environment;
    int _current_env = 0;

    private void Awake()
    {
        //_environment[1].SetActive(false);
        //_environment[2].SetActive(false);
        //_environment[3].SetActive(false);

        for (int i = 1; i < _environment.Count-1; i++)
        {
            _environment[i].SetActive(false);
        }
    }

    public void updateenv(int index)
    {
        if(_environment.Count <= index)
        {
            Debug.LogError("_environment.Count error");
            return;
        } 

        _current_env = index;
        for(int i=0; i < _environment.Count; i++)
        {
            if (_current_env == i) _environment[i].SetActive(true);
            else _environment[i].SetActive(false);
        }

    }

}
