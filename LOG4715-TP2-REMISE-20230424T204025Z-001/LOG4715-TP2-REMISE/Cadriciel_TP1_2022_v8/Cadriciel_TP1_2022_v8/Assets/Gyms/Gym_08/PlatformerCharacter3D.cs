using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerCharacter3D : MonoBehaviour
{
    [SerializeField] private float m_MaxSpeed = 10f;                    
    [SerializeField] private float m_JumpForce = 300f;                  
    [SerializeField] private bool m_AirControl = true;     
    
    [SerializeField] private LayerMask m_WhatIsGround;                  
    [SerializeField] private LayerMask m_WhatIsWall;

    private bool m_Grounded;
    private Transform m_GroundCheck, m_FrontWallCheck, m_BackWallCheck;
    [SerializeField] private float m_WallJumpTimer = 5f;
    private bool m_TouchesWall = false;     
    private bool m_IsWallJumping = false;   
    private bool m_IsBackToTheWall = false;
    private float m_WallJumpLerp = 10f;


    
    private Animator m_Anim;    
    private Rigidbody m_Rigidbody;
    private bool m_FacingRight = true; 
    private int jumpCount = 0;
    private int n_wallJump = 0;


    private void Awake()
    {
        m_GroundCheck = transform.Find("GroundCheck");
        m_FrontWallCheck = transform.Find("FrontWallCheck");
        m_BackWallCheck = transform.Find("BackWallCheck");
        m_Anim = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
        m_Rigidbody.freezeRotation = true; 
    }

    private void FixedUpdate()
    {
        //
        m_Grounded = false;
        m_TouchesWall = false;
        m_IsBackToTheWall = false;
        Collider[] colliders = Physics.OverlapSphere(m_GroundCheck.position, .2f, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                m_Grounded = true;
        }
        var wallCollider = Physics.OverlapSphere(m_FrontWallCheck.position, .2f, m_WhatIsWall);
        for (int i = 0; i < wallCollider.Length; i++)
        {
            if (wallCollider[i].gameObject != gameObject)
            {
                m_TouchesWall = true;
                m_IsBackToTheWall = false;
                break;
            } 
        }
        var backWallCollider = Physics.OverlapSphere(m_BackWallCheck.position, .7f, m_WhatIsWall);
        for (int i = 0; i < backWallCollider.Length; i++)
        {
            if (backWallCollider[i].gameObject != gameObject)
            {
                m_TouchesWall = true;
                m_IsBackToTheWall = true;
                break;
            }
        }
        //Animations
        m_Anim.SetBool("Ground", m_Grounded);
        m_Anim.SetFloat("vSpeed", m_Rigidbody.velocity.y);
    }

    public bool getGrounded() { return m_Grounded; }
    public bool getFacingRight() { return m_FacingRight; }

    private IEnumerator WallJumpTimer()
    {
        m_IsWallJumping = true;
        yield return new WaitForSeconds(m_WallJumpTimer);
        m_IsWallJumping = false;
    }


    public void Move(float move, bool crouch, bool jump)
    {
        //only control the player if grounded or airControl is turned on
        if ((m_Grounded || m_AirControl) && !m_IsWallJumping)
        {
            // The Speed animator parameter is set to the absolute value of the horizontal input.
            m_Anim.SetFloat("Speed", Mathf.Abs(move));
            // Move the character
            m_Rigidbody.velocity = new Vector3(move * m_MaxSpeed, m_Rigidbody.velocity.y, 0);
            if ((move > 0 && !m_FacingRight) || (move < 0 && m_FacingRight)) Flip();

        }
        // If the player should jump...
        if (m_Grounded && jump && m_Anim.GetBool("Ground") && jumpCount<2)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Anim.SetBool("Ground", false);
            m_Rigidbody.AddForce(new Vector3(0f, m_JumpForce,0));
            jumpCount++;
        }
        else if (m_TouchesWall && !m_Grounded && jump && n_wallJump <100)
        {
            m_Rigidbody.velocity = Vector3.zero;
            Vector3 pushbackDirection = m_FacingRight ? Vector3.left : Vector3.right;
            var backWallCollider = Physics.OverlapSphere(m_BackWallCheck.position, .7f, m_WhatIsWall);
            for (int i = 0; i < backWallCollider.Length; i++)
            {
                if (backWallCollider[i].gameObject != gameObject)
                {
                    m_TouchesWall = true;
                    m_IsBackToTheWall = true;
                    break;
                }
            }
            if (m_IsBackToTheWall)
            {
                pushbackDirection = m_FacingRight ? Vector3.right : Vector3.left;
            }
            //Vector3 ForceApplied = pushbackDirection * 400f + Vector3.up * m_JumpForce;
            //m_Rigidbody.AddForce(pushbackDirection * 400f);
            //m_Rigidbody.AddForce(Vector3.up * m_JumpForce);
            Vector3 HorizontalPush = pushbackDirection * 40f;
            Vector3 VerticalPush = new Vector3(0, 35f, 0);
            Vector3 finalVelocity = HorizontalPush + VerticalPush;
            m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, finalVelocity, m_WallJumpLerp * Time.deltaTime);
            if (!m_IsBackToTheWall)
                Flip();
            StartCoroutine(WallJumpTimer());
            n_wallJump++;
        }
        if (m_Grounded)
        {
            // reset number of jumps when 
            jumpCount = 0;
            n_wallJump = 0;
        }
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
