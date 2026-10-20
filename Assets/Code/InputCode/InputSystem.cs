using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator,cellIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;
    private GameObject obj = null;
    private bool isDeleting;
    private Renderer[] targetRenderers;
    private MaterialPropertyBlock[] originalProperties;
    [SerializeField]
    private ObjectDataBaseSO dataBaseSO;
    private int selectedObjectIndex = -1;

    private void Start()
    {
        StopPlacement();
    }

    public void StartPlacement(int ID)//通过按钮启动，ID在按钮页填入
    {   
        StopPlacement();
        if(dataBaseSO == null || dataBaseSO.objectDatas == null || dataBaseSO.objectDatas.Count == 0)
        {
            Debug.LogError($"[InputSystem] 物体数据库没赋值或者是空的，请先在 Object Data Base SO 里添加 ObjectData（传入的 ID={ID}）");
            return;
        }
        selectedObjectIndex = dataBaseSO.objectDatas.FindIndex(obj => obj.ID == ID);//是一个 Lambda 表达式，表示“检查每个对象的 ID 是否等于当前的 ID”。
        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"No found ID {ID}");
            return;
        }
        inputManager.OnClicked -= PlaceStructure;//先退订再订阅，避免重复调用StartPlacement时一次点击放多个
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        //Debug.Log("111");
        if(inputManager.IsMouseOverUI()) return;//鼠标在UI上时不要放置物体
       // Debug.Log("222");
        Vector3 mousePos = inputManager.GetSelctedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePos);//将鼠标位置转换为网格坐标
        GameObject newObject = Instantiate(dataBaseSO.objectDatas[selectedObjectIndex].Prefab);//
        newObject.transform.position = grid.GetCellCenterWorld(gridPosition);
    }

    public void StopPlacement()
    {
        selectedObjectIndex = -1;
        isDeleting = false;
        SetDeleteTarget(null);
        inputManager.OnClicked -= Delete;
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }



    public void DeleteObject()
    {
        StopPlacement();
        isDeleting = true;
        inputManager.OnClicked += Delete;
        inputManager.OnExit += StopPlacement;
    }

    private void Delete()
    {
        if (!isDeleting || inputManager.IsMouseOverUI()) return;
        GameObject target = inputManager.GetObjectUnderMouse();
        SetDeleteTarget(null);
        if (target != null) Destroy(target);
    }

    private void SetDeleteTarget(GameObject target)
    {
        if (obj == target) return;//如果是相同方块（鼠标没移动到其他方块上），就不需要重新设置高亮
        if (targetRenderers != null)
        {
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                if (targetRenderers[i] != null)
                    targetRenderers[i].SetPropertyBlock(originalProperties[i]);//如果targetRenderers不为空（即上一个方块还是红色的），就让originalProperties里的设置加进去，让它变成正常颜色
            }
        }

        obj = target;//把新的方块放进来
        targetRenderers = null;
        originalProperties = null;//清除旧的东西
        if (obj == null) return;

        targetRenderers = obj.GetComponentsInChildren<Renderer>();//存入新方块的组件，一会要先把这个传入originalProperties
        originalProperties = new MaterialPropertyBlock[targetRenderers.Length];//新建一个保存旧设置的class类，长度相同
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            originalProperties[i] = new MaterialPropertyBlock();
            targetRenderers[i].GetPropertyBlock(originalProperties[i]);//获取当前材质属性块的值，并将其存储在originalProperties数组中，以便稍后恢复原始颜色
            MaterialPropertyBlock highlight = new MaterialPropertyBlock();
            targetRenderers[i].GetPropertyBlock(highlight);//获取当前材质属性块的值，并将其存储在highlight中，以便稍后设置为红色
            highlight.SetColor("_Color", Color.red);
            highlight.SetColor("_BaseColor", Color.red);
            targetRenderers[i].SetPropertyBlock(highlight);//把设置应用到物体上
        }
    }

    private void Update()
    {
        
        if (isDeleting)
            SetDeleteTarget(inputManager.IsMouseOverUI() ? null : inputManager.GetObjectUnderMouse());

        Vector3 mousePos = inputManager.GetSelctedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePos);//将鼠标位置转换为网格坐标
        mouseIndicator.transform.position = mousePos;//
        if (selectedObjectIndex < 0) return;
        cellIndicator.transform.position = grid.GetCellCenterWorld(gridPosition);//将网格坐标转换为世界坐标，并将其赋值给网格指示器的位置
    }
}
