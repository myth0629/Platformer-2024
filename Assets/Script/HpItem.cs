using UnityEngine;

public class HpItem : MonoBehaviour
{
    private HeroKnight heroKnight;

    void Start()
    {
        heroKnight = GameObject.Find("HeroKnight").GetComponent<HeroKnight>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Heal");
        if (other.gameObject.CompareTag("Player"))
        {
            heroKnight.RestoreHealth();

            Destroy(gameObject);
        }
    }
}