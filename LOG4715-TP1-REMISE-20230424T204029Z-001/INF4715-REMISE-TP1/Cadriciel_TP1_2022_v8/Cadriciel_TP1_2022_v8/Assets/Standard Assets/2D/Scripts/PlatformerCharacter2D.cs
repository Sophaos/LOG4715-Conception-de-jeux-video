using System;
using System.Collections;
using UnityEngine;

#pragma warning disable 649
namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour
    {
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [SerializeField] private uint m_MaxJump = 2;                        // The max number of jumps allowed;
        [SerializeField] private float m_WallPushBackForce = 400f;          // The force used to push the user when wall jumping

        [Range(0, 1)][SerializeField] private float m_CrouchSpeed = .36f;   // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
        [SerializeField] private LayerMask m_WhatIsWall;                    // A mask determining what is wall to the character
        [SerializeField] private float m_MaxChargedJumpForce = 800f;        // The max charged jump value
        [SerializeField] private float m_WallJumpTimer = .3f;               // The amount of time for a complete wall jump

        // Ground
        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        
        // Ceiling
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up

        // Wall
        private Transform m_WallCheck;          // A position marking where to check for walls / platforms
        private Transform m_BackWallCheck;          // A position marking where to check for walls / platforms
        const float k_WallCheckRadius = .3f;    // Radius of the overlap circle to determine if the player touches the wall
        private bool m_TouchesWall = false;     // Detect if the character touches a wall / platform
        private bool m_IsWallJumping = false;   // Boolean to know if the character is currently wall jumping
        private bool m_IsBackToTheWall = false;

        // Visual
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.

        // Jump
        private uint n_jump = 0;        // Current number of jumps
       
        // Power jump
        private float m_ChargeStartTime = -1f;      // timer used for power jump
        private float m_chargeJumpIncrement;        // the increment value for charge jumping
        private float k_TimeNeedToMaxJump = 8f;     // the increment need to reach the max value, it will take 8 seconds
        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_WallCheck = transform.Find("WallCheck");
            m_BackWallCheck = transform.Find("BackWallCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();

            m_chargeJumpIncrement = m_MaxChargedJumpForce / k_TimeNeedToMaxJump;
        }


        private void FixedUpdate()
        {
            m_Grounded = false;
            m_TouchesWall = false;
            m_IsBackToTheWall = false;
            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                    m_Grounded = true;
            }

            // Detect wall collision
            var wallCollider = Physics2D.OverlapCircle(m_WallCheck.position, k_WallCheckRadius, m_WhatIsWall);
            var backWallCollider = Physics2D.OverlapCircle(m_BackWallCheck.position, k_WallCheckRadius, m_WhatIsWall);
            if (wallCollider || backWallCollider)
            {
                m_TouchesWall = true;
            }
            if (backWallCollider)
            {
                m_IsBackToTheWall = true;
            }
               
            m_Anim.SetBool("Ground", m_Grounded);

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
        }


        public void Move(float move, bool crouch, bool jump, bool isCharging)
        {
            // If crouching, check to see if the character can stand up
            if (!crouch && m_Anim.GetBool("Crouch"))
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    crouch = true;
                }
            }

            // Set whether or not the character is crouching in the animator
            m_Anim.SetBool("Crouch", crouch);

            // only control the player if grounded or airControl is turned on
            // and is not wall jummping
            if ((m_Grounded || m_AirControl) && !m_IsWallJumping)
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                move = (crouch ? move * m_CrouchSpeed : move);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(move));

                // Move the character
                m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody2D.velocity.y);

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
            // if the character is charging
            if (isCharging && m_Grounded)
            { 
                // the first tick will start the timer 
                if (m_ChargeStartTime < 0)
                {
                    m_ChargeStartTime = Time.time;
                    Debug.Log("Charging !");
                }
            }
            // if the character release the charge (since the charge time has started)
            else if (m_Grounded && m_ChargeStartTime > 0)
            {
                float timeSinceCharge = Time.time - m_ChargeStartTime;
                float totalJumpExpectedValue = m_JumpForce + (m_chargeJumpIncrement * timeSinceCharge);     // min could be m_JumpForce ( * 0 (sec))
                float totalJumpPowerActualValue = Math.Min(totalJumpExpectedValue, m_MaxChargedJumpForce);  // select the min value (max or current jump power)
                m_Rigidbody2D.AddForce(Vector2.up * totalJumpPowerActualValue);
                Debug.Log("Release charge : " + totalJumpPowerActualValue + "!");
                m_ChargeStartTime = -1f; // reset timer
            }
            else if (jump)
            {
                // can only wall jump when not grounded and touching wall
                if (m_TouchesWall && !m_Grounded)
                {
                    m_Rigidbody2D.velocity = Vector2.zero;

                    Vector2 pushbackDirection = m_FacingRight ? Vector2.left : Vector2.right;
                    if (m_IsBackToTheWall)
                    {
                        pushbackDirection = m_FacingRight ? Vector2.right : Vector2.left;
                    } 
                    m_Rigidbody2D.AddForce(pushbackDirection * m_WallPushBackForce);
                    m_Rigidbody2D.AddForce(Vector2.up * m_JumpForce);
                    Debug.Log("Wall Jump !");
                    if (!m_IsBackToTheWall)
                        Flip();
                    StartCoroutine(WallJumpTimer());
                }
                // if cant wall jump then jump
                else if (++n_jump < m_MaxJump)
                {
                    Debug.Log("Normal Jump !");
                    m_Grounded = false;
                    m_Anim.SetBool("Ground", false);
                    m_Rigidbody2D.AddForce(Vector2.up * m_JumpForce);
                }
            }
            
            if (m_Grounded)
            {
                // reset number of jumps when 
                n_jump = 0;
            }
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

        // set and reset the wall jump timer
        private IEnumerator WallJumpTimer()
        {
            m_IsWallJumping = true;
            yield return new WaitForSeconds(m_WallJumpTimer);
            m_IsWallJumping = false;
        }

        // if grounded (used for AutoCam)
        public bool getGrounded()
        {
            return m_Grounded;
        }
    }
}
