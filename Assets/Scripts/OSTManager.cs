using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSTManager : MonoBehaviour
{
    public static OSTManager ostManager;
    public static bool isAlive;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] clips;
    // Start is called before the first frame update
    void Start()
    {
        ostManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(isAlive)
            Invincible();

    }

    void Invincible()
    {
        if (PlatformerPlayer.isInvincible)
        {
            audioSource.clip = clips[1];
            if(!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            audioSource.clip = clips[0];
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
    }

    public void PlayDie()
    {
        audioSource.clip = clips[3];
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(clips[3]);
            isAlive = false;
            
    }

    public void StopPlaying()
    {
        audioSource.Stop();
        isAlive = false;
    }
    public void PlayWin()
    {
        audioSource.clip = clips[2];
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(clips[2]);
        

    }

}
