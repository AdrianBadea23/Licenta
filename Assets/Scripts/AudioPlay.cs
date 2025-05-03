using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying) {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
