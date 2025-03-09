using System.Collections;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    //Blok �zellikleri
    public int gridX { get; set; }
    public int gridY { get; set; }
    public Sprite blockSprite { get; set; }

    private SpriteRenderer blockRenderer;

    void Awake()
    {
        blockRenderer = GetComponent<SpriteRenderer>();
    }
    //Olu�turulan blo�a �zellik tan�mlamas�
    public void Initialize(int x, int y, Sprite sprite)
    {
        gridX = x;
        gridY = y;
        blockSprite = sprite;
        blockRenderer.sprite = blockSprite;
    }
    //Blok i�in hareket metodu
    public void MoveTo(int x, int y)
    {
        gridX = x;
        gridY = y;
        StartCoroutine(MoveSmoothly(new Vector2(x, y)));
    }
    public void SetSprite(Sprite sprite)
    {
        blockSprite = sprite;
        blockRenderer.sprite = blockSprite;
    }
    //Blok se�ili konuma ula�ana kadar hareket ettir.
    public IEnumerator MoveSmoothly(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = 0.4f;

        Vector3 startPosition = transform.position;
        while (elapsedTime < duration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime/duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}
