using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class projectile_launch : MonoBehaviour
{
    [SerializeField] private GameObject projPrefab;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float shootTime;
    [SerializeField] public float ammo;
    public Image[] fireballs;
    public float maxAmmo;
    private float shootCounter;
    private bool isReloading = false;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip FireSound;

    void Start()
    {
        maxAmmo = ammo;
        shootCounter = shootTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.isPaused) {
            if (Input.GetButtonDown("Fire1") && shootCounter <= 0 && ammo != 0)
            {

                StartCoroutine(ShootEffect());
                this.gameObject.GetComponent<PlayerFullControllerScript>().setIsShooting(true);

                Instantiate(projPrefab, launchPoint.position, Quaternion.identity);
                shootCounter = shootTime;
                ammo--;

                fireballs[(int)ammo].enabled = false;
            }
            shootCounter -= Time.deltaTime;
        }
    }

    public void reloadAmmo()
    {
        if (ammo < maxAmmo)
        {
            isReloading = true;
            fireballs[(int)ammo].enabled = true;
            ammo++;
        }
    }

    private IEnumerator ShootEffect()
    {
        this.gameObject.GetComponent<Animation_Test>().AttackAni();
        audioSource.PlayOneShot(FireSound);
        yield return new WaitForSeconds(shootTime);
        this.gameObject.GetComponent<Animation_Test>().IdleAni();
        this.gameObject.GetComponent<PlayerFullControllerScript>().setIsShooting(false);
    }

    void OnCollisionEnter(Collision coll)
    {

        if (coll.gameObject.tag == "Lava_Surface")
        {
            if (!isReloading)
                InvokeRepeating("reloadAmmo", 1.0f, 1f);
        } else
        {
            CancelInvoke("reloadAmmo");
            isReloading = false;
        }
    }
}