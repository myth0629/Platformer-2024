using UnityEngine;

public class HpItem : MonoBehaviour
{
    public int restoreAmount = 20;  // 회복할 체력 양
    private HeroKnight heroKnight;

    void Start()
    {
        heroKnight = FindObjectOfType<HeroKnight>();  // HeroKnight 찾기
    }

    public void OnUse()
    {
        if (heroKnight != null)
        {
            
            Destroy(gameObject);  // 아이템 사용 후 파괴
        }
    }
}