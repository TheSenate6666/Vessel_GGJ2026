using System;
using UnityEngine;
using UnityEngine.Events;


public class SoundEmitter : MonoBehaviour
{
    [SerializeField]AudioClip soundClip;
    

    
    public float baseVol = 0.8f;

    private void Start()
    {
       
    }

    public void PlaySound()
    {
        if (SoundManager.instance == null) { return; }
        if (soundClip == null) { return ; }
        
        SoundManager.instance.PlayAudio(soundClip,1, baseVol);
    }
    public void PlaySound(float volume)
    {

        if (SoundManager.instance == null) { return; }
        if (soundClip == null) { return; }

        SoundManager.instance.PlayAudio(soundClip,1,volume);
    }
    public void PlaySound(float volume, float pitch)
    {
       

        
        SoundManager.instance.PlayAudio(soundClip, pitch, volume);
    }


    public void SetAudioClip(AudioClip newClip)
    {
        soundClip = newClip;
    }

    public AudioClip GetAudioClip()
    {
        return soundClip;
    }
}
