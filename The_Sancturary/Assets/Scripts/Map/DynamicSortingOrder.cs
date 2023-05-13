using UnityEngine;

public class DynamicSortingOrder : MonoBehaviour
{
    private int sortingOrderBase = 5000;
    private int offset = 1000;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        // Try to get the SpriteRenderer component on the parent object
        spriteRenderer = GetComponent<SpriteRenderer>();

        // If the parent object doesn't have a SpriteRenderer, search for it in the children
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = (int)(sortingOrderBase - transform.position.y * 100) + offset;
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on the parent or its children.");
        }
    }
}
