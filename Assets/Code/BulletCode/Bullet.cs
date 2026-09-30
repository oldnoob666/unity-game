using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 6f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    private void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("111");
        if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            
            Destroy(gameObject);
        }
       IBulletHittable target = other.GetComponent<IBulletHittable>();
        if (target != null)
        {
            target.OnBulletHit(this);
        }
    }
}
