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
    public GameObject placementModle;

    [Header("临时测试用，验证完可以删掉")]
    [SerializeField]
    private bool enableTestHotkey = true;//勾上后按B键进入放置模式
    [SerializeField]
    private int testPlacementID = 0;//测试用的物体ID



    private void Start()
    {
        StopPlacement();
    }

    public void StartPlacement(int ID)
    {   
        if(dataBaseSO == null || dataBaseSO.objectDatas == null || dataBaseSO.objectDatas.Count == 0)
        {
            Debug.LogError($"[InputSystem] 物体数据库没赋值或者是空的，请先在 Object Data Base SO 里添加 ObjectData（传入的 ID={ID}）");
            return;
        }
        selectedObjectIndex = dataBaseSO.objectDatas.FindIndex(obj => obj.ID == ID);//根据ID查找物体在列表中的索引
        if(selectedObjectIndex < 0)
        {
            Debug.LogError($"No found ID {ID}");
            return;
        }
        if(placementModle == null)
        {
            Debug.LogError("InputSystem: 没有赋值 placementModle（放置预览模型）");
            selectedObjectIndex = -1;
            return;
        }
        placementModle.SetActive(true);
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
        if(placementModle != null) placementModle.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }
    private void Update()
    {
        TestHotkey();
        if(selectedObjectIndex < 0) return;
        Vector3 mousePos = inputManager.GetSelctedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePos);//将鼠标位置转换为网格坐标
        mouseIndicator.transform.position = mousePos;//
        cellIndicator.transform.position = grid.GetCellCenterWorld(gridPosition);//将网格坐标转换为世界坐标，并将其赋值给网格指示器的位置
    }

    private void TestHotkey()//临时测试：按B键进入放置模式，用来验证鼠标点击事件有没有触发
    {
        if(!enableTestHotkey) return;
        if(Input.GetKeyDown(KeyCode.B))
        {
            StartPlacement(testPlacementID);
        }
    }
}
