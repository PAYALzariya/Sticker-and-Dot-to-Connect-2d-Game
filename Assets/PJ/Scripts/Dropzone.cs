using UnityEngine;
using UnityEngine.EventSystems;
using System;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class Dropzone : MonoBehaviour
{
    RectTransform rectTransform;
    Dropable _ref = null;

    public Action<Dropzone> OnDrop = delegate { };
    public Action<Dropzone> OnLift = delegate { };

    // --- Label-matching options (matches your old behaviour) ---
    [Header("Matching")]
    [Tooltip("Require the TMP text on the dragged item to match this Dropzone's TMP text.")]
    public bool requireLabelMatch = true;

    [Tooltip("Dragged label is searched at: draggedRect.parent.GetChild(draggedLabelChildIndex)")]
    public int draggedLabelChildIndex = 1;

    [Tooltip("The TMP label on this Dropzone. If null, we try GetComponent<TextMeshProUGUI>().")]
    public TextMeshProUGUI dropzoneLabel;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (dropzoneLabel == null)
            dropzoneLabel = this.GetComponent<TextMeshProUGUI>(); // matches your old setup
    }

    public bool IsFull => _ref != null;

    public bool CanDrop(RectTransform draggedRect, PointerEventData eventData)
    {
        if (!enabled || IsFull) return false;

           // Dragged text (assumes label is on draggedRect.parent.GetChild(1))
        TextMeshProUGUI draggedLabel = null;
        if (draggedRect != null && draggedRect.parent != null && draggedRect.parent.childCount > 1)
        {
            draggedLabel = draggedRect.parent.GetChild(1).GetComponent<TextMeshProUGUI>();
        }
        if (draggedLabel == null) return false;

        // Compare texts
        if (draggedLabel.text != dropzoneLabel.text) return false;

     //  Debug.Log(draggedRect.name + "   " + draggedLabel.text + "    ");
        // Final check: inside dropzone AND text matches
        return  draggedLabel.text == dropzoneLabel.text;
    }


    public void Drop(Dropable obj)
    {
        _ref = obj;
        OnDrop(this);
    }

    public void Lift()
    {
        _ref = null;
        OnLift(this);
    }
}
