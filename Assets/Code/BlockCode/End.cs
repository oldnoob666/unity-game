using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour, IBulletHittable
{
    public GameObject gUI;
    public GameObject winUI;
    public Camera camera;
    public List<GameObject>levels = new List<GameObject>(); 
    
    public void OnBulletHit(Bullet bullet)
    {
        gUI.SetActive(false);
        winUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void NextLevel()
    {

        camera.transform.position += new Vector3(15.9f, 0f, 0f);
        Time.timeScale = 1f;
        gUI.SetActive(true);
        winUI.SetActive(false);
    }


}
