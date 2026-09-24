using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Launch : MonoBehaviour
{
    public InputCountrol inputCountrol;
    public GameObject bulletPrefab;
    public Transform muzzle;
    public float fireRate = 0.5f; // Time between shots
    float nextFireTime;
    private void Awake()
    {
        inputCountrol = new InputCountrol();
        inputCountrol.GamePlayerControl.Launch.started += LaunchBullet;
    }


    private void OnEnable()
    {
        inputCountrol.Enable();
    }

    private void OnDisable()
    {
        inputCountrol.Disable();
    }
    private void LaunchBullet(InputAction.CallbackContext context)
    {
        if(Time.time >= nextFireTime)
        {
            Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
            nextFireTime = Time.time + fireRate;
        }
        else
        {
            return;
        }
    }
}
