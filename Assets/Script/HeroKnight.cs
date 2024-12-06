using System.Collections;
using UnityEngine;

public class HeroKnight : MonoBehaviour
{
    [SerializeField] private float speed = 4.0f;
    [SerializeField] private float jumpForce = 7.5f;
    [SerializeField] private float rollForce = 6.0f;
    [SerializeField] private GameObject slideDustPrefab;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip landSound;
    
    AudioSource audioSource;
    private UIManager uiManager;
    private AudioClip soundEffect;

    private Animator animator;
    
    private Rigidbody2D rb;
    private Sensor_HeroKnight groundSensor;
    private Sensor_HeroKnight wallSensorR1;
    private Sensor_HeroKnight wallSensorR2;
    private Sensor_HeroKnight wallSensorL1;
    private Sensor_HeroKnight wallSensorL2;

    private bool isWallSliding = false;
    private float footstepTimer = 0f;
    private float footstepInterval = 0.4f; // 1초로 설정[1]
    private bool isWalking = false;

    private bool isGrounded = false;
    private bool isRolling = false;
    private bool isChargingJump = false;

    private int facingDirection = 1;
    private int currentAttack = 0;

    private float timeSinceLastAttack = 0.0f;
    private float idleDelay = 0.0f;
    private float rollDuration = 8.0f / 14.0f;
    private float rollTime;

    private float jumpTimeLimit = 3.0f;
    private float jumpChargeTime = 1.0f;

    public float currentHealth;
    public float maxHealth = 50;
    public float attackRange = 2f;
    public float attackDamage = 10;
    public LayerMask enemyLayer;
    public bool isInvincible;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    [SerializeField] private float attackCooldown = 0.5f; // 공격 쿨타임
    private float lastAttackTime = 0f; // 마지막 공격 시간
    private bool canAttack = true; // 공격 가능 여부
    private bool previouslyGrounded = false;

    void Start() {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.5f;
        InitializeSensors();
        currentHealth = maxHealth;  // 최대 체력 설정
        uiManager = GetComponent<UIManager>();
    }

    void Update()
    {
        UpdateTimers();
        CheckGroundStatus();
        HandleInput();
        timeSinceLastAttack += Time.deltaTime;
            // 이전 상태가 공중에 있다가(false) 현재 땅에 닿은 경우(true)
        if (!previouslyGrounded && isGrounded)
        {
            audioSource.PlayOneShot(landSound);  // 착지 소리 재생
        }
        
        previouslyGrounded = isGrounded;  // 현재 상태를 저장
    }

     private void InitializeSensors()
    {
        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    public void RestoreHealth()
    {
        currentHealth += 10;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;  // 최대 체력을 넘지 않도록 제한
        }
        Debug.Log("현재 체력: " + currentHealth);

    }

    private void UpdateTimers()
    {
        timeSinceLastAttack += Time.deltaTime;

        if (isRolling)
        {
            rollTime += Time.deltaTime;
            if (rollTime > rollDuration)
                isRolling = false;
        }
    }

    private void CheckGroundStatus()
    {
        Vector2 boxSize = new Vector2(0.8f, 0.5f);
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxSize, 0f, Vector2.down, groundCheckDistance, groundLayer);

        bool newGroundedState = hit.collider != null;

