using UnityEngine;


public class FollowPlayer : MonoBehaviour
{
    private string playerTag = "Player";
    private Vector3 _offset = new Vector3(10.68926f, 24.276801f, -5.980045f);
    private Transform _playerTransform;
    //private float _rotationSpeed = 5f;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            _playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("No player found");
        }
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            _playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("No player found");
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        FindPlayer();
        if (_playerTransform != null)
        {
            transform.position = _playerTransform.position + _offset;
            transform.rotation = Quaternion.Euler(52, -49, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerTransform == null)
        {
            FindPlayer();
        }
        
        if (_playerTransform != null)
        {
            transform.position = _playerTransform.position + _offset;
            transform.rotation = Quaternion.Euler(52, -49, 0);
        }
        
    }
}
