using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OllieJones
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        private AudioSource audioSource;

        [SerializeField] private AudioClip clipFlip;
        [SerializeField] private AudioClip clipFlop;
        [SerializeField] private AudioClip clipMatch;

        private void Awake()
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayCardFlip()
        {
            audioSource.clip = clipFlip;
            audioSource.Play();
        }

        public void PlayCardFlop()
        {
            audioSource.clip = clipFlop;
            audioSource.Play();
        }

        public void PlayCardMatch()
        {
            audioSource.clip = clipMatch;
            audioSource.Play();
        }
    }
}


