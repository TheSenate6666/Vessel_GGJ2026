using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;           // The name you use to call this sound (e.g. "Jump")
    public AudioClip clip;
    
    [Range(0f, 1f)]
    public float volume = 1f;
    
    [Range(0.1f, 3f)]
    public float pitch = 1f;
    
    public bool loop = false;
}