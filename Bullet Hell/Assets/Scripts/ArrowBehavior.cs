using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehavior : MonoBehaviour
{
    [SerializeField] private KeyCode _forwardKey;
    [SerializeField] private KeyCode _backwardKey;
    [SerializeField] private KeyCode _leftKey;
    [SerializeField] private KeyCode _rightKey;

    [SerializeField] private float _velocity = 2.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(_forwardKey))
        {
            transform.Translate(Vector3.up * _velocity * Time.deltaTime);
        }

        if (Input.GetKey(_backwardKey))
        {
            transform.Translate(Vector3.down * _velocity * Time.deltaTime);
        }

        if (Input.GetKey(_leftKey))
        {
            transform.Translate(Vector3.left * _velocity * Time.deltaTime);
        }

        if (Input.GetKey(_rightKey))
        {
            transform.Translate(Vector3.right * _velocity * Time.deltaTime);
        }
    }
}
