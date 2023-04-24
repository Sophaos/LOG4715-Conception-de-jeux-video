using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reload_handler : MonoBehaviour
{
    public void reloadAmmo()
    {
        projectile_launch projLaunch = GameObject.Find("StoneMonster").GetComponent<projectile_launch>();
        projLaunch.reloadAmmo();
    }
}
