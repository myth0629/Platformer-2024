using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public UIManager uiManager; // UIManager를 참조하기 위한 변수
    public Button againButton; // 다시 시작 버튼
    public Button finishButton; // 종료 버튼
    public Button hpPlusButton; // HP 증가 버튼
    public Button closeShopBtn; // 상점 닫기 버튼 추가
    public Button returnBtn; // 게임 초기화 버튼 추가
    
    // MainSettingCanvas 버튼
    public Button button1; 
    public Button button2;
    public Button button3; 
    public Button button4;

    private HeroKnight player; // 플레이어 스크립트 참조 추가

    void Start()
    {
        player = FindObjectOfType<HeroKnight>(); // 플레이어 오브젝트의 HeroKnight 스크립트 참조
        
        // 버튼에 클릭 이벤트 추가
        againButton.onClick.AddListener(uiManager.OnAgainButtonClicked);
        finishButton.onClick.AddListener(uiManager.OnFinishButtonClicked);
        hpPlusButton.onClick.AddListener(OnHpPlusButtonClicked);
        closeShopBtn.onClick.AddListener(OnCloseShopButtonClicked);
        returnBtn.onClick.AddListener(OnReturnButtonClicked); // returnBtn 클릭 이벤트 추가
        
        // MainSettingCanvas 버튼 클릭 이벤트 추가
        button1.onClick.AddListener(OnButton1Clicked);
        button2.onClick.AddListener(OnButton2Clicked);
        button3.onClick.AddListener(OnButton3Clicked);
        button4.onClick.AddListener(OnButton4Clicked);

        // 버튼 2, 3, 4를 비활성화
        SetButtonsActive(false, button2, button3, button4);
        
        // returnBtn 초기화 (게임 처음 시작했을 때처럼)
        ResetReturnButton();
    }

    private void ResetReturnButton()
    {
        // returnBtn을 활성화 상태로 초기화
        returnBtn.gameObject.SetActive(true); 
    }

    private void OnHpPlusButtonClicked()
{
    Debug.Log("HP 증가 버튼 클릭됨!");

    HeroKnight playerScript = player.GetComponent<HeroKnight>();
    if (playerScript != null)
    {
        // HP 증가
        playerScript.currentHealth += 20;
        if (playerScript.currentHealth > playerScript.maxHealth)
        {
            playerScript.currentHealth = playerScript.maxHealth; // 최대 체력 초과 방지
        }
        
        // 코인 감소 로직
        int hpPlusCost = 200; // HP 증가 비용

        if (uiManager.coinAmount >= hpPlusCost) // uiManager에서 코인 수 확인
        {
            uiManager.coinAmount -= hpPlusCost; // 코인 차감
            uiManager.UpdateCoinText(); // 코인 텍스트 업데이트
            Debug.Log($"코인이 {hpPlusCost} 감소했습니다. 남은 코인: {uiManager.coinAmount}");
        }
        else
        {
            Debug.Log("코인이 부족합니다.");
        }

        // 하트 UI 업데이트
        uiManager.UpdateHearts();
    }
}
    private void OnCloseShopButtonClicked()
    {
        Debug.Log("상점 닫기 버튼 클릭됨!");
        uiManager.CloseShopCanvas(); // UIManager의 CloseShopCanvas 메서드 호출
    }
    
    private void OnButton1Clicked()
    {
        SetButtonsActive(true, button2, button3, button4);
    }

    private void OnButton2Clicked() { }

    private void OnButton3Clicked() { }

    private void OnButton4Clicked()
    {
        SetButtonsActive(false, button2, button3, button4);
    }
    
    private void OnReturnButtonClicked() // 게임 초기화 메서드
    {
        Debug.Log("게임 초기화 버튼 클릭됨!");
        uiManager.ResetGameToInitialState(); // UIManager의 ResetGameToInitialState 메서드 호출
    }

    private void SetButtonsActive(bool isActive, params Button[] buttons)
    {
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(isActive);
        }
    }
}