using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserControl : MonoBehaviour
{
    private bool m_Jump;
    public AudioSource audioSource;
    public AudioClip JumpSound;
    public CharacterMovement m_Character;
    float moveSpeed = 40f;
    float horizontalMove = 0f;

    private void Awake()
    {
        m_Character = GetComponent<CharacterMovement>();
    }


    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * moveSpeed;
        if (!m_Jump)
        {
            // Read the jump input in Update so button presses aren't missed.
            m_Jump = Input.GetButtonDown("Jump");
            if (m_Jump && m_Character.getGrounded()) { audioSource.PlayOneShot(JumpSound); }
        }
    }


    private void FixedUpdate()
    {
        m_Character.Move(horizontalMove * Time.fixedDeltaTime, false, m_Jump);
        m_Jump = false;
    }
}

