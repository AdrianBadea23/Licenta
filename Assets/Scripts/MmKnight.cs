using UnityEngine;

public class MmKnight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] AudioClip song;
    void Start()
    {
        AudioSource.PlayClipAtPoint(song, transform.position, 0.5f); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
