using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneMonsterController : MonoBehaviour


{
    private static readonly Vector3 FlipRotation = new Vector3(0, 180, 0);
    private static readonly Vector3 CameraPosition = new Vector3(10, 1, 0);
    private static readonly Vector3 InverseCameraPosition = new Vector3(-10, 1, 0);

    // Déclaration des variables
    bool _Grounded { get; set; }
    bool _Flipped { get; set; }
    Rigidbody _Rb { get; set;}
    Camera _Camera { get; set; }
    bool isRunning = false;
    bool isExpanded = false;
    bool isShrinked = false;
    bool shrinkTimeDone = false;

    // Valeurs exposées
    [SerializeField]
    float MoveSpeed = 5.0f;

    [SerializeField]
    float JumpForce = 10f;

    [SerializeField]
    LayerMask WhatIsGround;

    [SerializeField]
    LayerMask WhatIsDefault;



    [SerializeField]
    LayerMask WhatIsWall;


    // Start is called before the first frame update
    void Start()
    {
        _Grounded = false;
        _Flipped = false;
        _Camera = Camera.main;
        _Rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        bool movingKeysPressed = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);
        Animation_Test animation = gameObject.GetComponent<Animation_Test>();
        if (!isRunning && movingKeysPressed)
        {
            animation.RunAni();
            isRunning = true;   

        }
        if (isRunning && !movingKeysPressed)
        {
            animation.IdleAni();
            isRunning = false;
        }

        undoShrinkIfPlayerCan();

        var horizontal = Input.GetAxis("Horizontal") * MoveSpeed;
        HorizontalMove(horizontal);
        FlipCharacter(horizontal);
        CheckJump();
    }

    // Gère le mouvement horizontal
    void HorizontalMove(float horizontal)
    {
        _Rb.velocity = new Vector3(_Rb.velocity.x, _Rb.velocity.y, horizontal);
    }

    // Gère l'orientation du joueur et les ajustements de la camera
    void FlipCharacter(float horizontal)
    {
        if (horizontal < 0 && !_Flipped)
        {
            _Flipped = true;
            transform.Rotate(FlipRotation);
            _Camera.transform.Rotate(FlipRotation);
            _Camera.transform.localPosition = InverseCameraPosition;
        }
        else if (horizontal > 0 && _Flipped)
        {
            _Flipped = false;
            transform.Rotate(-FlipRotation);
            _Camera.transform.Rotate(-FlipRotation);
            _Camera.transform.localPosition = CameraPosition;
        }
    }


    // Gère le saut du personnage, ainsi que son animation de saut
    void CheckJump()
    {
        if (_Grounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                _Rb.AddForce(new Vector3(0, JumpForce, 0), ForceMode.Impulse);
                _Grounded = false;
            }
        }
    }

    // Collision avec le sol
    void OnCollisionEnter(Collision coll)
    {
        if (isExpanded && (WhatIsWall.value & 1 << coll.gameObject.layer) == 1 << coll.gameObject.layer)
        {
            Debug.Log("collision with wall while expanded");
            Rigidbody rb;
            if (rb = coll.rigidbody) {
                coll.rigidbody.constraints = RigidbodyConstraints.None
                    | RigidbodyConstraints.FreezePositionX
                    | RigidbodyConstraints.FreezeRotationY
                    | RigidbodyConstraints.FreezeRotationZ;

                rb.AddExplosionForce(10f, transform.position, 5f);
            }

        }

        // On s'assure de bien être en contact avec le sol
        if ((WhatIsGround & (1 << coll.gameObject.layer)) == 0)
            return;

        // Évite une collision avec le plafond
        if (coll.relativeVelocity.y > 0)
        {
            _Grounded = true;
        }
    }

    public void setIsExpanded(bool value){
        isExpanded = value;
    }

    public void setIsShrinked(bool value) {
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

    public void setShrinkTimeDone(bool value) {
        shrinkTimeDone = value;
    }

    public void undoShrinkIfPlayerCan() {
        bool estEntreDeuxPlatform = false;
        int floorCount = 0;
        int defaultCount = 0;
        //TODO: specefique a la scene foudra un truc generique pour TP3
        foreach (Collider col in Physics.OverlapSphere(transform.position, 0.5f)) {
            if ((WhatIsDefault.value & 1 << col.gameObject.layer) == 1 << col.gameObject.layer) {
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

    public float getSpeed() {
        return MoveSpeed;
    }

    public void changeSpeed(float speed) {
        MoveSpeed = speed;    
    }
}
