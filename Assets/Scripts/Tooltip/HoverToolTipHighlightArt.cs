using UnityEngine;
using UnityEngine.EventSystems;

public class HoverToolTipHighlightArt : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string description;
    public GameObject normalObject;
    public GameObject highlightObject;

    public bool artClicked = false;

    void Start()
    {
        normalObject.SetActive(true);
        highlightObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTip.Instance.Show(description);
        normalObject.SetActive(false);
        highlightObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTip.Instance.Hide();
        normalObject.SetActive(true);
        highlightObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        artClicked = true;
    }
}
