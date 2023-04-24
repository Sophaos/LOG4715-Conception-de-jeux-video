using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpInvicible : PowerUP
{
    public override IEnumerator PickUpPowerUp(GameObject player)
    {
        player.layer = LayerMask.NameToLayer("PlayerInvicible");

        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;

        //wait 10 seconds
        yield return new WaitForSeconds(10f);
        player.layer = LayerMask.NameToLayer("Player");
        Destroy(gameObject);
    }
}
