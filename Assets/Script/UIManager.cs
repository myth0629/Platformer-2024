// UIManager.cs
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIManager : MonoBehaviour
{
    public Slider healthBar; // 체력바 슬라이더
    public Text healthText;  // 체력 텍스트
    public GameObject mainCanvas;
    public GameObject tutorialCanvas;
    public GameObject settingCanvas;
    public GameObject endCanvas;
    public GameObject clearCanvas;
    public GameObject playerCanvas; // 플레이어 체력바 캔버스 추가

    public Camera mainCamera;
    public Camera camera1;

    public Button againButton;
    public Button finishButton;

    public GameObject HeroKnight;  // 플레이어 오브젝트
    public GameObject enemyPrefab;  // 적의 프리팹
    public Transform[] enemySpawnPoints;  // 적이 스폰될 수 있는 위치들

    private bool isSettingVisible = false;
    private bool isTutorialActive = false;
    private GameObject currentEnemy;

    public Text elapsedTimeText; // 경과 시간을 표시할 텍스트 UI 요소

    private float startTime; // 게임 시작 시점
    private float elapsedTime; // 경과 시간

    void Start()
    {
        // 초기화 코드
        ShowMainCanvas();
        settingCanvas.SetActive(false);
        HeroKnight.SetActive(false);
        playerCanvas.SetActive(false);
        enemyPrefab.SetActive(false);
        elapsedTime = 0f;

        // 게임 시작 시 커서 보이도록 설정
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (isTutorialActive)
        {
            // ESC 키 입력으로 SettingCanvas 토글 (Camera1이 활성화된 경우에만)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleSettingCanvas();
            }

            // 경과 시간 업데이트
            elapsedTime = Time.time - startTime;
            UpdateElapsedTimeText();
        }
    }

    public void OnEndButtonClicked()
    {
        // 애플리케이션 종료
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnStartButtonClicked()
    {
        // MainCanvas 끄고 TutorialCanvas 켜기
        ShowTutorialCanvas();
        startTime = Time.time; // 게임 시작 시점 기록
    }

    public void OnNextButtonClicked()
    {
        // Camera1으로 전환하고 SettingCanvas 제어 가능
        HideAllCanvases();
        SwitchToCamera1();
        isTutorialActive = true;
        isSettingVisible = false;

        // Player와 playerCanvas 활성화
        HeroKnight.SetActive(true);
        playerCanvas.SetActive(true);

        // Enemy 랜덤 스폰 및 활성화
        SpawnEnemy();
        // 튜토리얼 시작 시 커서를 보이게 설정
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }   
    public void OnReturnButtonClicked()
    {
        // MainCanvas로 돌아가기
        ShowMainCanvas();
    }

    public void OnBackButtonClicked()
    {
        // SettingCanvas 닫기
        HideSettingCanvas();
    }

    public void OnAgainButtonClicked()
    {
        // 게임을 다시 시작: Tutorial에서 NextButton을 누른 것과 같은 동작
        ResetGame();
        OnNextButtonClicked();
        startTime = Time.time; // 게임 시작 시점 기록
    }

    public void OnFinishButtonClicked()
    {
        // 메인 화면으로 돌아가기
        ResetGame();
        ShowMainCanvas();
    }

    private void ResetGame()
    {
        // Player 오브젝트 찾기
        HeroKnight playerScript = HeroKnight.GetComponent<HeroKnight>();
        if (playerScript != null)
        {
            playerScript.currentHealth = playerScript.maxHealth;  // 플레이어 체력 리셋
        }

        // Again 버튼을 눌렀을 때 튜토리얼 상태로 돌아가기
        HideAllCanvases();
        HeroKnight.SetActive(false);
        playerCanvas.SetActive(false);

        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
        }

        // 경과 시간 초기화
        elapsedTime = 0f;
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
    }

    private void ShowMainCanvas()
    {
        mainCanvas.SetActive(true);
        tutorialCanvas.SetActive(false);
        settingCanvas.SetActive(false);
        endCanvas.SetActive(false);
        clearCanvas.SetActive(false);
        playerCanvas.SetActive(false);
        SwitchToMainCamera();
        isTutorialActive = false;
        // 메인 화면으로 돌아왔을 때 커서를 보이게 설정
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void ShowTutorialCanvas()
    {
        mainCanvas.SetActive(false);
        tutorialCanvas.SetActive(true);
        isTutorialActive = true;  // TutorialCanvas 활성화
    }

    public void ShowEndCanvas()
    {
        // EndCanvas를 표시하고 나머지는 비활성화
        HideAllCanvases();
        endCanvas.SetActive(true);
        playerCanvas.SetActive(false); // EndCanvas가 보일 때 playerCanvas 비활성화

        // Player 비활성화
        HeroKnight.SetActive(false);

        // 현재 스폰된 Enemy 비활성화
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
        }
    }

    public void ShowClearCanvas()
    {
        // ClearCanvas를 표시하고 나머지는 비활성화
        HideAllCanvases();
        clearCanvas.SetActive(true);
        playerCanvas.SetActive(false); // ClearCanvas가 보일 때 playerCanvas 비활성화

        // Player 비활성화
        HeroKnight.SetActive(false);

        // 현재 스폰된 Enemy 비활성화
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
        }
    }

    private void HideAllCanvases()
    {
        mainCanvas.SetActive(false);
        tutorialCanvas.SetActive(false);
        settingCanvas.SetActive(false);
        endCanvas.SetActive(false);
        clearCanvas.SetActive(false);
        playerCanvas.SetActive(false); // 모든 캔버스를 숨길 때 playerCanvas 비활성화
    }

    private void ToggleSettingCanvas()
    {
        if (isTutorialActive) // Camera1이 활성화된 경우에만 설정 가능
        {
            isSettingVisible = !isSettingVisible;
            settingCanvas.SetActive(isSettingVisible);
        }
    }

    private void HideSettingCanvas()
    {
        isSettingVisible = false;
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

    private void SpawnEnemy()
    {
        // 랜덤한 위치에서 Enemy를 스폰
        if (enemySpawnPoints.Length > 0 && enemyPrefab != null)
        {
            int randomIndex = Random.Range(0, enemySpawnPoints.Length);
            currentEnemy = Instantiate(enemyPrefab, enemySpawnPoints[randomIndex].position, Quaternion.identity);
            currentEnemy.SetActive(true); // 스폰된 적 활성화
        }
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
        {
            float healthRatio = currentHealth / maxHealth;
            healthBar.value = Mathf.Clamp01(healthRatio);
        }
    }
}