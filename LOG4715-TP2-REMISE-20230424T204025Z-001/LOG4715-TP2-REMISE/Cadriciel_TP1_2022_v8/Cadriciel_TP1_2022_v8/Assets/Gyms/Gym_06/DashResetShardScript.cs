using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashResetShardScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    private PlayerControllerDashResetScript dashResetScript;
    
    //public MeshRenderer initial;
    //public MeshRenderer used;
    public float _dashResetRespawnTime = 2.0f;
    public MeshRenderer _renderer;
    private SphereCollider _collider;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip DashResetPickedUp;
    public AudioClip DashResetRespawn;
    void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _renderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            dashResetScript = other.GetComponent<PlayerControllerDashResetScript>();
            dashResetScript._canDash = true;
            dashResetScript._dashedFromGround = false;
            StartCoroutine(RespawnDashReset());
        }

    }

    private IEnumerator RespawnDashReset()
    {
        _renderer.enabled = false;
        _collider.enabled = false;
        audioSource.PlayOneShot(DashResetPickedUp);
        yield return new WaitForSeconds(_dashResetRespawnTime);
        _renderer.enabled = true;
        _collider.enabled = true;
        audioSource.PlayOneShot(DashResetRespawn);
    }
}
