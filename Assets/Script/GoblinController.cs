using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinController : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;

    private bool isActive = false; // 고블린이 활성화되었는지 여부

    public float speed = 2f; // 적의 이동 속도
    public float detectionRange = 4f; // 플레이어 감지 범위
    public float attackRange = 2f; // 공격 범위
    public int maxHealth = 50; // 최대 체력
    public int damage = 10; // 공격력

    private int currentHealth; // 현재 체력
    private Transform player; // 플레이어의 위치
    private bool isChasing; // 플레이어를 추적 중인지 여부
    private int nextMove;
    private bool isDead = false; // 적이 사망했는지 여부
    private bool canAttack = true;
    public float attackCooldown = 1.5f;

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
        
        // 초기 상태에서 고블린을 비활성화
        //gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isActive || isDead) return; // 고블린이 활성화되지 않았거나 사망한 경우
        if (isDead) return; // 사망 상태에서는 Update를 실행하지 않음
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

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
        }
        else
        {
            isChasing = false;
        }

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
    }

    void Patrol()
    {
        rigid.velocity = new Vector2(nextMove * speed, rigid.velocity.y);
        anim.SetInteger("WalkSpeed", nextMove);

        // 이동 방향에 따라 스프라이트를 뒤집음
        if (nextMove != 0)
        {
            spriteRenderer.flipX = nextMove < 0;
        }
    }
    public void ActivateGoblin()
    {
        isActive = true; // 고블린 활성화
        gameObject.SetActive(true); // 게임 오브젝트 활성화
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
        Vector3 targetPosition = player.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // 플레이어의 위치에 따라 스프라이트 뒤집기
        spriteRenderer.flipX = player.position.x < transform.position.x;

        // 추적 상태 애니메이션 설정
        anim.SetInteger("WalkSpeed", 1);
    }

    public void AttackPlayer()
    {
        if(canAttack && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            canAttack = false;

            anim.SetTrigger("isAttack");

            player.GetComponent<HeroKnight>().TakeDamage(damage);

            Invoke("ResetAttack", attackCooldown);
        }
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // 이미 사망한 경우 추가 데미지 처리하지 않음
        
        currentHealth -= damage;
        Debug.Log("고블린 hp : " + currentHealth);
        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        anim.SetBool("isDeath", true);
        rigid.velocity = Vector2.zero; // 죽을 때 이동 멈춤
        this.enabled = false; // 스크립트 비활성화
    }
}