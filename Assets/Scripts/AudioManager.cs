using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource audioPlayer;
    [SerializeField] AudioClip[] clips;
    private static AudioManager audioManager;
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

    public void playSmallJump()
    {
        audioPlayer.PlayOneShot(clips[0]);
    }

    public void playSuperJump()
    {
        audioPlayer.PlayOneShot(clips[1]);
    }

    public void playCoin()
    {
        audioPlayer.PlayOneShot(clips[2]);
    }

    public void playBreakBlock()
    {
        audioPlayer.PlayOneShot(clips[3]);
    }

    public void playBump()
    {
        audioPlayer.PlayOneShot(clips[4]);
    }

    public void playPowerAppear()
    {
        audioPlayer.PlayOneShot(clips[5]);
    }

    public void playPowerup()
    {
        audioPlayer.PlayOneShot(clips[6]);
    }

    public void play1Up()
    {
        audioPlayer.PlayOneShot(clips[7]);
    }

    public void playStomp()
    {
        audioPlayer.PlayOneShot(clips[8]);
    }

    public void playFireball()
    {
        audioPlayer.PlayOneShot(clips[9]);
    }

    public void playPipe()
    {
        audioPlayer.PlayOneShot(clips[10]);
    }

    public void playFlag()
    {
        audioPlayer.PlayOneShot(clips[11]);
    }

    public void playDie()
    {
        audioPlayer.PlayOneShot(clips[13]);
    }
}
