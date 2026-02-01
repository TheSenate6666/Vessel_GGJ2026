using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The name must match an entry in SoundManager's Library")]
    [SerializeField] private string soundName; 
    
    [Range(0f, 1f)]
    public float volumeMultiplier = 1f;

    public void PlaySound()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySFX(soundName, volumeMultiplier);
        }
    }
    
    public void PlayAsMusic()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayMusic(soundName);
        }
    }

    // Allows changing the target sound dynamically
    public void SetSoundName(string newName)
    {
        soundName = newName;
    }
}