using UnityEngine;
using System.Collections;

public class DamageObject : MonoBehaviour
{
    public float speed = 5f;  // 오브젝트의 이동 속도
    public float lifetime = 5f;  // 오브젝트가 존재하는 시간 (초)
    public float damageAmount = 10f;  // 데미지 양

    private float distanceTraveled = 0f;

    void Start()
    {
        // lifetime 후에 오브젝트를 파괴
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // 오브젝트를 오른쪽으로 이동
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // 이동한 거리 계산
        distanceTraveled += speed * Time.deltaTime;

        // 디버그용: 콘솔에 이동 거리 출력
        Debug.Log("Distance Traveled: " + distanceTraveled);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어에게 데미지를 입힘
            HeroKnight playerHealth = other.GetComponent<HeroKnight>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}