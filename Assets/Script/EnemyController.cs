using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Animator animator;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    Transform player;

    public float detectionRange = 5f;  // 플레이어 추적 범위
    public float attackRange = 1.5f;   // 공격 범위
    public int damage = 10;            // 공격력
    public float speed = 2f;           // 이동 속도

    int nextMove;
    public bool isTracing;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        Invoke("Think", 5);
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();  // 공격 범위 내에서 공격
        }
        else if (distanceToPlayer <= detectionRange)
        {
            isTracing = true;
            ChasePlayer();   // 추적 범위 내에서 추적
        }
        else
        {
            isTracing = false;
            Patrol();        // 범위를 벗어나면 정찰 모드
        }

        if (!isTracing)
        {
            CheckGroundAhead(); // 추적 중이 아닐 때만 이동 경로 체크
        }
    }

    void Patrol()
    {
        rigid.velocity = new Vector2(nextMove * speed, rigid.velocity.y);
        animator.SetInteger("WalkSpeed", Mathf.Abs(nextMove));
    }

    void ChasePlayer()
    {
        Vector2 targetPosition = new Vector2(player.position.x, rigid.position.y);
        Vector2 newPosition = Vector2.MoveTowards(rigid.position, targetPosition, speed * Time.deltaTime);
        rigid.MovePosition(newPosition);

        animator.SetInteger("WalkSpeed", 1);
        spriteRenderer.flipX = player.position.x < transform.position.x;
    }

    void AttackPlayer()
    {
        animator.SetTrigger("isAttack");
        Debug.Log("Player takes " + damage + " damage.");
    }

    void Think()
    {
        if (!isTracing)
        {
            nextMove = Random.Range(-1, 2);
            animator.SetInteger("WalkSpeed", Mathf.Abs(nextMove));
            spriteRenderer.flipX = nextMove < 0;

            Invoke("Think", Random.Range(2f, 5f));
        }
    }

    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove < 0;

        CancelInvoke();
        Invoke("Think", 2);
    }

    void CheckGroundAhead()
    {
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y);
        Debug.DrawRay(rigid.position, Vector3.down, Color.green);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)
        {
            Turn();
        }
    }
}