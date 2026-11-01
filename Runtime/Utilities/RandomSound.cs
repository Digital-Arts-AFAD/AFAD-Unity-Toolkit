using UnityEngine;

public class RandomSound : MonoBehaviour
{
    public AudioClip[] sounds;
    public AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        if (audioSource && sounds.Length > 0)
        {
            audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
        }
    }

    public void Stop()
    {
        if (audioSource)
        {
            audioSource.Stop();
        }
    }

    public void PlayLoop()
    {
        if (audioSource && sounds.Length > 0)
        {
            audioSource.loop = true;
            audioSource.clip = sounds[Random.Range(0, sounds.Length)];
            audioSource.Play();
        }
    }
}
