using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform rectTransform;
    public Canvas canvas;
    public Vector2 correctPosition;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvas=FindAnyObjectByType<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Init(Sprite sprite, Vector2 correctPos)
    {
        GetComponent<Image>().sprite = sprite;
        correctPosition = correctPos;
        rectTransform.anchoredPosition = correctPos;

        Debug.Log($"Piece {name} placed at: {correctPos}");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (Vector2.Distance(rectTransform.anchoredPosition, correctPosition) < 50f)
        {
            rectTransform.anchoredPosition = correctPosition;
            Debug.Log("Right place");
            // Optional: lock dragging, disable component, play sound
        }
    }
}
