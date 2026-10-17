using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class EndEventSO : ScriptableObject
{
    public UnityAction OnEnd;

    public void EventRised()//‘⁄End.cs¿Ô¥•∑¢
    {
        OnEnd?.Invoke();
    }
}
