using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour, IBulletHittable
{
    public GameObject gUI;
    public GameObject winUI;
    public Camera camera;
    public List<Vector3>levels = new List<Vector3>();
    public GameObject player;
    private int i;
    
    public void OnBulletHit(Bullet bullet)
    {
        gUI.SetActive(false);
        winUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void NextLevel()
    {
        i++;
        
        player.transform.position = new Vector3(levels[i].x, levels[i].y, levels[i].z);
        camera.transform.position += new Vector3(15.61f, 0f, 0f);
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        gUI.SetActive(true);
        winUI.SetActive(false);
    }


}
