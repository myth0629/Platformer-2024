using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float movePower = 1f;

    Animator animator;
    SpriteRenderer spriteRenderer;
    Vector3 movement;
    int movementFlag = 0;


    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine("ChangeMovement");
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        Vector3 moveVelocity = Vector3.zero;

        if(movementFlag == 1)
        {
            moveVelocity = Vector3.left;
        }
        else if(movementFlag == 2)
        {
            moveVelocity = Vector3.right;
        }

        transform.position += moveVelocity * movePower * Time.deltaTime;
    }

    IEnumerator ChangeMovement()
    {
       movementFlag = Random.Range(0, 3);

       if(movementFlag == 0)
            animator.SetBool("isRun", false);
        else
            animator.SetBool("isRun", true);

        yield return new WaitForSeconds(3f);

        StartCoroutine("ChangeMovement");
    }
}