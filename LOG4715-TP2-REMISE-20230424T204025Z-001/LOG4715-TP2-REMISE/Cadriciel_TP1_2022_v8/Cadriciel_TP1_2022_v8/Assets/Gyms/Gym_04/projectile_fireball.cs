using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile_fireball : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed;
    [SerializeField] private float projectileLife;
    [SerializeField] private CharacterMovement playerMovement;
    private bool facingRight;

    void Start()
    {
        var playerMovement = GameObject.Find("StoneMonster").GetComponent<CharacterMovement>();
        facingRight = playerMovement.getFacingRight();
        if (!facingRight)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        projectileLife -= Time.deltaTime;
        if (projectileLife <= 0) Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        rb.velocity = facingRight? new Vector3(speed, rb.velocity.y, 0) : new Vector3(-speed, rb.velocity.y, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            Destroy(gameObject);
        }
    }
}
