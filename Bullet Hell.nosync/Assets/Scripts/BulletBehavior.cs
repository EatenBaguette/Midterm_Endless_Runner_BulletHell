using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float speed;

    private CameraControl _camera;

    void Start()
    {
        if (Camera.main != null) _camera = Camera.main.GetComponent<CameraControl>();
    }
    void Update()
    {
        transform.Translate(Vector3.up * (speed * Time.deltaTime), Space.Self);

        if (transform.position.x > _camera.bottomRight.x || transform.position.y < _camera.bottomRight.y ||
            transform.position.x < _camera.topLeft.x || transform.position.y > _camera.topLeft.y) Destroy(gameObject);
    }
}    