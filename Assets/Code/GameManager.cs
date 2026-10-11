using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour//管理UI，相机移动，玩家移动位置
{
    [Header("事件监听")]
    public EndEventSO endEvent;
    [Header("UI")]
    public GameObject gUI;
    public GameObject winUI;
    public GameObject firstUI;
    [Header("关卡切换设置")]
    public Camera camera;
    public List<Vector3> levels = new List<Vector3>();
    public GameObject player;
    private int i;

    private void OnEnable()
    {
       endEvent.OnEnd += OnWin;
    }
    private void OnDisable()
    {
        endEvent.OnEnd -= OnWin;
    }


    private void OnWin(GameObject obj)
    {
        gUI.SetActive(false);
        winUI.SetActive(true);
        Time.timeScale = 0f;
        obj.SetActive(false);
    }
    public void NextLevel()
    {
        i++;
        player.transform.position = new Vector3(levels[i].x, levels[i].y, levels[i].z);
        camera.transform.position += new Vector3(15.61f, 0f, 0f);
        Time.timeScale = 1f;
        gUI.SetActive(true);
        winUI.SetActive(false);
    }

    public void OnClick()
    {
        firstUI.SetActive(false);
    }
}
