using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject firstUI;
   public void OnClick()
    {
        firstUI.SetActive(false);
    }
}
