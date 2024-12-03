using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                // 게임 클리어 상태 설정
                uiManager.SetGameCleared();

                // 게임 클리어 UI 활성화
                uiManager.ShowClearCanvas(); 
            }
        }
    }
}