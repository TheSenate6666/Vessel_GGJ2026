using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;
    [SerializeField]AudioSource aSource;

    

    [SerializeField] AudioClip damageTaken;
    [SerializeField] AudioClip died;


    float pitchMlt = 1f;
    float maxPitchMult = 4f;
    float pitchStepMlt = 24* Mathf.Sqrt(2);
    float pitchResetTime = 0.5f;
    float timeSinceLastKill = 0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (timeSinceLastKill > pitchResetTime)
        {
            pitchMlt = 1;
        }
        timeSinceLastKill += Time.deltaTime;
    }


    public void PlayAudio(AudioClip clip, float pitch, float volume)
    {
       
        Play2DClip(clip, volume, pitch);
       
        

        
    }

    //I used the base playClipAtPoint function from AudioSource to make somehting similar for non spacial sound and changing pitch
    public void Play2DClip(AudioClip clip, float volume, float pitch)
    {
        GameObject gameObject = new GameObject("One shot 2D audio");
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        audioSource.clip = clip;
        audioSource.spatialBlend = 0f;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.Play();
        
        
        Object.Destroy(gameObject, clip.length);
    }


    public void EnemyDiedSound(float volume)
    {
        Play2DClip(died, 0.05f, pitchMlt);
        pitchMlt *= pitchStepMlt;
        if(pitchMlt> maxPitchMult)
        {
            pitchMlt = maxPitchMult;
        }
        timeSinceLastKill = 0;
    }


    public void SetHitEmitter(SoundEmitter emitter)
    {
        emitter.SetAudioClip(damageTaken);
    }
    public void SetDiedEmitter(SoundEmitter emitter)
    {
        emitter.SetAudioClip(died);
    }

}
