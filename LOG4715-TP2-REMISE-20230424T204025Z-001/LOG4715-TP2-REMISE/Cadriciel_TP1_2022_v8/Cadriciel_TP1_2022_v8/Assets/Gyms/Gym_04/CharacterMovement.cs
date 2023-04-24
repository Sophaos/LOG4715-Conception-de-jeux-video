using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float m_MaxSpeed = 10f;
    [SerializeField] private float m_JumpForce = 525f;
    [SerializeField] private LayerMask m_WhatIsGround;
    private bool m_AirControl = true;
    private bool m_Grounded;
    private Transform m_GroundCheck;
    private Animator m_Anim;
    private Rigidbody m_Rigidbody;
    private bool m_FacingRight = true;
    private int jumpCount = 0;
    private bool isShooting = false;

    private void Awake()
    {
        m_GroundCheck = transform.Find("GroundCheck");
        m_Anim = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
        m_Rigidbody.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        m_Grounded = false;
        Collider[] colliders = Physics.OverlapSphere(m_GroundCheck.position, .2f, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                m_Grounded = true;
        }
        //Animations
        m_Anim.SetBool("Ground", m_Grounded);
        m_Anim.SetFloat("vSpeed", m_Rigidbody.velocity.y);
    }

    public bool getGrounded() { return m_Grounded; }
    public bool getFacingRight() { return m_FacingRight; }


    public void Move(float move, bool crouch, bool jump)
    {
        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            // The Speed animator parameter is set to the absolute value of the horizontal input.
            if (move != 0 && !getIsShooting())
            {
                gameObject.GetComponent<Animation_Test>().RunAni();
            }
            else if (move == 0 && !getIsShooting()) {
                gameObject.GetComponent<Animation_Test>().IdleAni();
            }
            // Move the character
            m_Rigidbody.velocity = new Vector3(move * m_MaxSpeed, m_Rigidbody.velocity.y, 0);
            if ((move > 0 && !m_FacingRight) || (move < 0 && m_FacingRight)) Flip();

        }
        // If the player should jump...
        if (m_Grounded && jump && m_Anim.GetBool("Ground") && jumpCount < 2)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Anim.SetBool("Ground", false);
            m_Rigidbody.AddForce(new Vector3(0f, m_JumpForce, 0));
            jumpCount++;
        }
        if (m_Grounded)
        {
            // reset number of jumps when 
            jumpCount = 0;
        }
    }

    public bool getIsShooting() {
        return isShooting;
    }

    public void setIsShooting(bool val)
    {
        isShooting = val;
    }

    public void setIsShootingToFalse()
    {
        isShooting = false;
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;
        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.z *= -1;
        transform.localScale = theScale;
    }
}
