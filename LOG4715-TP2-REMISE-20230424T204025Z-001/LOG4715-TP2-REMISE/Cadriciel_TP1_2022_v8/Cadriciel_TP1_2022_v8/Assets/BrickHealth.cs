using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickHealth : MonoBehaviour
{
  
    public float MaxBrickLifetime = 3;
    private float timeRemaining;
    public bool WallDestructed = false;
    // Start is called before the first frame update
    void Start()
    {
        timeRemaining = MaxBrickLifetime;
    }

    // Update is called once per frame
    void Update()
    {
        if (WallDestructed) {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0)
            {
                Destroy(gameObject);
            }
        }
       
    }
}
