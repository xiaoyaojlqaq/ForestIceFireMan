using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinAudioManager : MonoBehaviour
{
    public static coinAudioManager instance;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        audioSource=GetComponent<AudioSource>();
    }


    public void PlayCoinSound()
    {
        audioSource.Play();
    }
}
