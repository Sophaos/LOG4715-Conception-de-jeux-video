using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnScript : MonoBehaviour
{
    public GameObject player;
    public GameObject respawnPoint;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip _checkPoint;
    private bool canDeblockCheckpoint = true;
   
    IEnumerator WaitCheckpoint(GameObject player)
    {
        yield return new WaitForSeconds(3);
        player.GetComponent<Playerhealth>().checkpointText.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (canDeblockCheckpoint) {
                canDeblockCheckpoint = false;
            audioSource.PlayOneShot(_checkPoint);
            player.GetComponent<Playerhealth>().respawnPoint = gameObject;
                player.GetComponent<Playerhealth>().checkpointText.SetActive(true);
                StartCoroutine(WaitCheckpoint(player));
            }
        }

    }
}
