using System;
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

    [SerializeField] private float _borderCollisionDistance = 0.25f;

    [SerializeField] private float _interpolateTime = 3.0f;

    private ScoreBehavior _scoreBehavior;

    private float topYPos = 0;
    
    // Start is called before the first frame update
    void Start()
    { 
        _scoreBehavior = GetComponent<ScoreBehavior>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Movement
        if (Input.GetKey(_forwardKey))
        {
            transform.Translate(Vector3.up * (_velocity * Time.deltaTime));
        }

        if (Input.GetKey(_backwardKey))
        {
            transform.Translate(Vector3.down * (_velocity * Time.deltaTime));
        }

        if (Input.GetKey(_leftKey))
        {
            transform.Translate(Vector3.left * (_velocity * Time.deltaTime));
        }

        if (Input.GetKey(_rightKey))
        {
            transform.Translate(Vector3.right * (_velocity * Time.deltaTime));
        }
        
        //Score Updating
        if (transform.position.y > topYPos)
        {
            topYPos = transform.position.y;
            _scoreBehavior.Score = topYPos;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Border"))
        {
            float xDistance = (transform.position.x - other.transform.position.x) / other.transform.localScale.x;
            float yDistance = (transform.position.y - other.transform.position.y) / other.transform.localScale.y;
            
            

            if (Mathf.Abs(xDistance) > Mathf.Abs(yDistance))
            {
                Vector3 targetpos = new Vector3(
                    Mathf.Sign(xDistance) == 1 ? transform.position.x + _borderCollisionDistance : transform.position.x - _borderCollisionDistance,
                    transform.position.y,
                    transform.position.z
                    );
                
                transform.position = Vector3.Lerp(transform.position, targetpos, _interpolateTime);
            }
            
            else if (Mathf.Abs(yDistance) > Mathf.Abs(xDistance))
            {
                Vector3 targetpos = new Vector3(
                    transform.position.x,
                    Mathf.Sign(yDistance) == 1 ? transform.position.y + _borderCollisionDistance : transform.position.y - _borderCollisionDistance,
                    transform.position.z
                    );
                
                transform.position = Vector3.Lerp(transform.position, targetpos, _interpolateTime);
            }
        }
    }
}
