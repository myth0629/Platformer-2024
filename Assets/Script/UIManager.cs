using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIManager : MonoBehaviour
{
    public Image[] hearts;  // 하트 이미지 배열
    public Transform playerInitialPosition; // 캐릭터 초기 위치
    public Transform[] monsterInitialPositions; // 몬스터 초기 위치 배열
    public GameObject player; // 캐릭터 오브젝트
    public GameObject[] monsters; // 몬스터 오브젝트 배열

    private int coinCount = 500; // 코인 카운트 초기화


    private bool isGoblinActive = false; // 고블린이 활성화되었는지 여부

    public int maxHealth = 100;
    private int currentHealth;

    public Text healthText;
    public GameObject playerCanvas;

    // 메인 메뉴, 튜토리얼, 설정, 엔딩 관련 캔버스
    public GameObject mainCanvas;
    public GameObject tutorialCanvas;
    public GameObject settingCanvas;
    public GameObject endCanvas;
    public GameObject clearCanvas;

    public GameObject mainSettingCanvas; 
    public GameObject shopCanvas;  // 상점 캔버스

    public Camera mainCamera;
    public Camera camera1;

    public GameObject HeroKnight;
    public GameObject enemyPrefab;
    public Transform[] enemySpawnPoints;

    public Text elapsedTimeText;
    private float elapsedTime;
    private float startTime;

    private bool isTutorialActive = false;
    private GameObject currentEnemy;

    public Text coinText; // 코인 텍스트 UI 요소
    public int coinAmount = 0; // 코인 변수

    void Start()
    { 
        currentHealth = maxHealth;  // 초기 체력 설정
        UpdateHearts();  // 하트 UI 업데이트
        ShowMainCanvas();
        UpdateCoinText(); // 초기 코인 텍스트 업데이트
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        UpdateHealthText();

    }
    // 코인 추가 메서드
    public void AddCoin(int amount)
    {
        coinAmount += amount;
        UpdateCoinText();
    }
    // 코인 텍스트 업데이트 메서드
    public void UpdateCoinText()
    {
        coinText.text = "코인: " + coinAmount.ToString();
    }

    void Update()
    {
        if (isTutorialActive)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleSettingCanvas();
            }

            // playerCanvas가 활성화 되었을 때만 시간 업데이트
            if (playerCanvas.activeSelf)
            {
                elapsedTime = Time.time - startTime;
                UpdateElapsedTimeText();
            }
            else
            {
                // playerCanvas가 비활성화되면 elapsedTime 초기화
                elapsedTime = 0f; // 시간 초기화
            }
        }
    }
    public void DeductCoins(int amount)
    {
        coinCount -= amount;
        if (coinCount < 0) coinCount = 0; // 코인 수가 0보다 작아지지 않도록
        Debug.Log("남은 코인: " + coinCount);
    }

    public void OnEndButtonClicked()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnStartButtonClicked()
    {
        ShowTutorialCanvas();
    }

    public void OnNextButtonClicked()
    {
        HideAllCanvases();
        SwitchToCamera1();
        isTutorialActive = true;
        HeroKnight.SetActive(true);
        playerCanvas.SetActive(true);
        SpawnEnemy();
        isGoblinActive = true; // 고블린 활성화
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        startTime = Time.time; // 현재 시간을 startTime으로 설정
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        UpdateHearts();
        UpdateHealthText();  // 체력 텍스트 업데이트
    }

    public void UpdateHearts()
    {
        int heartIndex = Mathf.CeilToInt(currentHealth / 20f); // 체력 20마다 하트 1개씩
        
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = (i < heartIndex);  // 체력이 남아 있으면 하트 보이고, 없으면 숨기기
        }
    }
    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text =  currentHealth.ToString() + "/100";
        }
        else
        {
            Debug.LogWarning("healthText is not assigned.");
        }
    }

    public void OnMonsterAttack()
    {
        TakeDamage(20);  // 몬스터 공격 시 체력 20 감소
    }

    public void OnReturnButtonClicked()
    {
        ShowMainCanvas();
        // 코인 초기화
        coinAmount = 0;
        UpdateCoinText(); // 텍스트 업데이트
    }

    public void OnBackButtonClicked()
    {
        HideSettingCanvas();
    }

    public void OnFinishButtonClicked()
    {
        ResetGame();
        ShowMainCanvas();
    }

    public void OnAgainButtonClicked()
    {
        ResetGame();
        ShowTutorialCanvas();
    }

    public void ResetGame()
    {
        HeroKnight playerScript = HeroKnight.GetComponent<HeroKnight>();
        if (playerScript != null)
        {
            playerScript.currentHealth = playerScript.maxHealth;  
        }
        HideAllCanvases();
        HeroKnight.SetActive(false);
        playerCanvas.SetActive(false);

        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
        }
    }
   public void ResetGameToInitialState()
{
    // 캐릭터 위치 초기화
    if (player != null)
    {
        player.transform.position = playerInitialPosition.position;
        player.transform.rotation = playerInitialPosition.rotation; // 필요시 회전 초기화

        // Rigidbody 초기화 (필요시)
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // 속도 초기화
            rb.angularVelocity = 0; // 각속도 초기화
        }
    }

    // 몬스터 위치 초기화
    for (int i = 0; i < monsters.Length; i++)
    {
        if (monsters[i] != null)
        {
            monsters[i].transform.position = monsterInitialPositions[i].position;
            monsters[i].transform.rotation = monsterInitialPositions[i].rotation; // 필요시 회전 초기화
            monsters[i].GetComponent<GoblinController>().TakeDamage(-monsters[i].GetComponent<GoblinController>().maxHealth); // 체력 초기화
        }
    }

    // 하트 UI 초기화
    currentHealth = maxHealth;
    UpdateHearts();
  // MainCanvas 활성화
    mainCanvas.SetActive(true);
    // 다른 모든 캔버스 비활성화
    playerCanvas.SetActive(false);
    clearCanvas.SetActive(false);
}
    
    public void CloseShopCanvas()
    {
        if (shopCanvas != null)
        {
            shopCanvas.SetActive(false); // shopCanvas 비활성화
        }
        else
        {
            Debug.LogWarning("shopCanvas is not assigned.");
        }
    }

    private void ShowMainCanvas()
    {
        if (mainCanvas != null)
            mainCanvas.SetActive(true);
        
        if (mainSettingCanvas != null)
            mainSettingCanvas.SetActive(true);

        if (tutorialCanvas != null)
            tutorialCanvas.SetActive(false);

        if (settingCanvas != null)
            settingCanvas.SetActive(false);

        if (endCanvas != null)
            endCanvas.SetActive(false);

        if (clearCanvas != null)
            clearCanvas.SetActive(false);

        if (playerCanvas != null)
            playerCanvas.SetActive(false);

        SwitchToMainCamera();
        isTutorialActive = false;

        if (mainSettingCanvas != null)
            mainSettingCanvas.SetActive(false);
    
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void ShowTutorialCanvas()
    {
        SetCanvasActive(false, mainCanvas, mainSettingCanvas);
        SetCanvasActive(true, tutorialCanvas);
        isTutorialActive = true;
    }

    private void HideAllCanvases()
    {
        SetCanvasActive(false, mainCanvas, tutorialCanvas, settingCanvas, endCanvas, clearCanvas, playerCanvas, mainSettingCanvas);
    }

    private void ToggleSettingCanvas()
    {
        if (isTutorialActive)
        {
            settingCanvas.SetActive(!settingCanvas.activeSelf);
        }
    }

    private void HideSettingCanvas()
    {
        settingCanvas.SetActive(false);
    }

    private void SwitchToMainCamera()
    {
        mainCamera.enabled = true;
        camera1.enabled = false;
    }

    private void SwitchToCamera1()
    {
        mainCamera.enabled = false;
        camera1.enabled = true;
    }
//
    private void SpawnEnemy()
    {
        if (enemySpawnPoints.Length > 0 && enemyPrefab != null)
        {
            int randomIndex = Random.Range(0, enemySpawnPoints.Length);
            currentEnemy = Instantiate(enemyPrefab, enemySpawnPoints[randomIndex].position, Quaternion.identity);
            currentEnemy.SetActive(true); // 적 활성화

            // 고블린의 활성화 메서드를 호출하여 동작 시작
            currentEnemy.GetComponent<GoblinController>().ActivateGoblin();
        }
    }
    public void RestoreHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;  // 최대 체력을 넘지 않도록 제한
        }
        UpdateHealthText();  // 체력 텍스트 업데이트
    }

    private void UpdateElapsedTimeText()
    {
        if (elapsedTimeText != null)
        {
            // 시간 포맷을 "MM:SS"로 변환
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            elapsedTimeText.text = $"{minutes:D2}:{seconds:D2}";
        }
        else
        {
            Debug.LogWarning("elapsedTimeText is not assigned.");
        }
    }

    private void SetCanvasActive(bool isActive, params GameObject[] canvases)
    {
        foreach (GameObject canvas in canvases)
        {
            canvas.SetActive(isActive);
        }
    }
}