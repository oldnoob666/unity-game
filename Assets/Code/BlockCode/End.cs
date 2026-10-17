using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class End : MonoBehaviour, IBulletHittable
{
   
    public EndEventSO endEvent;


    public void OnBulletHit(Bullet bullet)
    {
        endEvent.EventRised();
        gameObject.SetActive(false);
    }


}
