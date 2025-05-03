using UnityEngine;

public class SpiritFlw : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject player;
    Vector3 velocity = Vector3.zero;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            Vector3 targetPosition = new Vector3(player.transform.position.x, 4f, player.transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.1f);
        }
    }

    public void ChangeColor(Color color)
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = color;
    }
}
