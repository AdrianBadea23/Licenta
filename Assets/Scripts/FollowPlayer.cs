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
            // transform.rotation = Quaternion.Euler(52, -49, 0);
        }
        
        if (_playerTransform != null)
        {
            // Get input from A (-1) and D (+1) for rotation
            float horizontalInput = 0f;
            if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
            if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;

            if (horizontalInput != 0)
            {
                // Rotate the offset around the player
                Quaternion rotation = Quaternion.AngleAxis(horizontalInput * 100f * Time.deltaTime, Vector3.up);
                _offset = rotation * _offset;
            }

            // Zoom with W and S
            float zoomInput = 0f;
            if (Input.GetKey(KeyCode.W)) zoomInput = -1f;
            if (Input.GetKey(KeyCode.S)) zoomInput = 1f;

            float zoomSpeed = 5f;
            float minDistance = 5f;
            float maxDistance = 30f;

            // Adjust offset magnitude for zoom effect
            float newDistance = Mathf.Clamp(_offset.magnitude + zoomInput * zoomSpeed * Time.deltaTime, minDistance, maxDistance);
            _offset = _offset.normalized * newDistance;

            // Maintain camera position based on offset
            transform.position = _playerTransform.position + _offset;

            // Make the camera look at the player
            transform.LookAt(_playerTransform.position);
        }
        
    }
}
