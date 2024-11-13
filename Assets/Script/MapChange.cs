using UnityEngine;

public class MoveOnCollision : MonoBehaviour
{
    public Vector2 targetPosition = new Vector2(0, 0);  // 이동할 목표 좌표

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트의 태그가 지정된 태그와 일치하는지 확인
        if (collision.gameObject.CompareTag("Player"))
        {
            // 충돌한 오브젝트의 위치를 targetPosition으로 이동
            collision.gameObject.transform.position = targetPosition;

            // 디버그 메시지 출력 (선택 사항)
            Debug.Log($"{collision.gameObject.name} has been moved to {targetPosition}");
        }
    }
}
