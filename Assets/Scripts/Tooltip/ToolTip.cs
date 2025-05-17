using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    public static ToolTip Instance;

    public RectTransform tooltipRectTransform;
    public TextMeshProUGUI tooltipText;

    void Awake()
    {
        Instance = this;
        Hide();
    }

    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        transform.position = mousePosition;
    }

    public void Show(string text)
    {
        tooltipText.text = text;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
