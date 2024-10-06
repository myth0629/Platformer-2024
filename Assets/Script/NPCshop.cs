using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCshop : MonoBehaviour
{
    public GameObject shopCanvas; // 상점 캔버스
    public GameObject player; // 플레이어 오브젝트
    private bool isPlayerInRange = false; // 플레이어가 NPC 범위 내에 있는지 체크
    private bool isShopOpen = false; // 상점이 열려 있는지 체크
    public Text coinText; // 코인 텍스트를 참조할 변수

    private int currentCoins; // 현재 코인 수

    void Start()
    {
        shopCanvas.SetActive(false); // 게임 시작 시 상점 캔버스를 비활성화
        currentCoins = 500; // 초기 코인 수 
        UpdateCoinText();
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleShop();
        }
    }
     // 코인 수를 업데이트하는 메서드
    public void UpdateCoins(int amount)
    {
        currentCoins += amount;
        UpdateCoinText();
    }

    // 코인 텍스트를 업데이트하는 메서드
    private void UpdateCoinText()
    {
        coinText.text = "코인: " + currentCoins.ToString(); // 텍스트 업데이트
    }



    private void ToggleShop()
    {
        isShopOpen = !isShopOpen;
        shopCanvas.SetActive(isShopOpen);

        Time.timeScale = isShopOpen ? 0 : 1; // 상점이 열리면 게임 일시정지
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            isPlayerInRange = true; // 플레이어가 NPC 범위 안에 들어왔을 때
            Debug.Log("플레이어가 NPC 범위에 들어왔습니다.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            isPlayerInRange = false; // 플레이어가 NPC 범위 밖으로 나갔을 때
            Debug.Log("플레이어가 NPC 범위를 벗어났습니다.");
        }
    }
    
}