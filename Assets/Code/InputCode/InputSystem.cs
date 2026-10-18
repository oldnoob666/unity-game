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
    [SerializeField]
    private ObjectDataBaseSO dataBaseSO;
    private int selectedObjectIndex = -1;

    private void Start()
    {
        StopPlacement();
    }

    public void StartPlacement(int ID)//通过按钮启动，ID在按钮页填入
    {   
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
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }
    private void Update()
    {
        
        Vector3 mousePos = inputManager.GetSelctedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePos);//将鼠标位置转换为网格坐标
        mouseIndicator.transform.position = mousePos;//
        if (selectedObjectIndex < 0) return;
        cellIndicator.transform.position = grid.GetCellCenterWorld(gridPosition);//将网格坐标转换为世界坐标，并将其赋值给网格指示器的位置
    }
}
