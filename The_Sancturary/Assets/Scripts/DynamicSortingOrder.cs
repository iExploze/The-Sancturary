using UnityEngine;

public class DynamicSortingOrder : MonoBehaviour
{
    public int sortingOrderBase = 5000;
    public int offset = 0;
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
