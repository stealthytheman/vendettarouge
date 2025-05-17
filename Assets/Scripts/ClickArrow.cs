using UnityEngine;

public class ClickArrow : MonoBehaviour
{
    public ScrollBackground scrollBackground;
    public bool isRightArrow = true; // Set in Inspector: true for right, false for left

    void Start()
    {
        if (scrollBackground == null)
        {
            Debug.LogWarning("ScrollBackground reference not set on ClickArrow!");
        }
    }
    private void OnMouseDown()
    {
        if (scrollBackground == null)
        {
            Debug.LogWarning("ScrollBackground reference not set on ClickArrow!");
            return;
        }

        if (isRightArrow)
        {
            scrollBackground.ScrollRight();
        }
        else
        {
            scrollBackground.ScrollLeft();
        }
    }
}