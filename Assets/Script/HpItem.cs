using UnityEngine;

public class HPItem : MonoBehaviour
{
    public float healAmount = 20f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HeroKnight heroKnight = other.GetComponent<HeroKnight>();
            if (heroKnight != null)
            {
                heroKnight.RestoreHealth(healAmount); // 체력 회복
                Destroy(gameObject); // 아이템 사용 후 제거
            }
        }
    }
}