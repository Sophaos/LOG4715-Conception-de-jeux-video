using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class projectile_launch : MonoBehaviour
{
    [SerializeField] private GameObject projPrefab;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float shootTime;
    [SerializeField] private int ammo;
    private int maxAmmo;
    private TMP_Text ammoText;
    private float shootCounter;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip FireSound;

    void Start()
    {
        maxAmmo = ammo;
        ammoText = GameObject.Find("AmmoText").GetComponent<TMPro.TMP_Text>();
        ammoText.text = "Munitions : " + maxAmmo.ToString() + " / " + maxAmmo.ToString();
        shootCounter = shootTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && shootCounter <= 0 && ammo != 0)
        {
            gameObject.GetComponent<Animation_Test>().AttackAni();
            audioSource.PlayOneShot(FireSound);
            gameObject.GetComponent<CharacterMovement>().setIsShooting(true);
            Instantiate(projPrefab, launchPoint.position, Quaternion.identity);
            shootCounter = shootTime;
            ammo--;
            ammoText.text = "Munitions : " + ammo.ToString() + " / " + maxAmmo.ToString();
        }
        shootCounter -= Time.deltaTime;
    }

    public void reloadAmmo()
    {
        ammo = maxAmmo;
        ammoText.text = "Munitions : " + maxAmmo.ToString() + " / " + maxAmmo.ToString();
    }
}
