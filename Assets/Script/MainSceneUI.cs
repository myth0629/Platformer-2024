using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
#endif

public class MainSceneUI : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject tutorialCanvas;

    public GameObject mainSettingCanvas;

    public Camera mainCamera;

    public Text elapsedTimeText;
    private float elapsedTime;

    private bool isTutorialActive = false;

    void Start()
    { 
        ShowMainCanvas();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        
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

    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }


    private void ShowMainCanvas()
    {
        if (mainCanvas != null)
            mainCanvas.SetActive(true);
        
        if (mainSettingCanvas != null)
            mainSettingCanvas.SetActive(true);

        if (tutorialCanvas != null)
            tutorialCanvas.SetActive(false);

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
        SetCanvasActive(false, mainCanvas, tutorialCanvas, mainSettingCanvas);
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