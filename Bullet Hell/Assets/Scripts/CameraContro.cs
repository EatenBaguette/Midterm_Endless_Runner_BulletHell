using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform target;

    public Transform bg1;
    public Transform bg2;
    private float _size;

    [SerializeField] private float _interpolateTime = 0.1f;

    [SerializeField] private int _distanceInFront = -2;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _size = bg1.GetComponent<BoxCollider2D>() == null ? 0 : bg1.GetComponent<BoxCollider2D>().size.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        Vector3 targetPos = new Vector3(0, target.position.y - _distanceInFront, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, _interpolateTime);

        if (transform.position.y >= bg2.position.y)
        {
            bg1.position = new Vector3(bg1.position.x, bg2.position.y + _size, bg1.position.z);
            SwitchBGVariable();
        }
    }

    private void SwitchBGVariable()
    {
        Transform temporary = bg1;
        bg1 = bg2;
        bg2 = temporary;
    }
}
