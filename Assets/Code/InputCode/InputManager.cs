using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour//用来获取鼠标地图位置的脚本
{
    [SerializeField]
    private Camera sceneCamera;
    private Vector3 lastPosition;//记录上一次鼠标的位置
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private LayerMask blockLayerMask;

    public event Action OnClicked, OnExit;//声明一个点击事件和退出事件
    public InputCountrol inputCountrol;



    private void Awake()
    {
       inputCountrol = new InputCountrol();
       inputCountrol.GamePlayerControl.Click.performed += ctx => OnClicked?.Invoke();//当鼠标点击时，触发OnClicked事件，ctx是上下文参数，表示当前的输入事件
       inputCountrol.GamePlayerControl.exit.performed += ctx => OnExit?.Invoke();//当鼠标点击时，触发OnExit事件
    }

    private void OnEnable()//必须启用动作表，否则里面的动作不会触发任何回调
    {
       inputCountrol.Enable();
    }

    private void OnDisable()//禁用动作表，避免内存泄漏（生成类的析构函数会断言）
    {
       inputCountrol.Disable();
    }


    public bool IsMouseOverUI()//判断鼠标是否在UI上
          => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();//如果鼠标在UI上，返回true，否则返回false（场景里没有EventSystem时不会报空引用）


    public Vector3 GetSelctedMapPosition()//获取鼠标在地图上的位置
    {
        Vector3 mousePos = Input.mousePosition;//获取鼠标位置的Vector3值，给mousePos的x和y赋值，z值为0
        mousePos.z = sceneCamera.nearClipPlane;//将鼠标位置的z值设置为摄像机的近裁剪面距离
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);//将鼠标位置转换为射线，为当前点到摄像机的射线
        RaycastHit hit;//声明一个射线碰撞体hit
        if (Physics.Raycast(ray, out hit,100, layerMask))//如果射线碰撞体hit与射线ray发生碰撞，且碰撞体在layerMask层中，就把物体赋值给hit
        {
            lastPosition = hit.point;//将碰撞体的点赋值给lastPosition
        }

        return lastPosition;
    }

     public GameObject GetObjectUnderMouse()//获取鼠标下的物体,进行删除
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, blockLayerMask))
        {
            Transform block = hit.collider.transform;
            while (block.parent != null && (blockLayerMask.value & (1 << block.parent.gameObject.layer)) != 0)
            {
                block = block.parent;
            }
            return block.gameObject;
        }
        return null;
    }
    


}
