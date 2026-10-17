using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Two : MonoBehaviour, IBulletHittable//把子弹一分为二
{
    public GameObject bulletPrefab;
    public Transform trans1;
    public Transform trans2;
    public float resetTime = 5f;
    private float time = 0f;
    private bool isDestroyed = false;

    private void Update()
    {
        time += Time.deltaTime;
        if(time > resetTime)
        {
            isDestroyed = false;
            time = 0f;
        }
    }

    public void OnBulletHit(Bullet bullet)
    {
        if (isDestroyed) return;
        Destroy(bullet.gameObject);
        isDestroyed = true;
        Instantiate(bulletPrefab, trans1.position, trans1.rotation);
        Instantiate(bulletPrefab, trans2.position, trans2.rotation);
    }
}
