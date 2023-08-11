using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public float hitTime;
    public Scaner scanner;
    public Hand[] hands;
    public bool isHit;


    Rigidbody2D rigid;
    public Animator animator;
    SpriteRenderer spriter;
    public RuntimeAnimatorController[] animatorControllers;

    public Vector2 inputVec;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        scanner = GetComponent<Scaner>();
        hands = GetComponentsInChildren<Hand>(true);
    }
    void Start()
    {

    }


    void Update()
    {

    }

    void FixedUpdate()
    {
        Move();
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (isHit) return;
        if (!GameManager.instance.isLive) return;
        if (collision.gameObject.tag == "Enemy")
        {
            GameManager.instance.health -= (int)collision.gameObject.GetComponent<Enemy>().damage;
            if(GameManager.instance.health<= 0)
            {
                animator.SetTrigger("dead");
                GameManager.instance.GameOver();
                GameManager.instance.Stop();
                return;
            }
            isHit = true;
            spriter.color = new Color(1, 0.5f, 0.5f, 1);
            Invoke("ExitHit", hitTime);
        }
    }





    void Move()
    {
        animator.SetFloat("Speed", inputVec.magnitude);
        Vector2 moveVec = inputVec * moveSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + moveVec);
        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x > 0 ? false : true;
        }
    }


    void ExitHit()
    {
        isHit = false;
        spriter.color = Color.white;
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

}
