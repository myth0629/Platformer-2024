using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid;
    
    [Header("Stats")]
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float currentHealth;
    // speed를 직접 값을 할당하고 더 큰 값으로 설정
    private float speed = 3f;  
    // nextMove를 더 명확한 값으로 설정
    private int nextMove = 1;  // 1: 오른쪽, -1: 왼쪽
    private float moveThreshold = 0.1f;
    private bool isInvincible = false;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        
        // 속도 직접 설정
        rigid.velocity = new Vector2(nextMove * speed, rigid.velocity.y);
    }

    private void FixedUpdate()  // Update 대신 FixedUpdate 사용
    {
        // 이동 실행
        Patrol();
        
        // 앞쪽 지형 체크
        CheckGroundAhead();
    }

    private void Update()
    {
        // 애니메이션 업데이트만 Update에서 처리
        UpdateAnimationState();
    }


    private void UpdateAnimationState()
    {
        float currentSpeed = Mathf.Abs(rigid.velocity.x);
        bool isMoving = currentSpeed > moveThreshold;
        
        animator.SetInteger("WalkSpeed", isMoving ? 1 : 0);
        
        if (isMoving)
        {
            spriteRenderer.flipX = rigid.velocity.x < 0;
        }
    }

    private void Patrol()
    {
        rigid.velocity = new Vector2(nextMove * speed, rigid.velocity.y);
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

    private void Turn()
    {
        nextMove *= -1;
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        
        animator.SetTrigger("Hurt");
        Debug.Log("Attack!!");
        
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
        
        // 1초 후 오브젝트 제거
        Destroy(gameObject, 1f);
    }
}
