using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    private float initialBaseTime = 3.5f; // Base spawn interval
    private float finalBaseTime = 2.5f;
    private float rampDuration = 15f;
    private float minSpawnInterval = 1.1f;
    private float noiseScale = 1.2f; // Scale for the Perlin noise
    private float noiseStrength = 0.3f;
    private float timeElapsed = 0f; 
    private float noiseOffset;
    private float gameTime;
    public AudioClip spawnSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        noiseOffset = Random.Range(0f, 100f); 
        Spawn();
    }

    private void Update()
    {
        gameTime += Time.deltaTime;
    }

    private void Spawn()
    {
        float spawnTime = GetNoisySpawnTime();
        Instantiate(prefab, transform.position, Quaternion.identity);
        Invoke(nameof(Spawn), spawnTime);
        audioSource.PlayOneShot(spawnSound);
    }

    private float GetNoisySpawnTime()
    {
        float progress = Mathf.Clamp01(gameTime/ rampDuration);
        float currentBaseTime = Mathf.Lerp(initialBaseTime, finalBaseTime, progress);

        float noise = Mathf.PerlinNoise(noiseOffset, timeElapsed * noiseScale);
        float noiseEffect = Mathf.Lerp(-noiseStrength, noiseStrength, noise);

        float rawSpawnTime = currentBaseTime + noiseEffect;
        return Mathf.Clamp(rawSpawnTime, minSpawnInterval, initialBaseTime * 1.2f);
    }
}
