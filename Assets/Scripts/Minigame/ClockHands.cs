using UnityEngine;

public class ClockHands : MonoBehaviour
{
    public RectTransform hourHand;
    public RectTransform minuteHand;
    public float hourSpeed = 10f;
    public float minuteSpeed = 60f;

    void Update()
    {
        // Rotate hands clockwise (positive angles)
        hourHand.Rotate(0, 0, hourSpeed * Time.deltaTime);
        minuteHand.Rotate(0, 0, minuteSpeed * Time.deltaTime);
    }
}
