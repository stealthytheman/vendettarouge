using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Animations;
using System.Collections.Generic;

public class ToolTip : MonoBehaviour
{
    public static ToolTip Instance;

    public RectTransform tooltipRectTransform;
    public TextMeshProUGUI tooltipText;

    public Vector2 mousePosition;
    public Vector2 offset;

    public List<GameObject> children;

    void Awake()
    {
        Instance = this;
        Hide();

        children = new List<GameObject> { transform.GetChild(0).gameObject, transform.GetChild(1).gameObject };
    }

    void Update()
    {
        mousePosition = Input.mousePosition;
        foreach (GameObject child in children)
        {
            child.transform.position = mousePosition + offset;
        }
        //print($"Mouse Position: {mousePosition}");
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
