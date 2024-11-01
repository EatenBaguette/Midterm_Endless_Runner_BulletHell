using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BottomBorderBehavior : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;

    private Vector2 _targetPosition;
    private Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _targetPosition = new Vector2(0, _mainCamera.transform.position.y - _mainCamera.orthographicSize -0.2f);
        rb.MovePosition(_targetPosition);
    }
}
