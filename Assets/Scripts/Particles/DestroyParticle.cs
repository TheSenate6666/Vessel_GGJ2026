using UnityEngine;

public class DestroyParticleSystem : MonoBehaviour
{
    void Start()
    {
        // Destroy the GameObject after 3 seconds
        Destroy(gameObject, 3f);
    }
}