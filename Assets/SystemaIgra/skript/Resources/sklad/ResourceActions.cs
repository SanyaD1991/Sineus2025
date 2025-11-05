using UnityEngine;

public class ResourceActions : MonoBehaviour
{
    [Header("Ёффекты и звуки при создании/подборе ресурса")]
    public AudioClip spawnSound;
    public ParticleSystem spawnEffect;

    public void PlaySpawnEffects(Vector3 position)
    {
        if (spawnEffect != null)
            Instantiate(spawnEffect, position, Quaternion.identity);

        if (spawnSound != null)
            AudioSource.PlayClipAtPoint(spawnSound, position);
    }
}
