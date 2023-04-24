using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFullControllerScript : MonoBehaviour
{
    private Playerhealth _playerhealth;
    // Déclaration des constantes
    private static readonly Vector3 FlipRotation = new Vector3(0, 180, 0);

    [SerializeField] private LayerMask m_WhatIsWall;

    // Déclaration des variables
    bool _Grounded { get; set; }
    public bool _Flipped { get; set; }
    Rigidbody _Rb { get; set; }

    // Valeurs exposées
    [SerializeField] float MoveSpeed = 4.0f;
    [SerializeField] LayerMask WhatIsGround;
    [SerializeField] LayerMask WhatIsLava;
    [SerializeField] LayerMask WhatIsWater;

    [Header("Jumping")]
    [SerializeField] float JumpForce = 7f;
    public float fallMultiplier = 4.5f;
    public float lowJumpMultiplier = 2f;
    private bool _isJumping;

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
    public AudioClip CantDashSound;
    public AudioClip WaterSound;
    public AudioClip DeathSound;
    public AudioClip HealingSound;

    [Header("Power-ups")]
    [SerializeField] LayerMask WhatIsDefault;
    [SerializeField] LayerMask WhatIsWall;
    bool isExpanded = false;
    bool isShrinked = false;
    bool shrinkTimeDone = false;

    private bool isShooting = false;

    [Header("Wall jump")]
    [SerializeField] private float m_WallJumpTimer = 5f;
    [SerializeField] private float wallPushBackForce = 14f;
    private Transform m_FrontWallCheck, m_BackWallCheck;
    
    private bool m_TouchesWall = false;
    private bool m_IsWallJumping = false;
    private bool m_IsBackToTheWall = false;

    private TrailRenderer _trailRenderer;
    private ParticleSystem _healingParticle;
    private ParticleSystem _waterParticle;
    private ParticleSystem _speedParticle;
    private ParticleSystem _invincibilityParticle;


    public Transform positionRecast;

    //private float _lavaSurfaceSpeed;
    private float _waterSurfaceSpeed;
    private float _defaultSurfaceSpeed;

    public bool _isInWater;
    public bool _isInLava;

    // Awake se produit avait le Start. Il peut être bien de régler les références dans cette section.
    void Awake()
    {
        _Rb = GetComponent<Rigidbody>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _healingParticle = GameObject.Find("healingParticle").GetComponent<ParticleSystem>();
        _healingParticle.Stop();
        _waterParticle = GameObject.Find("waterParticle").GetComponent<ParticleSystem>();
        _waterParticle.Stop();
        _invincibilityParticle = GameObject.Find("invincibilityParticle").GetComponent<ParticleSystem>();
        _invincibilityParticle.Stop();
        _speedParticle = GameObject.Find("speedParticle").GetComponent<ParticleSystem>();
        _speedParticle.Stop();
        m_FrontWallCheck = transform.Find("FrontWallCheck");
        m_BackWallCheck = transform.Find("BackWallCheck");
        _defaultSurfaceSpeed = MoveSpeed;
        // todo: speed boost doit garder sa vitesse meme si je jump (ou passe) dans une surface differentes
        //_lavaSurfaceSpeed = 1.25f * _defaultSurfaceSpeed;
        _waterSurfaceSpeed = 0.5f * _defaultSurfaceSpeed;
    }

    // Utile pour régler des valeurs aux objets
    void Start()
    {
        _playerhealth = GetComponent<Playerhealth>();
        _Grounded = false;
        _Flipped = false;
        _isInWater = false;
        _isInLava = false;
    }

    private void FixedUpdate()
    {
        m_TouchesWall = false;
        m_IsBackToTheWall = false;
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
    }
    // Vérifie les entrées de commandes du joueur
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var dashInput = Input.GetMouseButtonDown(1);
        undoShrinkIfPlayerCan();

        Dash(dashInput);
        if (_isDashing)
        {
            return;
        }
        if (m_IsWallJumping)
        {
            return;
        }
        HorizontalMove(horizontal);
        FlipCharacter(horizontal);
        Jump();
    }

    private IEnumerator CurrentlyDashing()
    {
        audioSource.PlayOneShot(DashSound);
        _Rb.useGravity = false;
        this.gameObject.GetComponent<Animation_Test>().AttackAni();
        yield return new WaitForSeconds(_dashingTime);
        this.gameObject.GetComponent<Animation_Test>().IdleAni();
        _Rb.velocity = Vector3.zero;
        _Rb.useGravity = true;
        _isDashing = false;
        _trailRenderer.emitting = false;
    }

    private IEnumerator WallJumpTimer()
    {
        m_IsWallJumping = true;
        yield return new WaitForSeconds(m_WallJumpTimer);
        m_IsWallJumping = false;
    }

    // Gère le mouvement horizontal
    void HorizontalMove(float horizontal)
    {
        if (horizontal != 0 && !getIsShooting())
        {
            gameObject.GetComponent<Animation_Test>().RunAni();
        }
        else if (horizontal == 0 && !getIsShooting())
        {
            gameObject.GetComponent<Animation_Test>().IdleAni();
        }
        _Rb.velocity = new Vector3(0, _Rb.velocity.y, horizontal * MoveSpeed);
    }

    private void Dash(bool dashInput)
    {
        if (_isInWater)
        {
            _canDash = false;
        }
        if (dashInput && _isInWater)
        {
            audioSource.PlayOneShot(CantDashSound);
            return;
        }
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
            StartCoroutine(CurrentlyDashing());
        }
    }

    // Gère le saut du personnage, ainsi que son animation de saut
    void Jump()
    {
        var jumpInput = Input.GetButtonDown("Jump");
        if (_Grounded)
        {
            if (jumpInput)
            {
                audioSource.clip = JumpSounds[Random.Range(0, JumpSounds.Length)];
                audioSource.PlayOneShot(audioSource.clip);
                _isJumping = true;
                _Rb.velocity = new Vector3(0, 0, _Rb.velocity.z);
                _Rb.velocity += Vector3.up * JumpForce;
                _Grounded = false;
            }
        } 
        else if (!_Grounded && m_TouchesWall && _isJumping && jumpInput) {
            
            _Rb.velocity = Vector3.zero;
            Vector3 pushbackDirection = !_Flipped ? new Vector3(0, 0, -1) : new Vector3(0, 0, 1);
            if (m_IsBackToTheWall)
            {
                pushbackDirection = !_Flipped ? new Vector3(0, 0, 1) : new Vector3(0, 0, -1);
            }
            Vector3 HorizontalPush = pushbackDirection * wallPushBackForce;
            Vector3 VerticalPush = new Vector3(0, wallPushBackForce, 0);
            Vector3 finalVelocity = HorizontalPush + VerticalPush;
            _Rb.velocity = finalVelocity;
            if (_Flipped)
            {
                _Flipped = true;
                FlipCharacter(pushbackDirection.z);
            }
            else
            {
                _Flipped = false;
                FlipCharacter(pushbackDirection.z);
            }
            
            StartCoroutine(WallJumpTimer());
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
            //_MainCamera.transform.Rotate(-FlipRotation);
            //_MainCamera.transform.localPosition = InverseCameraPosition;
        }
        else if (horizontal > 0 && _Flipped)
        {
            _Flipped = false;
            transform.Rotate(-FlipRotation);
            //_MainCamera.transform.Rotate(FlipRotation);
            //_MainCamera.transform.localPosition = CameraPosition;
        }
    }

    // Collision avec le sol
    void OnCollisionEnter(Collision coll)
    {
        if (isExpanded && (WhatIsWall.value & 1 << coll.gameObject.layer) == 1 << coll.gameObject.layer)
        {
            Rigidbody rb;
            if (rb = coll.rigidbody)
            {
                coll.rigidbody.constraints = RigidbodyConstraints.None
                    | RigidbodyConstraints.FreezePositionX
                    | RigidbodyConstraints.FreezeRotationY
                    | RigidbodyConstraints.FreezeRotationZ;

                rb.AddExplosionForce(10f, transform.position, 5f);
            }

        }
        if (!_isInLava && coll.gameObject.tag == "Lava_Surface")
        {
            //MoveSpeed = _lavaSurfaceSpeed;
            _isInWater = false;
            _isInLava = true;
            audioSource.PlayOneShot(HealingSound);
            _healingParticle.Play();
            _waterParticle.Stop();
        }
        else if (!_isInWater && coll.gameObject.tag == "Water_Surface")
        {
            MoveSpeed = _waterSurfaceSpeed;
            _isInWater = true;
            audioSource.PlayOneShot(WaterSound);
            _isInLava = false;
            _healingParticle.Stop();
            _waterParticle.Play();
        }
        else if (!_isInWater && coll.gameObject.tag == "Spike")
        {
            MoveSpeed = _waterSurfaceSpeed;
            _isInWater = false;
            audioSource.PlayOneShot(DeathSound);
            _isInLava = false;
            _healingParticle.Stop();
            _waterParticle.Play();
            _playerhealth.InstantKill();
        }
        else if (coll.gameObject.tag == "Lava_Surface" || coll.gameObject.tag == "Water_Surface") { }
        else
        {
            _healingParticle.Stop();
            _waterParticle.Stop();
            MoveSpeed = _defaultSurfaceSpeed;
            _isInWater = false;
            _isInLava = false;
        }

        // On s'assure de bien être en contact avec le sol
        if ((WhatIsGround & (1 << coll.gameObject.layer)) == 0)
            return;

        // Évite une collision avec le plafond
        if (coll.relativeVelocity.y > 0)
        {

            _Grounded = true;
            _dashedFromGround = false;
            _canDash = true;
            _isJumping = false;
        }
    }

    public void setIsExpanded(bool value)
    {
        isExpanded = value;
    }

    public void setIsShrinked(bool value)
    {
        isShrinked = value;
    }

    public bool getIsExpanded()
    {
        return isExpanded;
    }

    public bool getIsShrinked()
    {
        return isShrinked;
    }

    public void setShrinkTimeDone(bool value)
    {
        shrinkTimeDone = value;
    }

    public void undoShrinkIfPlayerCan()
    {
        bool estEntreDeuxPlatform = false;
        int floorCount = 0;
        int defaultCount = 0;
        //TODO: specefique a la scene foudra un truc generique pour TP3
        foreach (Collider col in Physics.OverlapSphere(transform.position, 0.5f))
        {
            if ((WhatIsDefault.value & 1 << col.gameObject.layer) == 1 << col.gameObject.layer)
            {
                defaultCount++;
            }
            else if ((WhatIsGround.value & 1 << col.gameObject.layer) == 1 << col.gameObject.layer)
            {
                floorCount++;
            }
        }
        //TODO: specefique a la scene foudra un truc generique pour TP3
        if (defaultCount == 2) estEntreDeuxPlatform = true;
        else if (defaultCount == 1 && floorCount >= 1) estEntreDeuxPlatform = true;


        if (isShrinked && shrinkTimeDone && !estEntreDeuxPlatform)
        {
            transform.localScale = Vector3.one;
            isShrinked = false;
            shrinkTimeDone = false;
        }
    }

    public void PlaySpeedParticleEffect(bool willPlay)
    {
        if (willPlay)
            _speedParticle.Play();
        else _speedParticle.Stop();
    }

    public void PlayInvincibilityParticleEffect(bool willPlay)
    {
        if (willPlay)
            _invincibilityParticle.Play();
        else _invincibilityParticle.Stop();
    }

    public float getSpeed()
    {
        return MoveSpeed;
    }

    public void changeSpeed(float speed)
    {
        MoveSpeed = speed;
    }

    public bool getIsShooting()
    {
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
}