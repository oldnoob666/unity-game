using UnityEngine;

public interface IBulletHittable//这是一个接口，是子弹链接目标物体的中介，触发other.GetComponent<IBulletHittable>()函数后，会通过借口找到组件（赋值给target），从而调用OnBulletHit函数
{
    void OnBulletHit(Bullet bullet);

}

