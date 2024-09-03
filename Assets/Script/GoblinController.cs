using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinController : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;

    public float speed = 2f; // 적의 이동 속도
    public float detectionRange = 4f; // 플레이어 감지 범위
    public float attackRange = 1f; // 공격 범위
    public int maxHealth = 100; // 최대 체력
    public int damage = 10; // 공격력

    private int currentHealth; // 현재 체력
    private Transform player; // 플레이어의 위치
    private bool isChasing; // 플레이어를 추적 중인지 여부
    private int nextMove;
    private bool isDead = false; // 적이 사망했는지 여부

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 플레이어 태그로 플레이어 오브젝트를 찾음
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 초기화된 체력 설정
        currentHealth = maxHealth;

        // 초기 Think 호출
        Think();
    }

    void Update()
    {
        if (isDead) return; // 사망 상태에서는 Update를 실행하지 않음
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            isChasing = true;
            anim.SetBool("isRun", true);
        }
        else
        {
            isChasing = false;
            anim.SetBool("isRun", true);
        }

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
            anim.SetBool("isRun", false);
        }
    }

    void Patrol()
    {
        rigid.velocity = new Vector2(nextMove * speed, rigid.velocity.y);

        // 이동 방향에 따라 스프라이트를 뒤집음
        if (nextMove != 0)
        {
            spriteRenderer.flipX = nextMove < 0;
            anim.SetBool("isRun", true);
        }
        else
        {
            anim.SetBool("isRun", false);
        }
    }

    void Think()
    {
        // -1 ~ 2 사이의 값을 랜덤으로 결정
        nextMove = Random.Range(-1, 2);

        // 다음 행동을 결정하는 시간 설정
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    void ChasePlayer()
    {
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z); // y축 고정
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // 플레이어의 위치에 따라 스프라이트 뒤집기
        spriteRenderer.flipX = player.position.x < transform.position.x;

        // 추적 상태 애니메이션 설정
        anim.SetBool("isRun", true);
    }

    void AttackPlayer()
    {
        // 공격 애니메이션 트리거
        anim.SetTrigger("isAttack");

        // 공격 로직 (여기서는 간단히 로그로 대체)
        Debug.Log("Player takes " + damage + " damage.");
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // 이미 사망한 경우 추가 데미지 처리하지 않음

        currentHealth -= damage;
        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        anim.SetTrigger("isDeath");
        rigid.velocity = Vector2.zero; // 죽을 때 이동 멈춤
        GetComponent<Collider2D>().enabled = false; // 콜라이더 비활성화
        this.enabled = false; // 스크립트 비활성화
    }
}