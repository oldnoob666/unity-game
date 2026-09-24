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
        if (turnDir == TurnDir.Left)
        {
            //Debug.Log("Bullet hit LeftAndRight: Left");
            bullet.transform.Rotate(0, 90, 0);
        }
        else if (turnDir == TurnDir.Right)
        {
            bullet.transform.Rotate(0, -90, 0);
        }
    }
}
