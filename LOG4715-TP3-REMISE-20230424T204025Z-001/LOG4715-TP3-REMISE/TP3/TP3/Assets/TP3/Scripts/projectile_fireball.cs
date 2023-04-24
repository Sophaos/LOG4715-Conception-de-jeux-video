using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile_fireball : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed;
    [SerializeField] private float projectileLife;
    [SerializeField] private PlayerFullControllerScript playerFullControllerScript;

    private bool _flipped;

    void Start()
    {
        playerFullControllerScript = GameObject.Find("StoneMonster").GetComponent<PlayerFullControllerScript>();
        _flipped = playerFullControllerScript._Flipped;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1, _flipped ? transform.position.z - 2 : transform.position.z + 2);
        transform.rotation = Quaternion.Euler(0, _flipped ? 90 : -90, 0);
    }

    // Update is called once per frame
    void Update()
    {
        projectileLife -= Time.deltaTime;
        if (projectileLife <= 0) Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        rb.velocity = _flipped ? new Vector3(0, 0, -speed * 2) : new Vector3(0, 0, speed * 2);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            Destroy(gameObject);
        }
    }
}
