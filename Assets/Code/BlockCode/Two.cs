using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Two : MonoBehaviour, IBulletHittable
{
    public void OnBulletHit(Bullet bullet)
    {
        Destroy(bullet.gameObject);

    }
}