        if (isGrounded != newGroundedState)
        {
            isGrounded = newGroundedState;
            animator.SetBool("Grounded", isGrounded);
        }
    }
    private void HandleInput()
    {
        float inputX = Input.GetAxis("Horizontal");
        HandleMovement(inputX);
        HandleActions(inputX);
    }

    private void HandleMovement(float inputX)
{
    if (inputX > 0)
    {
        GetComponent<SpriteRenderer>().flipX = false; // 오른쪽으로 이동할 때 스프라이트를 뒤집지 않음
        facingDirection = 1; // 현재 방향 설정
    }
    else if (inputX < 0)
    {
        GetComponent<SpriteRenderer>().flipX = true; // 왼쪽으로 이동할 때 스프라이트를 뒤집음
        facingDirection = -1; // 현재 방향 설정
    }

    // 롤링 중이 아니고 점프를 차지하고 있지 않을 때만 이동
    if (!isRolling && !isChargingJump)
        rb.velocity = new Vector2(inputX * speed, rb.velocity.y); // Rigidbody2D를 사용하여 수평 속도 설정

    animator.SetFloat("AirSpeedY", rb.velocity.y); // Y축 속도를 애니메이터에 전달
}

    private void HandleActions(float inputX)
    {
        if (Input.GetMouseButtonDown(0) && timeSinceLastAttack > 0.25f && !isRolling)
        {
            PerformAttack();
        }
        // else if (Input.GetMouseButtonDown(1) && !isRolling)
        // {
        //     animator.SetTrigger("Block");
        //     animator.SetBool("IdleBlock", true);
        // }
        // else if (Input.GetMouseButtonUp(1))
        // {
        //     animator.SetBool("IdleBlock", false);
        // }
        // else if (Input.GetKeyDown("left shift") && !isRolling && !isWallSliding)
        // {
        //     Roll();
        // }
        else if (Input.GetKeyUp("space") && isGrounded)
        {
            isChargingJump = false;
            Jump();
        }
        else if (Input.GetKey("space") && isGrounded)
        {
            ChargeJump();
        }
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            SetRunningState();
        }
        else
        {
            SetIdleState();
        }
    }
    private void PerformAttack()
    {
        if (!canAttack) return;
        
        currentAttack = (currentAttack % 3) + 1;

        if (timeSinceLastAttack > 1.0f)
            currentAttack = 1;

        animator.SetTrigger("Attack" + currentAttack);
        audioSource.PlayOneShot(attackSound);
        timeSinceLastAttack = 0.0f;
        
        // 공격 실행
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyController>().TakeDamage(attackDamage);
        }
        
        // 쿨타임 시작
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        lastAttackTime = Time.time;
        
        yield return new WaitForSeconds(attackCooldown);
        
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void Roll()
    {
        isRolling = true;
        animator.SetTrigger("Roll");
        rb.velocity = new Vector2(facingDirection * rollForce, rb.velocity.y);
    }

    private void Jump()
    {
        audioSource.PlayOneShot(jumpSound);
        animator.SetBool("IdleBlock", false);
        isChargingJump = false;
        animator.SetTrigger("Jump");
        isGrounded = false;
        animator.SetBool("Grounded", isGrounded);
        rb.AddForce(Vector3.up * jumpForce * jumpChargeTime, ForceMode2D.Impulse);
        jumpChargeTime = 1.0f;
    }

    private void ChargeJump()
    {
        animator.SetBool("IdleBlock", true);
        isChargingJump = true;
        if (jumpTimeLimit > jumpChargeTime)
            jumpChargeTime += 0.1f;
    }

    private void SetRunningState()
    {
        idleDelay = 0.05f;
        animator.SetInteger("AnimState", 1);
        
        // 타이머 업데이트
        footstepTimer += Time.deltaTime;
        
        // 1초마다 발소리 재생
        if (footstepTimer >= footstepInterval && isGrounded)
        {
            audioSource.PlayOneShot(walkSound);
            footstepTimer = 0f; // 타이머 리셋
        }
    }

    private void SetIdleState()
    {
        idleDelay -= Time.deltaTime;
        if (idleDelay < 0)
            animator.SetInteger("AnimState", 0);
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0)
            return;

        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        if (currentHealth < 0)
            currentHealth = 0;

        Debug.Log("플레이어 hp : " + currentHealth);

        if (currentHealth <= 0)
        {
            Death();
        }
        else
        {
            StartCoroutine(DamageCoolTime());
        }
    }

    IEnumerator DamageCoolTime()
    {
        isInvincible = true;
        yield return new WaitForSeconds(1.5f);
        isInvincible = false;
    }

    public void Death()
    {
        animator.SetTrigger("Death");

        GetComponent<HeroKnight>().enabled = false; // 조작 정지
        rb.velocity = Vector2.zero; // 이동중이라면 정지
    }

    private void AE_SlideDust()
    {
        Vector3 spawnPosition = (facingDirection == 1) ? wallSensorR2.transform.position : wallSensorL2.transform.position;

        if (slideDustPrefab != null)
        {
            GameObject dust = Instantiate(slideDustPrefab, spawnPosition, gameObject.transform.localRotation);
            dust.transform.localScale = new Vector3(facingDirection, 1, 1);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    { 
        if (other.CompareTag("Trap"))
        {
            TakeDamage(10);
            Debug.Log("Player Hit!!");
        }
    }
}