using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class EndEventSO : ScriptableObject
{
    public UnityAction<GameObject> OnEnd;

    public void EventRised(GameObject obj)//‘⁄End.cs¿Ô¥•∑¢
    {
        OnEnd?.Invoke(obj);
    }
}
