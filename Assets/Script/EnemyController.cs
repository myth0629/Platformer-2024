using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2f; // 적의 이동 속도
    public float detectionRange = 5f; // 플레이어 감지 범위
    public float attackRange = 1f; // 공격 범위
    public int maxHealth = 100; // 최대 체력
    public int damage = 10; // 공격력
    public Transform[] patrolPoints; // 패트롤 경로

    private int currentHealth; // 현재 체력
    private Transform player; // 플레이어의 위치
    private int currentPatrolIndex; // 현재 패트롤 포인트 인덱스
    private bool isChasing; // 플레이어를 추적 중인지 여부
    private Animator animator; // 애니메이터

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentPatrolIndex = 0;
        isChasing = false;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            AttackPlayer();
        }
    }

    void Patrol()
    {
        Transform targetPoint = patrolPoints[currentPatrolIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    void AttackPlayer()
    {
        // 공격 애니메이션 트리거
        animator.SetTrigger("Attack");
        // 플레이어에게 피해 입히기 (여기서는 간단히 로그로 대체)
        Debug.Log("Player takes " + damage + " damage.");
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 사망 애니메이션 트리거
        animator.SetTrigger("Die");
        // 적 비활성화 또는 파괴
        Destroy(gameObject);
    }
}