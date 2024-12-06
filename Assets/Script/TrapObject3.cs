using UnityEngine;
using System.Collections;

public class DamageObject3 : MonoBehaviour
{
    public float speed = 5f;  // 오브젝트의 이동 속도
    public float lifetime = 3f;  // 오브젝트가 존재하는 시간 (초)
    public float damageAmount = 10f;  // 데미지 양

    private float distanceTraveled = 0f;

    void Start()
    {
        // lifetime 후에 오브젝트를 파괴
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // 플레이어에게 데미지를 입힘
            HeroKnight playerHealth = other.gameObject.GetComponent<HeroKnight>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                Debug.Log("Player Hit!!");

                Destroy(gameObject);
            }
        }
        
    }
}