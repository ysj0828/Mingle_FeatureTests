using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace MingleCamera
{
    public class ParticleEffectManagerScript : MonoBehaviour
    {
        public static ParticleEffectManagerScript instance = null;
        public GameObject[] ParticleEffects;
        public List<AudioClip> sfxClips = new List<AudioClip>();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }

            else
            {
                Destroy(gameObject);
            }
        }
    }
}