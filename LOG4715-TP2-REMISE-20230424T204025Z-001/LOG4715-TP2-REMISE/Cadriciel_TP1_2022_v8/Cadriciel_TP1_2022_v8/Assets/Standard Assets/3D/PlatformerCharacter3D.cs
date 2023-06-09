using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerCharacter3D : MonoBehaviour
{
    [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField] private float m_JumpForce = 650f;                  // Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

    private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private Transform m_CeilingCheck;   // A position marking where to check for ceilings
    const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
    private Animator m_Anim;            // Reference to the player's animator component.
    private Rigidbody m_Rigidbody;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private int jumpCount = 0;
    public Transform positionRecast;
    
    public bool OnLava ,OnWater,OnGround = false;
    private void Awake()
    {
        // Setting up references.
        m_GroundCheck = transform.Find("GroundCheck");
        m_CeilingCheck = transform.Find("CeilingCheck");
        m_Anim = GetComponent<Animator>();
        m_Rigidbody = GetComponentInParent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        m_Grounded = false;
        CheckGround();
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        // If the character has a ceiling preventing them from standing up, keep them crouching
        if (Physics.CheckSphere(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround))
        {
            m_Grounded = true;
           
        }
        m_Anim.SetBool("Ground", m_Grounded);
        if (m_Grounded) {
            jumpCount = 0;


        }
        // Set the vertical animation
        m_Anim.SetFloat("vSpeed", m_Rigidbody.velocity.y);
    }

    public bool getGrounded() {
        return m_Grounded;
    }

    public bool getFacingRight() { return m_FacingRight; }

    public void Move(float move, bool crouch, bool jump)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch && m_Anim.GetBool("Crouch"))
        {
     
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics.CheckSphere(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        // Set whether or not the character is crouching in the animator
        m_Anim.SetBool("Crouch", crouch);

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            // Reduce the speed if crouching by the crouchSpeed multiplier
            move = (crouch ? move * m_CrouchSpeed : move);

            // The Speed animator parameter is set to the absolute value of the horizontal input.
            m_Anim.SetFloat("Speed", Mathf.Abs(move));

            // Move the character
            m_Rigidbody.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody.velocity.y);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
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
    }
    /// <summary>
    /// Permet de checker le type de sol sur lequel on marche
    /// </summary>
    /// <returns></returns>
    bool CheckGround()
    {
        Vector3 pos = positionRecast.position;
        RaycastHit hit;

        if (Physics.Raycast(pos, -transform.up, out hit, 1.8f, m_WhatIsGround))
        {
          
            if (hit.collider.gameObject.tag =="Lava_Surface" )
            {
                m_MaxSpeed = 10f;
               
            }
         else if (hit.collider.gameObject.tag == "Water_Surface")
            {
                m_MaxSpeed = 2f;
              
            }
            else if (hit.collider.gameObject.tag == "Default_Surface")
            {
                m_MaxSpeed = 5f;
               
            }
            Debug.DrawLine(pos, hit.point, Color.red, 10.0f);
            return false;
        }
        //"Il n'y a pas d'obstacle"
        return true;
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
