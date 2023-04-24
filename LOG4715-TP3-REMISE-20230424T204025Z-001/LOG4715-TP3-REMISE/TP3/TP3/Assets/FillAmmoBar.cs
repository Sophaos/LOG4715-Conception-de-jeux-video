using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillAmmoBar : MonoBehaviour
{
    public projectile_launch projectile_launch;
    public Image fillImage;
    private Slider slider;

    // Start is called before the first frame update


    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        float fillValue = projectile_launch.ammo / projectile_launch.maxAmmo;

        slider.value = fillValue;
    }
}
