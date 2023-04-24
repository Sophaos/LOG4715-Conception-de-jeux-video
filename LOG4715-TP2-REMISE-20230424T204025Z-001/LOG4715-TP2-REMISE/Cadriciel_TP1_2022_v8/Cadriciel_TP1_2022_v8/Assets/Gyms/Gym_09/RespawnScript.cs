using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnScript : MonoBehaviour
{
    public GameObject player;
    public GameObject respawnPoint;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip DeathSound;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            audioSource.PlayOneShot(DeathSound);
            player.transform.position = respawnPoint.transform.position;

        }

    }
}
