using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashResetShardScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    private PlayerFullControllerScript _playerControllerFullScript;
    
    //public MeshRenderer initial;
    //public MeshRenderer used;
    public float _dashResetRespawnTime = 2.0f;
    public MeshRenderer _renderer;
    private SphereCollider _collider;

    [Header("Sound")]
    private AudioSource _audioSource;
    public AudioClip DashResetPickedUp;
    public AudioClip DashResetRespawn;
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
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
            _playerControllerFullScript = other.GetComponent<PlayerFullControllerScript>();
            _playerControllerFullScript._canDash = true;
            _playerControllerFullScript._dashedFromGround = false;
            StartCoroutine(RespawnDashReset());
        }

    }

    private IEnumerator RespawnDashReset()
    {
        _renderer.enabled = false;
        _collider.enabled = false;
        _audioSource.PlayOneShot(DashResetPickedUp);
        yield return new WaitForSeconds(_dashResetRespawnTime);
        _renderer.enabled = true;
        _collider.enabled = true;
        _audioSource.PlayOneShot(DashResetRespawn);
    }
}
