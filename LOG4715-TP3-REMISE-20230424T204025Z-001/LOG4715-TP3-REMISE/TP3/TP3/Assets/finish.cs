using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finish : MonoBehaviour
{
    private BoxCollider _collider;
    public GameObject _canvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _canvas.active = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
