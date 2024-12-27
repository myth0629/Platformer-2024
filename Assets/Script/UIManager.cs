using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private bool gameCleared = false;
    public Image[] hearts;  // 하트 이미지 배열
    public Transform playerInitialPosition; // 캐릭터 초기 위치
    public Transform[] monsterInitialPositions; // 몬스터 초기 위치 배열
    public GameObject player; // 캐릭터 오브젝트
    public GameObject[] monsters; // 몬스터 오브젝트 배열

    private int coinCount = 500; // 코인 카운트 초기화


    private bool isGoblinActive = false; // 고블린이 활성화되었는지 여부


    public Text healthText;
    public GameObject playerCanvas;

    // 메인 메뉴, 튜토리얼, 설정, 엔딩 관련 캔버스
    public GameObject mainCanvas;
    public GameObject tutorialCanvas;
    public GameObject settingCanvas;
    public GameObject endCanvas;
    public GameObject clearCanvas;

    // public GameObject mainSettingCanvas; 
    public GameObject shopCanvas;  // 상점 캔버스

    public Camera mainCamera;
    public Camera camera1;

    public GameObject HeroKnight;
    public GameObject enemyPrefab;
    public Transform[] enemySpawnPoints;
    public ScrollingText scrollingText;

    private bool isTutorialActive = false;
    private GameObject currentEnemy;
    public Text coinText; // 코인 텍스트 UI 요소
    public int coinAmount = 0; // 코인 변수

    void Start()
    { 
        settingCanvas.SetActive(false);
        UpdateHearts();  // 하트 UI 업데이트
        // ShowMainCanvas();
        UpdateCoinText(); // 초기 코인 텍스트 업데이트
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        HideClearCanvas();
    }
    void Update()
{
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        bool isActive = !settingCanvas.activeSelf;
        settingCanvas.SetActive(isActive);
        Cursor.visible = isActive;
        Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
    }
    if (gameCleared) // 게임 클리어 상태일 때
        {
            ShowClearCanvas(); // 게임 클리어 화면 표시
        }
}
public void ShowClearCanvas()
{
    if (clearCanvas != null)
    {
        clearCanvas.SetActive(true);

        
    }
}

public void HideClearCanvas()
{
    if (clearCanvas != null)
    {
        clearCanvas.SetActive(false);

        // ScrollingText 비활성화
        if (scrollingText != null)
        {
            scrollingText.gameObject.SetActive(false);
        }
    }
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

    
public void SetGameCleared()
    {
        gameCleared = true;
    }        public void DeductCoins(int amount)
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
      
        SwitchToCamera1();
        LoadGameScene();
        isTutorialActive = true;
        HeroKnight.SetActive(true);
        playerCanvas.SetActive(true);
        settingCanvas.SetActive(false);
        SpawnEnemy();
        isGoblinActive = true; // 고블린 활성화
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
       
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
    

    public void UpdateHearts()
    {
        HeroKnight heroKnight = FindAnyObjectByType<HeroKnight>();
        // 체력 10마다 하트 1개씩 표시하도록 heartIndex를 설정
        int heartIndex = Mathf.FloorToInt(heroKnight.currentHealth / 10f);
        
        for (int i = 0; i < hearts.Length; i++)
        {
            // 하트 배열이 비어있지 않고, 할당이 되었는지 확인하는 코드 추가
            if (hearts[i] == null)
            {
                Debug.LogWarning($"hearts[{i}] is not assigned.");
                continue;
            }
            
            // 체력에 따라 하트 활성화 여부 결정
            hearts[i].enabled = i < heartIndex;
        }
        
        Debug.Log($"Updated hearts: currentHealth = {heroKnight.currentHealth}, heartIndex = {heartIndex}");
    }




    public void OnReturnButtonClicked()
    {
        LoadGameScene();
    }

    public void OnBackButtonClicked()
    {
        HideSettingCanvas();
    }

    public void OnFinishButtonClicked()
    {
        LoadMainScene();
        // ShowMainCanvas();
    }

    public void OnAgainButtonClicked()
    {
        LoadGameScene();
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

    // private void ShowMainCanvas()
    // {
    //     if (mainCanvas != null)
    //         mainCanvas.SetActive(true);
        
    //     // if (mainSettingCanvas != null)
    //     //     mainSettingCanvas.SetActive(true);

    //     if (tutorialCanvas != null)
    //         tutorialCanvas.SetActive(false);

    //     if (endCanvas != null)
    //         endCanvas.SetActive(false);

    //     if (clearCanvas != null)
    //         clearCanvas.SetActive(false);

    //     if (playerCanvas != null)
    //         playerCanvas.SetActive(true);

    //     SwitchToMainCamera();
    //     isTutorialActive = false;

    //     // if (mainSettingCanvas != null)
    //     //     mainSettingCanvas.SetActive(false);
    
    //     Cursor.visible = true;
    //     Cursor.lockState = CursorLockMode.None;
    // }

    private void ShowTutorialCanvas()
    {
        SetCanvasActive(false, mainCanvas);
        SetCanvasActive(true, tutorialCanvas);
        isTutorialActive = true;
    }

    private void HideAllCanvases()
    {
        SetCanvasActive(false, mainCanvas, tutorialCanvas, settingCanvas, endCanvas, clearCanvas, playerCanvas);
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
    

    private void SetCanvasActive(bool isActive, params GameObject[] canvases)
    {
        foreach (GameObject canvas in canvases)
        {
            canvas.SetActive(isActive);
        }
    }

    public void ShowEndCanvas() {
    if (endCanvas != null) {      
        endCanvas.SetActive(true);   // End Canvas만 활성화
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    } else {
        Debug.LogWarning("endCanvas가 할당되지 않았습니다.");
    }
}
}