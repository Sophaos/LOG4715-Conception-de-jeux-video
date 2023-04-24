using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Playerhealth : MonoBehaviour
{
    // Start is called before the first frame update
    //private float MaxHealth = 100f;
    //public float PlayerHealth;
    //public AudioClip waterhitSound;
    [SerializeField] public float maxHealth;
    private AudioSource playerAudioSource;
    private bool isHealing = false;
    private bool isHurting = false;
    public GameObject player;
    private PlayerFullControllerScript playerFullControllerScript;
    public GameObject respawnPoint;
    public GameObject m_GotHurtScreen;
    public GameObject checkpointText;

    public bool BigMode = false; //Sera a true quand le joueur aura une forme agrandit

    private float nextActionTime = 0.0f;
    public float period = 4.0f;

    private float nextActionTime_1 = 0.0f;
    public float period_1 = 1.0f;

    public float currentHealth;
    void Start()
    {
        playerAudioSource = GetComponentInParent<AudioSource>();
        //PlayerHealth = MaxHealth;
        playerFullControllerScript = GetComponent<PlayerFullControllerScript>();
        currentHealth = maxHealth;

    }

    public void PlayerGetHitted(int value) {
        currentHealth -= value;
        //playerAudioSource.PlayOneShot(waterhitSound);

    }
    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            if (isHealing)
                Healing();
            if (isHurting)
                Hurting();
        }

        if (Time.time > nextActionTime_1)
        {
            nextActionTime_1 += period_1;
            if (isHealing)
                Healing();
            if (isHurting)
                Hurting();
        }

        //if (Input.GetButtonDown("L")) {
        //    BigMode = true;


        //}
    }

    public void Healing()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth = currentHealth + 25f;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
        }
    }
    public void InstantKill()
    {
      
            Debug.Log("Dead");
            currentHealth = currentHealth - 1;
            var color = m_GotHurtScreen.GetComponent<Image>().color;
            color.a = 0.2f;
            m_GotHurtScreen.GetComponent<Image>().color = color;
            StartCoroutine(FadingHurt());
            currentHealth = 0;
            if (currentHealth <= 0)
            {
                // todo respawn point
                // death animation

                player.transform.position = respawnPoint.GetComponent<RespawnScript>().respawnPoint.transform.position;
            currentHealth = maxHealth;
                //playerFullControllerScript.gameObject.GetComponent<Animation_Test>().DeathAni();
            }
            
    }

    public void Hurting()
    {
        if (currentHealth >= 0)
        {
            Debug.Log("hurting");
            currentHealth = currentHealth - 1f;
            var color = m_GotHurtScreen.GetComponent<Image>().color;
            color.a = 0.2f;
            m_GotHurtScreen.GetComponent<Image>().color = color;
            StartCoroutine(FadingHurt());
            if (currentHealth < 0f) currentHealth = 0f;
            if (currentHealth <= 0f)
            {
                // todo respawn point
                // death animation

                player.transform.position = respawnPoint.GetComponent<RespawnScript>().respawnPoint.transform.position;
                currentHealth = maxHealth;
                //playerFullControllerScript.gameObject.GetComponent<Animation_Test>().DeathAni();
            }
        }
    }

    public void Hurting(float damage)
    {
        if (currentHealth >= 0)
        {
            Debug.Log("hurting");
            currentHealth = currentHealth - damage;
            var color = m_GotHurtScreen.GetComponent<Image>().color;
            color.a = 0.2f;
            m_GotHurtScreen.GetComponent<Image>().color = color;
            StartCoroutine(FadingHurt());
            if (currentHealth < 0f) currentHealth = 0f;
            if (currentHealth <= 0f)
            {
                // todo respawn point
                // death animation

                player.transform.position = respawnPoint.GetComponent<RespawnScript>().respawnPoint.transform.position;
                currentHealth = maxHealth;
                //playerFullControllerScript.gameObject.GetComponent<Animation_Test>().DeathAni();
            }
        }
    }

    private IEnumerator FadingHurt()
    {
        Debug.Log("fading");
        var color = m_GotHurtScreen.GetComponent<Image>().color;
        color.a -= 0.05f;
        m_GotHurtScreen.GetComponent<Image>().color = color;
        yield return new WaitForSeconds(0.25f);
        color.a -= 0.05f;
        m_GotHurtScreen.GetComponent<Image>().color = color;
        yield return new WaitForSeconds(0.25f);
        color.a -= 0.05f;
        m_GotHurtScreen.GetComponent<Image>().color = color;
        yield return new WaitForSeconds(0.25f);
        color.a -= 0.05f;
        m_GotHurtScreen.GetComponent<Image>().color = color;
    }

    void OnCollisionEnter(Collision coll)
    {

        if (coll.gameObject.tag == "Lava_Surface")
        {
            isHealing = true;
            isHurting = false;

        }
        else
        {
            isHealing = false;
            isHurting = true;
        }
    }

}
