using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public float damage;
    public float exp;
    public RuntimeAnimatorController[] animCon;
    public bool isDead;
    public Rigidbody2D target;
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    WaitForFixedUpdate wait;
    Collider2D coll;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
        coll = GetComponent<Collider2D>();
    }

    void Update()
    {
    }
    void FixedUpdate()
    {
        anim.SetBool("Dead", isDead);
        if (isDead || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return;
        Vector2 dirVec = target.position - rigid.position;
        Vector2 moveVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        spriter.flipX = dirVec.x > 0 ? false : true;
        rigid.MovePosition(rigid.position + moveVec);
        rigid.velocity = Vector2.zero;
    }

    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        coll.enabled = true;
        spriter.sortingOrder = 2;
        rigid.simulated = true;
        isDead = false;
        health = maxHealth;
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed * (GameManager.instance.stage * 0.5f + 1);
        maxHealth = data.health * (GameManager.instance.stage * 0.5f + 1);
        health = data.health * (GameManager.instance.stage * 0.5f+ 1);
        damage = data.damage * (GameManager.instance.stage * 0.5f + 1);
        exp = data.exp * (GameManager.instance.stage * 0.5f + 1);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || isDead) return;
        StartCoroutine(KnockBack());
        health -= collision.GetComponent<Bullet>().damage;

        if(health > 0)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
            anim.SetTrigger("Hit");
        }
        else
        {
            isDead = true;
            rigid.simulated = false;
            coll.enabled = false;
            spriter.sortingOrder = 1;
            GameManager.instance.kill++;
            GameManager.instance.GetExp(exp);
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        }
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }

    IEnumerator KnockBack()
    {
        yield return wait;
        Vector3 dirVec = (GameManager.instance.player.transform.position - transform.position).normalized;
        rigid.AddForce((-1) * dirVec * 3, ForceMode2D.Impulse);
    }
}
