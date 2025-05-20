using UnityEngine;

public class ClockHands : MonoBehaviour
{
    public RectTransform hourHand;
    public RectTransform minuteHand;
    public float hourSpeed = 10f;
    public float minuteSpeed = 60f;
    public bool gameStarted = false;

    void Update()
    {
        if (!gameStarted) return;

        if (GameFlags.cameFromRoom == 1)
        {
            hourHand.Rotate(0, 0, hourSpeed * Time.deltaTime);
            minuteHand.Rotate(0, 0, minuteSpeed * Time.deltaTime);
        }
        else if (GameFlags.cameFromRoom == 2)
        {
            hourHand.Rotate(0, 0, -hourSpeed * Time.deltaTime);
            minuteHand.Rotate(0, 0, -minuteSpeed * Time.deltaTime);
        }
    }
}
