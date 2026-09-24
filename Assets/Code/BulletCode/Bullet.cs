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
        if(other.CompareTag("End"))
        {
            //TODO:触发结束游戏事件
        }
        if (other.CompareTag("Left"))
        {
            //TODO:触发左转
        }
        if (other.CompareTag("Right"))
        {
            //TODO:触发右转
        }
    }
}
