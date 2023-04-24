using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUP : MonoBehaviour
{


    [SerializeField]
    protected LayerMask WhatIsPlayer;

    [SerializeField]
    protected LayerMask WhatIsPlayerInvicible;


    [SerializeField]
    protected Camera playerCamera;

    [SerializeField]
    protected float PowerUpTime = 10f;

    private void OnTriggerEnter(Collider other)
    {
        LayerMask mask = other.gameObject.layer;
        if ((WhatIsPlayer.value & 1 << mask) == 1 << mask || (WhatIsPlayerInvicible.value & 1 << mask) == 1 << mask)
        {
            StartCoroutine(PickUpPowerUp(other.gameObject));
        }

    }

    public abstract IEnumerator PickUpPowerUp(GameObject player);
}
