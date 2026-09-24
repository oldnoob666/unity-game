using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour, IBulletHittable
{
     public void OnBulletHit(Bullet bullet)
    {
        Debug.Log("Bullet hit the end point");
    }
}
