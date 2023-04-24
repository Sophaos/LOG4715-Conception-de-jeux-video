using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringController : MonoBehaviour
{
    public float maxTime = 1f;
    public float timeRemaining;
    public GameObject waterToSpawn;
    // Start is called before the first frame update
    void Start()
    {
        timeRemaining = maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime* 0.1f;
        if (timeRemaining < 0.99)
        {
            timeRemaining = maxTime;
            Instantiate(waterToSpawn,transform.position,Quaternion.identity);
        }
    }
}
