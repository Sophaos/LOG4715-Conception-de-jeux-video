using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterBomb : MonoBehaviour
{

    // Déclaration des variables
    Animator _Anim { get; set; }
    Rigidbody _Rb { get; set; }

    bool isAnimationTriggered = false;

    [SerializeField]
    float radius = 5f;

    [SerializeField]
    float force = 10f;

    [SerializeField]
    LayerMask WhatIsPlayer;

    [SerializeField]
    LayerMask WhatIsWall;

    // Start is called before the first frame update
    void Start()
    {
        _Anim = GetComponent<Animator>();
        _Rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision c) {
        if (isAnimationTriggered) { return; }

        if (shouldExplose(c))
        {
            //blocker le momevement dans z pour la bomb
            _Rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ;
            if (!isAnimationTriggered) 
            {
                _Anim.SetTrigger("attack01");
                isAnimationTriggered = true;
            }
        }
    }

    private bool shouldExplose(Collision c) {
        return (WhatIsPlayer.value & 1 << c.gameObject.layer) == 1 << c.gameObject.layer ||(WhatIsWall.value & 1 << c.gameObject.layer) == 1 << c.gameObject.layer;
    }

    public void sendHitAfterAnimation() {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, radius)){
            Hit hit;
            Rigidbody rb;
            if (hit = collider.GetComponent<Hit>())
            {
                hit.getHit(gameObject);
            }
            else if(rb = collider.GetComponent<Rigidbody>())
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }
        }
        Destroy(gameObject);
    }


}
