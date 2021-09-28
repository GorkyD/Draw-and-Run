using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private Vector3 _offset;

    private void Start()
    { 
        _offset = transform.position - player.transform.position;
    }

    private void LateUpdate()
    {      
        if (player != null)
        {
            transform.position = player.transform.position + _offset;
        }
    }
}