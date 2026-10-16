using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneIndex, LoadSceneMode.Single);
    }

    public void OnClick()
    {
        firstUI.SetActive(false);
    }
}
