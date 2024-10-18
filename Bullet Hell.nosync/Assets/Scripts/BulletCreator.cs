using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCreator : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    
    public GameObject CreateBullet(Vector3 pos, Quaternion rot, float speed)
    {
        GameObject bulletInstance = Instantiate(bulletPrefab, pos, rot);
        
        bulletInstance.GetComponent<BulletBehavior>().speed = speed;
        
        return bulletInstance;
    }
}
