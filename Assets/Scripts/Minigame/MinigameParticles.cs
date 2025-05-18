using UnityEngine;

public class MinigameParticles : MonoBehaviour
{
    public ParticleSystem particlePrefab;

    // Call this to spawn particles at world position (0,0,0)
    public void SpawnParticles()
    {
        if (particlePrefab != null)
        {
            Instantiate(particlePrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Particle prefab not assigned.");
        }
    }
}
