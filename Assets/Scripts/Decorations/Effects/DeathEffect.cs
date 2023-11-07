using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;

    void Update()
    {
        if (!particleSystem.isPlaying)
            Destroy(gameObject);
    }
}
