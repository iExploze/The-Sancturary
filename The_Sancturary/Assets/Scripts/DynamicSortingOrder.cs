using UnityEngine;

public class DynamicSortingOrder : MonoBehaviour
{
    private int sortingOrderBase = 5000;
    private int offset = 1000;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        spriteRenderer.sortingOrder = (int)(sortingOrderBase - transform.position.y * 100) + offset;
    }
}
