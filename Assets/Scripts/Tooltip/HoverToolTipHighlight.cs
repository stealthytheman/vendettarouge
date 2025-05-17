using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverToolTipHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string description = "placeholder description";

    public GameObject normalObject;
    public GameObject highlightObject;

    void Start()
    {
        normalObject.SetActive(true);
        highlightObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("Mouse Entered");
        ToolTip.Instance.Show(description);

        normalObject.SetActive(false);
        highlightObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("Mouse Exited");
        {
            ToolTip.Instance.Hide();

            normalObject.SetActive(true);
            highlightObject.SetActive(false);
        }
    }
}
