using UnityEngine;


public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    private Vector3 _offset = new Vector3(10.68926f, 24.276801f, -5.980045f);
    //private float _rotationSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = playerTransform.transform.position + _offset;
        transform.rotation = Quaternion.Euler(52, -49, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerTransform.transform.position + _offset;
        transform.rotation = Quaternion.Euler(52, -49, 0);
    }
}
