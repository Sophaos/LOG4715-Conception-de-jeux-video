using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireScript : MonoBehaviour
{
    // D�claration des constantes
    private static readonly Vector3 FlipRotation = new Vector3(0, 180, 0);
    private static readonly Vector3 CameraPosition = new Vector3(10, 1, 0);
    private static readonly Vector3 InverseCameraPosition = new Vector3(10, 1, 0);

    // D�claration des variables
    bool _Grounded { get; set; }
    bool _Flipped { get; set; }
    //Animator _Anim { get; set; }
    Rigidbody _Rb { get; set; }
    Camera _MainCamera { get; set; }

    // Valeurs expos�es
    [SerializeField]
    float MoveSpeed = 5.0f;

    [SerializeField]
    float JumpForce = 10f;

    [SerializeField]
    LayerMask WhatIsGround;

    // Awake se produit avait le Start. Il peut �tre bien de r�gler les r�f�rences dans cette section.
    void Awake()
    {
        //_Anim = GetComponent<Animator>();
        _Rb = GetComponentInParent<Rigidbody>();
        _MainCamera = Camera.main;
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
        HorizontalMove(horizontal);
        FlipCharacter(horizontal);
        CheckJump();
    }

    // G�re le mouvement horizontal
    void HorizontalMove(float horizontal)
    {
        _Rb.velocity = new Vector3(0, _Rb.velocity.y, 0);
        //_Anim.SetFloat("MoveSpeed", Mathf.Abs(horizontal));
    }

    // G�re le saut du personnage, ainsi que son animation de saut
    void CheckJump()
    {
        if (_Grounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                _Rb.AddForce(new Vector3(0, JumpForce, 0), ForceMode.Impulse);
                _Grounded = false;
                //_Anim.SetBool("Grounded", false);
                //_Anim.SetBool("Jump", true);
            }
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
            //_Anim.SetBool("Grounded", _Grounded);
        }
    }
}