using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Setup")]
    [SerializeField] private AudioSource musicSource; // Assign a dedicated AudioSource for music in Inspector
    [SerializeField] private Sound[] soundLibrary;    // Add all your clips here in the Inspector

    // Dictionary for fast lookups by name
    private Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();

    [Header("Combo System")]
    [SerializeField] private string enemyDeathSoundName = "EnemyDied"; // Name must match Library
    [SerializeField] private string damageTakenSoundName = "DamageTaken";
    
    private float pitchMlt = 1f;
    private float maxPitchMult = 4f;
    // Note: 24 * Sqrt(2) is a massive number (~33). Ensure this is the math you intended.
    private float pitchStepMlt = 24 * Mathf.Sqrt(2); 
    private float pitchResetTime = 0.5f;
    private float timeSinceLastKill = 0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep sound manager across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeLibrary();
    }

    private void InitializeLibrary()
    {
        foreach (Sound s in soundLibrary)
        {
            if (!soundDictionary.ContainsKey(s.name))
            {
                soundDictionary.Add(s.name, s);
            }
        }
    }

    void Update()
    {
        // Combo Logic
        if (timeSinceLastKill > pitchResetTime)
        {
            pitchMlt = 1f;
        }
        timeSinceLastKill += Time.deltaTime;
    }

    // --- MUSIC SYSTEM ---

    public void PlayMusic(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound s))
        {
            // Don't restart if it's already playing
            if (musicSource.clip == s.clip && musicSource.isPlaying) return;

            musicSource.clip = s.clip;
            musicSource.volume = s.volume;
            musicSource.pitch = s.pitch;
            musicSource.loop = true; // Music usually loops
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Music: " + name + " not found!");
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // --- SFX SYSTEM ---

    public void PlaySFX(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound s))
        {
            Play2DClip(s.clip, s.volume, s.pitch);
        }
        else
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
    }

    // Overload to allow manual volume/pitch overrides
    public void PlaySFX(string name, float volumeScale, float pitchOverride = -1)
    {
        if (soundDictionary.TryGetValue(name, out Sound s))
        {
            float finalPitch = (pitchOverride > 0) ? pitchOverride : s.pitch;
            Play2DClip(s.clip, s.volume * volumeScale, finalPitch);
        }
    }

    private void Play2DClip(AudioClip clip, float volume, float pitch)
    {
        GameObject go = new GameObject("OneShotAudio");
        AudioSource audioSource = go.AddComponent<AudioSource>();
        
        audioSource.clip = clip;
        audioSource.spatialBlend = 0f; // 2D sound
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.Play();

        Destroy(go, clip.length + 0.1f); // Destroy after clip finishes
    }

    // --- COMBO SPECIFIC ---

    public void PlayEnemyDiedSound()
    {
        // Try to find the sound in the library first
        if (soundDictionary.TryGetValue(enemyDeathSoundName, out Sound s))
        {
            Play2DClip(s.clip, 0.05f, pitchMlt);
            
            // Logic Update
            pitchMlt *= pitchStepMlt;
            if (pitchMlt > maxPitchMult) pitchMlt = maxPitchMult;
            timeSinceLastKill = 0;
        }
    }
}