using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerhealth : MonoBehaviour
{
    // Start is called before the first frame update
    private float MaxHealth = 100f;
    public float PlayerHealth;
    public AudioClip waterhitSound;
   
    private AudioSource playerAudioSource;
    public bool BigMode = false; //Sera a true quand le joueur aura une forme agrandit
    void Start()
    {
        playerAudioSource = GetComponentInParent<AudioSource>();
        PlayerHealth = MaxHealth;
    
    }

    public void PlayerGetHitted(int value) {
        PlayerHealth -= value;
        playerAudioSource.PlayOneShot(waterhitSound);
       
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("L")) {
            BigMode = true;


        }
    }
}
