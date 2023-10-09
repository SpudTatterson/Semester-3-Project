using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomSoundPlayer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector2 minMaxTimeBetweenGames;
    [Header("References")]
    AudioSource audioSource;
    [SerializeField] AudioClip[] clips;


    bool started = false;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!started)
        {
            started = true;
            float time = Random.Range(minMaxTimeBetweenGames.x, minMaxTimeBetweenGames.y);
            Invoke("Play", time);
        }

    }

    public void Play()
    {
        SetRandomClip();
        audioSource.Play();
        Invoke("ToggleStarted" ,audioSource.clip.length);
    }
    void ToggleStarted()
    {
        started = false;
    }
    void SetRandomClip()
    {
        int clipIndex = Random.Range(0, clips.Length);
        audioSource.clip = clips[clipIndex];
    }
}
