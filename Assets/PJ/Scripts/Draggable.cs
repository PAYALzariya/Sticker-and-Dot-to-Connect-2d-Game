using UnityEngine;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(RectTransform))]
public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    enum DragState { Idle, Dragging };

    public Action<Draggable> OnDragStart = delegate { };
    public Action<Draggable> OnDragStop = delegate { };

    DragState state = DragState.Idle;

    Vector2 originalPos = Vector2.zero;
    public Vector2 OriginalPos => originalPos;

    RectTransform rectTransform;
    Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        originalPos = rectTransform.anchoredPosition;
    }

    public void SetOriginalPos(Vector2 newOriginalPos)
    {
        originalPos = newOriginalPos;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!enabled || state != DragState.Idle) return;
        state = DragState.Dragging;
        OnDragStart(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!enabled || state != DragState.Dragging) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!enabled || state != DragState.Dragging) return;
        state = DragState.Idle;
        OnDragStop(this);
        transform.localScale = new Vector3(1f, 1f, 1);
    }
}
