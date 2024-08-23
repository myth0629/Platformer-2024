using UnityEngine;
using UnityEngine.UI;

public class HeroKnight : MonoBehaviour
{
    [SerializeField] private float speed = 4.0f;
    [SerializeField] private float jumpForce = 7.5f;
    [SerializeField] private float rollForce = 6.0f;
    //[SerializeField] private bool noBlood = false;
    [SerializeField] private GameObject slideDustPrefab;

    private Animator animator;
    private Rigidbody2D rb;
    private Sensor_HeroKnight groundSensor;
    private Sensor_HeroKnight wallSensorR1;
    private Sensor_HeroKnight wallSensorR2;
    private Sensor_HeroKnight wallSensorL1;
    private Sensor_HeroKnight wallSensorL2;

    private bool isWallSliding = false;
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
    public float maxHealth;

    public Slider healthBarSlider;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        InitializeSensors();
        SetHp(100f); // 예시: 최대 체력을 100으로 설정
    }

    private void InitializeSensors()
    {
        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    private void Update()
    {
        UpdateTimers();
        CheckGroundStatus();
        HandleInput();
        //UpdateAnimatorState();
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
        if (!isGrounded && groundSensor.State())
        {
            isGrounded = true;
            animator.SetBool("Grounded", isGrounded);
        }
        else if (isGrounded && !groundSensor.State())
        {
            isGrounded = false;
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
            GetComponent<SpriteRenderer>().flipX = false;
            facingDirection = 1;
        }
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            facingDirection = -1;
        }

        if (!isRolling && !isChargingJump)
            rb.velocity = new Vector2(inputX * speed, rb.velocity.y);

        animator.SetFloat("AirSpeedY", rb.velocity.y);
    }

    private void HandleActions(float inputX)
    {
        if (Input.GetMouseButtonDown(0) && timeSinceLastAttack > 0.25f && !isRolling)
        {
            PerformAttack();
        }
        else if (Input.GetMouseButtonDown(1) && !isRolling)
        {
            animator.SetTrigger("Block");
            animator.SetBool("IdleBlock", true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("IdleBlock", false);
        }
        else if (Input.GetKeyDown("left shift") && !isRolling && !isWallSliding)
        {
            Roll();
        }
        else if (Input.GetKeyUp("space") && isGrounded && !isRolling)
        {
            Jump();
        }
        else if (Input.GetKey("space") && isGrounded && !isRolling)
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
        currentAttack = (currentAttack % 3) + 1;

        if (timeSinceLastAttack > 1.0f)
            currentAttack = 1;

        animator.SetTrigger("Attack" + currentAttack);
        timeSinceLastAttack = 0.0f;
    }

    private void Roll()
    {
        isRolling = true;
        animator.SetTrigger("Roll");
        rb.velocity = new Vector2(facingDirection * rollForce, rb.velocity.y);
    }

    private void Jump()
    {
        isChargingJump = false;
        animator.SetTrigger("Jump");
        isGrounded = false;
        animator.SetBool("Grounded", isGrounded);
        rb.AddForce(Vector3.up * jumpForce * jumpChargeTime, ForceMode2D.Impulse);
        jumpChargeTime = 1.0f;
        groundSensor.Disable(0.2f);
    }

    private void ChargeJump()
    {
        isChargingJump = true;
        if (jumpTimeLimit > jumpChargeTime)
            jumpChargeTime += 0.1f;
    }

    private void SetRunningState()
    {
        idleDelay = 0.05f;
        animator.SetInteger("AnimState", 1);
    }

    private void SetIdleState()
    {
        idleDelay -= Time.deltaTime;
        if (idleDelay < 0)
            animator.SetInteger("AnimState", 0);
    }

    public void SetHp(float amount)
    {
        maxHealth = amount;
        currentHealth = maxHealth;
    }

    public void UpdateHealthBar()
    {
        if (healthBarSlider != null)
            healthBarSlider.value = currentHealth / maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (maxHealth == 0 || currentHealth <= 0)
            return;

        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            // Implement death logic here
        }
    }
    public void RestoreHealth(float amount)
{
    currentHealth += amount;
    if (currentHealth > maxHealth)
    {
        currentHealth = maxHealth; // 최대 체력을 초과하지 않도록 함
    }

    UpdateHealthBar(); // 체력 바 업데이트
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
}