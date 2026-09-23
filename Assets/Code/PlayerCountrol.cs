using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCountrol : MonoBehaviour
{
    public InputCountrol inputCountrol;
    
    private void Awake()
    {
        inputCountrol = new InputCountrol();
        inputCountrol.GamePlayerControl.PlayerDirRight.started += RightDir;
        inputCountrol.GamePlayerControl.PlayerDirLeft.started += LeftDir;
        inputCountrol.GamePlayerControl.PlayerDirPos.started += PosDIr;
    }
    private void OnEnable()
    {
        inputCountrol.Enable();
    }
    private void OnDisable()
    {
        inputCountrol.Disable();
    }



    public void RightDir(InputAction.CallbackContext context)
    {
        // 不能直接修改transform.rotation.y，需要整体赋值
        transform.rotation = Quaternion.Euler(0, -90, 0);
    }


    public void LeftDir(InputAction.CallbackContext context)
    {
        transform.rotation = Quaternion.Euler(0, 90, 0);
    }


    public void PosDIr(InputAction.CallbackContext context)
    {
        
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
