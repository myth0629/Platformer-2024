using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Components")]
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid;
    public Transform player;
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float invincibilityDuration = 0.3f;
    
    private bool isInvincible = false;
    private Material material;
    private Color originalColor;

    [Header("Stats")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float attackCooldown = 2f; // 공격 쿨타임 (초)
    private float lastAttackTime = 0f; // 마지막 공격 시간

    private bool isTracing = false;
    private int nextMove = 0;
    private float moveThreshold = 0.1f; // 움직임 감지 임계값

    private void Start()
    {
        currentHealth = maxHealth;
        InitializeComponents();
        Invoke("Think", 5);
    }

    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        Invoke("Think", 5);
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        UpdateBehavior(distanceToPlayer);
        UpdateAnimationState();
    }

    private void UpdateBehavior(float distanceToPlayer)
    {
        if (distanceToPlayer <= attackRange)
        {
            isTracing = false;
            StopMovement();
            AttackPlayer();
        }
        else
        {
            isTracing = false;
            Patrol();
            if (!isTracing) CheckGroundAhead();
        }
    }

    private void UpdateAnimationState()
    {
        float currentSpeed = Mathf.Abs(rigid.velocity.x);
        bool isMoving = currentSpeed > moveThreshold;
        
        // 이동 애니메이션 설정
        animator.SetInteger("WalkSpeed", isMoving ? 1 : 0);
        
        // 이동 중일 때만 방향 전환
        if (isMoving)
        {
            spriteRenderer.flipX = rigid.velocity.x < 0;
        }
    }

    private void StopMovement()
    {
        rigid.velocity = new Vector2(0, rigid.velocity.y);
    }

    private void Patrol()
    {
        rigid.velocity = new Vector2(nextMove * speed, rigid.velocity.y);
    }

    private void AttackPlayer()
{
    // 현재 시간과 마지막 공격 시간을 비교하여 쿨타임 체크
    if (Time.time - lastAttackTime >= attackCooldown)
    {
        // 공격 애니메이션 실행
        animator.SetTrigger("isAttack");
        
        // 플레이어에게 데미지 적용
        HeroKnight playerHealth = player.GetComponent<HeroKnight>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
        
        // 마지막 공격 시간 업데이트
        lastAttackTime = Time.time;
    }
}

    private void Think()
    {
        if (!isTracing)
        {
            nextMove = Random.Range(-2, 3);
            Invoke("Think", Random.Range(2f, 5f));
        }
    }

    private void Turn()
    {
        nextMove *= -1;
        CancelInvoke();
        Invoke("Think", 2);
    }

    private void CheckGroundAhead()
    {
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, Color.green);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)
        {
            Turn();
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        
        animator.SetTrigger("Hurt");
        Debug.Log("Attack!!");
        
        // 체력 확인
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 사망 애니메이션
        animator.SetTrigger("isDeath");
        
        // 물리 효과 비활성화
        rigid.velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        
        // 모든 행동 중지
        this.enabled = false;
        
        // 사망 처리 (선택적)
        Destroy(gameObject, 1f); // 1초 후 오브젝트 제거
    }
}