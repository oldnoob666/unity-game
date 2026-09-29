using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftAndRight : MonoBehaviour, IBulletHittable
{
    public enum TurnDir
    {
        Left = 0,
        Right = 1,
    }
    public TurnDir turnDir;

    public void OnBulletHit(Bullet bullet)
    {
        //Debug.Log("Bullet hit LeftAndRight");   
        if (bullet == null) return;
        bullet.transform.rotation = transform.rotation;
    }
}
