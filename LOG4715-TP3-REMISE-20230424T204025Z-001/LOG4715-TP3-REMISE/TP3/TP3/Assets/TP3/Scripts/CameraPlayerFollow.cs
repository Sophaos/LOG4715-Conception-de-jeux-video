using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerFollow : MonoBehaviour
{
    public Transform target;
    public float smoothing = 5f;
    private Vector3 offset;

    private Vector3 velocity = Vector3.zero;
    void Start()
    {
        offset = transform.position - target.position;
        //transform.position = target.position;
    }

    void Update()
    {
        //Vector3 targetCamPos = target.position + offset;
        //transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);

        Vector3 targetCamPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        transform.position = Vector3.SmoothDamp(transform.position, targetCamPos, ref velocity, smoothing);
    }


    //public Transform target;
    //public float smoothing;
    //private Vector3 offset;

    //private Vector3 velocity = Vector3.zero;

    //void Start()
    //{
    //    offset = transform.position - target.position;
    //    //transform.position = target.position;
    //}

    //void LateUpdate()
    //{
    //    Vector3 targetCamPos = target.position + offset;
    //    transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    //    //transform.position = Vector3.SmoothDamp(transform.position, targetCamPos, ref velocity, smoothing);
    //}
}
