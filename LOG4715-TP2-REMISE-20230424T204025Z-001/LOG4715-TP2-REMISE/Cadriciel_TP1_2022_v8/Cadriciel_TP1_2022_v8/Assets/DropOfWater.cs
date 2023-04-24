using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOfWater : MonoBehaviour
{
    // Start is called before the first frame update
    public float timeRemaining = 10;
    const int waterDamageValue = 1;
    public AudioClip waterhitSound;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;
        if (timeRemaining < 0)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SafePlateform") {
            Destroy(gameObject);
        }
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<Playerhealth>().PlayerGetHitted(waterDamageValue);
            Destroy(gameObject);
           
        }

    }
}
