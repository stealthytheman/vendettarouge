using UnityEngine;

public class ClockHands : MonoBehaviour
{
    public RectTransform hourHand;
    public RectTransform minuteHand;
    public float hourSpeed = 10f;
    public float minuteSpeed = 60f;

    void Update()
    {

         // 1 = backward in time 2 = forward in time
        if (GameFlags.cameFromRoom == 1)
        {
           // Rotate hands clockwise (positive angles)
            hourHand.Rotate(0, 0, hourSpeed * Time.deltaTime);
            minuteHand.Rotate(0, 0, minuteSpeed * Time.deltaTime);
        }
        else if (GameFlags.cameFromRoom == 2)
        {
            // Rotate hands counter-clockwise (negative angles)
            hourHand.Rotate(0, 0, -hourSpeed * Time.deltaTime);
            minuteHand.Rotate(0, 0, -minuteSpeed * Time.deltaTime);
        }
    }
}
