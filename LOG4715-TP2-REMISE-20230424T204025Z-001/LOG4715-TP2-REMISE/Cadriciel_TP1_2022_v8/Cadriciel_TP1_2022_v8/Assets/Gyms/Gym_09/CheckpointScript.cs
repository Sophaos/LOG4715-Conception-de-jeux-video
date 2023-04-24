using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    private RespawnScript respawn;
    private BoxCollider _collider;
    public ParticleSystem partSys;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip CheckpointGrantedSound;

    private void Awake()
    {
        respawn = GameObject.FindWithTag("Respawn").GetComponent<RespawnScript>();
        _collider = GetComponent<BoxCollider>();
        partSys.Stop();


    }
    // Start is called before the first frame update
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
            partSys.Play();
            respawn.respawnPoint = this.gameObject;
            _collider.enabled = false;
            audioSource.PlayOneShot(CheckpointGrantedSound);
        }
    }
}
