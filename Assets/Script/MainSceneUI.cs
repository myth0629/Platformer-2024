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


    public Camera mainCamera;

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
    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainMenu");
    }


    private void ShowMainCanvas()
    {
        if (mainCanvas != null)
            mainCanvas.SetActive(true);

        if (tutorialCanvas != null)
            tutorialCanvas.SetActive(false);

    
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void ShowTutorialCanvas()
    {
        SetCanvasActive(false, mainCanvas);
        SetCanvasActive(true, tutorialCanvas);
        isTutorialActive = true;
    }

    // private void HideAllCanvases()
    // {
    //     SetCanvasActive(false, mainCanvas, tutorialCanvas, mainSettingCanvas);
    // }



    private void SetCanvasActive(bool isActive, params GameObject[] canvases)
    {
        foreach (GameObject canvas in canvases)
        {
            canvas.SetActive(isActive);
        }
    }
}