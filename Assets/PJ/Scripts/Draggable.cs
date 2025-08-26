using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

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

    public Vector2 smallSize = new Vector2(250, 250);
    private Vector2 originalSize;
    private Vector2 originalPosition;  // Store original pos
    public RectTransform id;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();

    }

    // Sprite refSprite = null;
    public RectTransform MyRect;
    void Start()
    {
        rectTransform.sizeDelta = smallSize;
        originalPosition = rectTransform.anchoredPosition;  // Save start position

        originalPos = rectTransform.anchoredPosition;

        Image img = GetComponent<Image>();
        /*foreach (var tagged in StickerGameManager.instance.refSprites)
        {
            if (tagged.sprite == img.sprite)
            {
                refSprite = tagged.sprite;
            }
        }*/
        for (int i = 0; i < PuzzleManager.instance.levelemptyImage.Count; i++)
        {


            if (this.transform.parent.GetComponent<Image>().sprite.name == PuzzleManager.instance.levelemptyImage[i].GetComponent<Image>().sprite.name)
            {
                MyRect = PuzzleManager.instance.levelemptyImage[i].GetComponent<RectTransform>();
            }
        }
        if (MyRect != null)
        {
            float width = MyRect.rect.width/* / MyRect.pixelsPerUnit * 100f*/;
            float height = MyRect.rect.height /*/ MyRect.pixelsPerUnit * 100f*/;
            originalSize = new Vector2(width, height);
            Debug.Log("found " + originalSize);
        }

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
        if (MyRect != null)
        {
            float width = MyRect.rect.width/* / MyRect.pixelsPerUnit * 100f*/;
            float height = MyRect.rect.height /*/ MyRect.pixelsPerUnit * 100f*/;
            originalSize = new Vector2(width, height);
            Debug.Log("found " + originalSize);
        }
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.85f;
        rectTransform.sizeDelta = GetScaledSize(originalSize);
        //rectTransform.sizeDelta = originalSize;

    }
    private Vector2 GetScaledSize(Vector2 baseSize)
    {
        if (PuzzleManager.instance.contentREct == null) return baseSize;
        Vector3 scale = PuzzleManager.instance.contentREct.localScale;
        return new Vector2(baseSize.x * scale.x, baseSize.y * scale.y);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!enabled || state != DragState.Dragging) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        //   transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        //  rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!enabled || state != DragState.Dragging) return;
        state = DragState.Idle;
        OnDragStop(this);
        // rectTransform.parent = null;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;


        //  transform.localScale = new Vector3(1f, 1f, 1);
        rectTransform.anchoredPosition = originalPosition;
          rectTransform.sizeDelta = smallSize;
       // rectTransform.sizeDelta = GetScaledSize(smallSize);
    } 
    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.sizeDelta = smallSize;
      //  rectTransform.sizeDelta = GetScaledSize(smallSize);
    }
}

