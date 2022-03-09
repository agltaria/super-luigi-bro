using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource audioPlayer;
    [SerializeField] AudioSource extraPlayer;
    [SerializeField] AudioClip[] clips;
    public static AudioManager audioManager;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (audioManager == null)
        {
            audioManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playSound(int x)
    {
        if (!audioPlayer.isPlaying)
            audioPlayer.clip = clips[x];
            audioPlayer.Play();
    }

    public void playExtra(int x)
    {
        if (!extraPlayer.isPlaying)
            extraPlayer.clip = clips[x];
            extraPlayer.Play();
    }
    public void playFlagpole()
    {
        if (!audioPlayer.isPlaying)
            audioPlayer.PlayOneShot(clips[11]);
    }

    public void playPoints()
    {
        if (!extraPlayer.isPlaying)
            extraPlayer.clip = clips[14];
            extraPlayer.PlayDelayed(0.2f);
    }
}
