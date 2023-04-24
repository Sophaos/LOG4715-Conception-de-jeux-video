using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpeed : PowerUP
{

    [SerializeField]
    float speedMultiplier = 2f;


    public override IEnumerator PickUpPowerUp(GameObject player)
    {
        StoneMonsterController c = player.GetComponent<StoneMonsterController>();
        float currentSpeed = c.getSpeed();

        c.changeSpeed(currentSpeed * speedMultiplier);

        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;

        //wait 10 seconds
        yield return new WaitForSeconds(PowerUpTime);

        currentSpeed = c.getSpeed();
        c.changeSpeed(currentSpeed / speedMultiplier);


        Destroy(gameObject);
    }
}
