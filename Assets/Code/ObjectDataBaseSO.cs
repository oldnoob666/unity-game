using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectDataBaseSO : ScriptableObject
{
    public List<ObjectData> objectDatas;
}

[Serializable]
public class ObjectData
{
    [field: SerializeField]
    public string Name { get; private set; }//物体名称

    [field: SerializeField]
    public int ID { get; private set; }//物体ID

    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;//物体大小

    [field: SerializeField]
    public GameObject Prefab { get; private set; }//物体预制体
    
}