using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class KnockbackFeedback : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private float power = 16, delay = 0.15f, raduis = 5f;


    public UnityEvent OnBegin, OnDone;


    public void PlayFeedBack(GameObject sender)
    {
        Debug.Log("PlayFeedBack called");
        StopAllCoroutines();
        OnBegin?.Invoke();

        rb.AddExplosionForce(power, sender.transform.position, raduis, 0f, ForceMode.Impulse);

        StartCoroutine(Reset());
    }


    private IEnumerator Reset() 
    { 
        yield return new WaitForSeconds(delay);

        rb.velocity = Vector3.zero;

        OnDone?.Invoke();
    } 
}
