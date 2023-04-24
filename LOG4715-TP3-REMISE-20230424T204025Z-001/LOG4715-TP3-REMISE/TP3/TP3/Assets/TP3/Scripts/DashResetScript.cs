using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashResetScript : MonoBehaviour
{
    // Déclaration des constantes
    private static readonly Vector3 FlipRotation = new Vector3(0, 180, 0);
    private static readonly Vector3 CameraPosition = new Vector3(10, 1, 0);
    private static readonly Vector3 InverseCameraPosition = new Vector3(-10, 1, 0);

    // Déclaration des variables
    bool _Grounded { get; set; }
    bool _Flipped { get; set; }
    Animator _Anim { get; set; }
    Rigidbody _Rb { get; set; }
    Camera _MainCamera { get; set; }

    // Valeurs exposées
    [SerializeField]
    float MoveSpeed = 4.0f;

    [SerializeField]
    float JumpForce = 7f;

    [SerializeField]
    LayerMask WhatIsGround;

    [Header("Dashing")]
    [SerializeField] private float _dashingVelocity = 40f;
    [SerializeField] private float _dashingTime = 0.2f;
    private Vector3 _dashingDir;
    private bool _isDashing;
    public bool _canDash = true;

    public float fallMultiplier = 4.5f;
    public float lowJumpMultiplier = 2f;


    // Awake se produit avait le Start. Il peut être bien de régler les références dans cette section.
    void Awake()
    {
        _Anim = GetComponent<Animator>();
        _Rb = GetComponent<Rigidbody>();
        _MainCamera = Camera.main;
    }

    // Utile pour régler des valeurs aux objets
    void Start()
    {
        _Grounded = false;
        _Flipped = false;
    }

    // Vérifie les entrées de commandes du joueur
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal") * MoveSpeed;
        var dashInput = Input.GetButtonDown("Dash");

        if (dashInput && _canDash)
        {
            _isDashing = true;
            _canDash = false;
            Dash();
            StartCoroutine(StopDashing());
        }


        if (_isDashing)
        {
            return;
        }

        if (_Grounded)
        {
            _canDash = true;
        }

        HorizontalMove(horizontal);
        FlipCharacter(horizontal);
        CheckJump();
    }

    private IEnumerator StopDashing()
    {
        _Rb.useGravity = false;
        yield return new WaitForSeconds(_dashingTime);
        _Rb.velocity = Vector3.zero;
        _Rb.useGravity = true;
        _isDashing = false;
    }

    // Gère le mouvement horizontal
    void HorizontalMove(float horizontal)
    {
        _Rb.velocity = new Vector3(0, _Rb.velocity.y, horizontal * MoveSpeed);
        _Anim.SetFloat("MoveSpeed", Mathf.Abs(horizontal));
    }

    private void Dash()
    {
        var y = Input.GetAxisRaw("Vertical");
        var z = Input.GetAxisRaw("Horizontal");
        _dashingDir = new Vector3(0, y, z);
        if (_dashingDir == Vector3.zero)
        {
            if (!_Flipped)
            {
                _dashingDir = new Vector3(0, 0, transform.localScale.z);
            }
            else
            {
                _dashingDir = new Vector3(0, 0, -transform.localScale.z);
            }
            _Rb.velocity = Vector3.zero;
            _Rb.velocity += _dashingDir.normalized * _dashingVelocity;
        }
        else
        {
            _Rb.velocity = Vector3.zero;
            _Rb.velocity += new Vector3(0, y, z).normalized * _dashingVelocity;
        }

    }

    // Gère le saut du personnage, ainsi que son animation de saut
    void CheckJump()
    {
        if (_Grounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                _Rb.velocity = new Vector3(0, 0, _Rb.velocity.z);
                _Rb.velocity += Vector3.up * JumpForce;
                _Grounded = false;
                _Anim.SetBool("Grounded", false);
                _Anim.SetBool("Jump", true);
            }
        }
        if (_Rb.velocity.y < 0)
        {
            _Rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (_Rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            _Rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    // Gère l'orientation du joueur et les ajustements de la camera
    void FlipCharacter(float horizontal)
    {
        if (horizontal < 0 && !_Flipped)
        {
            _Flipped = true;
            transform.Rotate(FlipRotation);
            _MainCamera.transform.Rotate(-FlipRotation);
            _MainCamera.transform.localPosition = InverseCameraPosition;
        }
        else if (horizontal > 0 && _Flipped)
        {
            _Flipped = false;
            transform.Rotate(-FlipRotation);
            _MainCamera.transform.Rotate(FlipRotation);
            _MainCamera.transform.localPosition = CameraPosition;
        }
    }

    // Collision avec le sol
    void OnCollisionEnter(Collision coll)
    {
        // On s'assure de bien être en contact avec le sol
        if ((WhatIsGround & (1 << coll.gameObject.layer)) == 0)
            return;

        // Évite une collision avec le plafond
        if (coll.relativeVelocity.y > 0)
        {
            _Grounded = true;
            _Anim.SetBool("Grounded", _Grounded);
        }
    }
}