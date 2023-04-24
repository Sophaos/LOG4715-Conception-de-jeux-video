using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventHelper : MonoBehaviour
{
    public UnityEvent OnAttackAnimationDone;

    public void triggerEvent()
    {
        OnAttackAnimationDone?.Invoke();
    }

}
