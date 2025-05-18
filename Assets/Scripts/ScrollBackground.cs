using UnityEngine;

public class ScrollBackground : MonoBehaviour
{
    public float scrollAmount = 1f;      // How much to move per click
    public float minX = -5f;            // Leftmost position
    public float maxX = 10f;             // Rightmost position

   
    public void ScrollRight()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Min(pos.x + scrollAmount, maxX);
        transform.position = pos;
    }

   
    public void ScrollLeft()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Max(pos.x - scrollAmount, minX);
        transform.position = pos;
    }
}
