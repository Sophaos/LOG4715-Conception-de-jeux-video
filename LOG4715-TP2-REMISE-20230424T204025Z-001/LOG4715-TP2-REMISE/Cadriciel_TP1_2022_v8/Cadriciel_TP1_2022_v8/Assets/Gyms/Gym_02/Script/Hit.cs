using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hit : MonoBehaviour
{

    public UnityEvent<GameObject> OnHitWithReference;

    public void getHit(GameObject sender)
    {
        if (sender.layer == gameObject.layer) {
            return;
        }

        Debug.Log("Hit received");

        //je verifie juste onHit pas besion onDeath pour cette scene
        OnHitWithReference?.Invoke(sender);
    }
}
