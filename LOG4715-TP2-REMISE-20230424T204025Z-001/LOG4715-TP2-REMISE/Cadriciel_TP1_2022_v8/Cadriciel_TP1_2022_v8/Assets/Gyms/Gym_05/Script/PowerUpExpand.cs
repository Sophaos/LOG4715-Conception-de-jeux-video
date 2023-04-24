using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpExpand : PowerUP
{
    [SerializeField]
    float sizeMultiplier = 3f;

    public override IEnumerator PickUpPowerUp(GameObject player)
    {

        StoneMonsterController c = player.GetComponent<StoneMonsterController>();
        c.setIsExpanded(true);
        
        if (c.getIsShrinked())
        {
            c.setIsShrinked(false);
        }

        player.transform.localScale = Vector3.one;
        player.transform.localScale *= sizeMultiplier;

        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;

        //wait 10 seconds
        yield return new WaitForSeconds(PowerUpTime);
        
        //on verifie si on est toujours expaned
        if (c.getIsExpanded())
        {
            player.transform.localScale = Vector3.one;
        }
        
        c.setIsExpanded(false);


        Destroy(gameObject);
    }
}
