using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerDashScript : MonoBehaviour
{
    // D�claration des constantes
    private static readonly Vector3 FlipRotation = new Vector3(0, 180, 0);
    private static readonly Vector3 CameraPosition = new Vector3(10, 1, 0);
    private static readonly Vector3 InverseCameraPosition = new Vector3(-10, 1, 0);

    // D�claration des variables
    bool _Grounded { get; set; }
    bool _Flipped { get; set; }
    Rigidbody _Rb { get; set; }
    Camera _MainCamera { get; set; }

    // Valeurs expos�es
    [SerializeField] float MoveSpeed = 4.0f;
    [SerializeField] LayerMask WhatIsGround;

    [Header("Jumping")]
    [SerializeField] float JumpForce = 7f;
    public float fallMultiplier = 4.5f;
    public float lowJumpMultiplier = 2f;
    

    [Header("Dashing")]
    [SerializeField] private float _dashingVelocity = 40f;
    [SerializeField] private float _dashingTime = 0.2f;
    private Vector3 _dashingDir;
    private bool _isDashing;
    public bool _canDash = true;
    public bool _dashedFromGround = false;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip[] JumpSounds;
    public AudioClip DashSound;

    private TrailRenderer _trailRenderer;


    // Awake se produit avait le Start. Il peut �tre bien de r�gler les r�f�rences dans cette section.
    void Awake()
    {
        //_Anim = GetComponent<Animator>();
        _Rb = GetComponent<Rigidbody>();
        _MainCamera = Camera.main;
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    // Utile pour r�gler des valeurs aux objets
    void Start()
    {
        _Grounded = false;
        _Flipped = false;
    }

    // V�rifie les entr�es de commandes du joueur
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal") * MoveSpeed;
        var dashInput = Input.GetMouseButtonDown(1);


        Dash(dashInput);
        if (_isDashing)
        {
            return;
        }
        HorizontalMove(horizontal);
        FlipCharacter(horizontal);
        CheckJump();
    }

    private IEnumerator StopDashing()
    {
        audioSource.PlayOneShot(DashSound);
        _Rb.useGravity = false;
        yield return new WaitForSeconds(_dashingTime);
        _Rb.velocity = Vector3.zero;
        _Rb.useGravity = true;
        _isDashing = false;
        _trailRenderer.emitting = false;
    }

    // G�re le mouvement horizontal
    void HorizontalMove(float horizontal)
    {
        _Rb.velocity = new Vector3(0, _Rb.velocity.y, horizontal * MoveSpeed);
    }

    private void Dash(bool dashInput)
    {
        if (_dashedFromGround && _Rb.velocity.y < 0)
        {
            _canDash = false;
        }
        else if (_dashedFromGround && _Rb.velocity.y >= 0)
        {
            _canDash = true;
        }
        if (dashInput && _canDash)
        {
            _isDashing = true;
            _canDash = false;
            _trailRenderer.emitting = true;
            var y = Input.GetAxisRaw("Vertical");
            var z = Input.GetAxisRaw("Horizontal");
            if (y > 0)
            {
                _Grounded = false;
            }
            if (y == 0 && z != 0 && _Grounded)
            {
                _dashedFromGround = true;
            }

            _dashingDir = new Vector3(0, y, z);
            if ((_dashingDir == Vector3.zero) && _Grounded)
            {
                _dashedFromGround = true;
            }
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
            StartCoroutine(StopDashing());
        }
        if (_Grounded)
        {
            _canDash = true;
        }

    }

    // G�re le saut du personnage, ainsi que son animation de saut
    void CheckJump()
    {
        if (_Grounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                audioSource.clip = JumpSounds[Random.Range(0, JumpSounds.Length)];
                audioSource.PlayOneShot(audioSource.clip);
                _Rb.velocity = new Vector3(0, 0, _Rb.velocity.z);
                _Rb.velocity += Vector3.up * JumpForce;
                _Grounded = false;
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

    // G�re l'orientation du joueur et les ajustements de la camera
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
        // On s'assure de bien �tre en contact avec le sol
        if ((WhatIsGround & (1 << coll.gameObject.layer)) == 0)
            return;

        // �vite une collision avec le plafond
        if (coll.relativeVelocity.y > 0)
        {
            _Grounded = true;
        }
    }
}