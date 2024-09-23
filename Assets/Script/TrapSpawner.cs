using UnityEngine;

public class TrapSpawner : MonoBehaviour
{
    public GameObject damageObjectPrefab;  // 생성할 데미지 오브젝트의 프리팹
    public float spawnInterval = 3f;  // 오브젝트 생성 간격 (초)
    public Vector3 spawnPosition;  // 오브젝트가 생성될 위치

    void Start()
    {
        // 3초마다 SpawnObject 메서드를 호출
        InvokeRepeating("SpawnObject", 0f, spawnInterval);
        spawnPosition = transform.position;
    }

    void SpawnObject()
    {
        // 지정된 위치에 데미지 오브젝트 생성
        Instantiate(damageObjectPrefab, spawnPosition, Quaternion.identity);
    }
}