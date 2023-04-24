using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PowerUpSrink : PowerUP
{
    [SerializeField]
    float sizeMultiplier = 3f;

    public override IEnumerator PickUpPowerUp(GameObject player)
    {
        PlayerFullControllerScript c = player.GetComponent<PlayerFullControllerScript>();
       
        if (c.getIsExpanded())
        {
            c.setIsExpanded(false);
        }
        c.setIsShrinked(true);
        
        player.transform.localScale = Vector3.one;  
        player.transform.localScale /= sizeMultiplier;
       
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
        
        //wait 10 seconds
        yield return new WaitForSeconds(PowerUpTime);

        //on verifie si on est toujours shrinked
        if (c.getIsShrinked()) {
            c.setShrinkTimeDone(true);
            c.undoShrinkIfPlayerCan();
        }

        Destroy(gameObject);
    }
}
