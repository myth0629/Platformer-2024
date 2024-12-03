using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollingText : MonoBehaviour
{
    public float speed = 150f; 
    private RectTransform rectTransform;
    public GameObject imageObject;


    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

   
        rectTransform.anchoredPosition = new Vector2(0, -Screen.height); // 화면 하단 밖에서 시작
        if (imageObject != null)
        {
            imageObject.SetActive(false);
        }
    }

    void Update()
    {
        if (rectTransform != null && gameObject.activeSelf)
        {
        
            if (rectTransform.anchoredPosition.y < rectTransform.rect.height / 2)
            {
                rectTransform.anchoredPosition += Vector2.up * speed * Time.deltaTime;
            }

 
            if (rectTransform.anchoredPosition.y >= rectTransform.rect.height / 2)
            {
                rectTransform.anchoredPosition = new Vector2(0, rectTransform.rect.height / 2 ); 
            }


            if (imageObject != null)
            {
                StartCoroutine(ActivateImageAfterDelay(7f));
            }
        }
    }

    public void ResetPosition()
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(0, -rectTransform.rect.height / 2); // 초기 위치로 설정
        }

        gameObject.SetActive(true);
        if (imageObject != null)
        {
            imageObject.SetActive(false);
        }
    }

    private IEnumerator ActivateImageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (imageObject != null)
        {
            imageObject.SetActive(true); 
            Debug.Log("이미지가 활성화되었습니다.");
        }
    }
}
