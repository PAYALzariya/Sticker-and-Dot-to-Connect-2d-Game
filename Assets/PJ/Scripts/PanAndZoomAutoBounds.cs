using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
public class PanAndZoomAutoBounds : MonoBehaviour
{
    public Camera cam;
    private Vector3 dragOrigin;
    private void Update()
    {
        PanCamera();
    }
    private void PanCamera()
    {
        if(Input.GetMouseButtonDown(0))
        {dragOrigin=cam.ScreenToWorldPoint(Input.mousePosition);

            }
        if (Input.GetMouseButton(0))
        {
            Vector3 diffrencer=dragOrigin-cam.ScreenToWorldPoint(Input.mousePosition);
            print("ORif=gin " + dragOrigin + "N");
            cam.transform.position += diffrencer;
        }
    }
}
/* [Header("Zoom Settings")]
 public float zoomSpeedTouch = 0.01f;
 public float zoomSpeedMouse = 0.5f;
 public float minScale = 0.5f;
 public float maxScale = 3f;

 [Header("Pan Settings")]
 public float panSpeed = 0.01f;

 private Camera cam;
 private Vector2 levelSize;
 private bool isDragging = false;
 private Vector3 lastMousePos;

 private void Start()
 {
     cam = Camera.main;

     // auto-size from children
     levelSize = GetChildrenBounds(gameObject);
     Debug.Log("Level Size: " + levelSize);
 }

 private void Update()
 {
#if UNITY_EDITOR || UNITY_STANDALONE
     HandleMousePan();
     HandleMouseZoom();
#endif
#if UNITY_ANDROID || UNITY_IOS
     HandleTouchPanAndZoom();
#endif
     ClampPosition();
 }

 // -----------------------------
 // PAN
 // -----------------------------
 private void HandleMousePan()
 {
     if (Input.GetMouseButtonDown(0))
     {
         isDragging = true;
         lastMousePos = Input.mousePosition;  // screen space
     }
     else if (Input.GetMouseButtonUp(0))
     {
         isDragging = false;
     }

     if (isDragging)
     {
         Vector3 delta = Input.mousePosition - lastMousePos;
         transform.Translate(-delta.x * panSpeed, -delta.y * panSpeed, 0);
         lastMousePos = Input.mousePosition;
     }
 }

 // -----------------------------
 // ZOOM
 // -----------------------------
 private void HandleMouseZoom()
 {
     float scroll = Input.GetAxis("Mouse ScrollWheel");
     if (Mathf.Abs(scroll) > 0.01f)
     {
         ZoomAtPosition(Input.mousePosition, scroll * zoomSpeedMouse);
     }
 }

 private void HandleTouchPanAndZoom()
 {
     if (Input.touchCount == 1)
     {
         Touch t = Input.GetTouch(0);
         if (t.phase == TouchPhase.Moved)
         {
             transform.Translate(-t.deltaPosition.x * panSpeed, -t.deltaPosition.y * panSpeed, 0);
         }
     }
     else if (Input.touchCount == 2)
     {
         Touch t0 = Input.GetTouch(0);
         Touch t1 = Input.GetTouch(1);

         Vector2 prevDist = (t0.position - t0.deltaPosition) - (t1.position - t1.deltaPosition);
         Vector2 currDist = t0.position - t1.position;
         float delta = currDist.magnitude - prevDist.magnitude;

         Vector2 mid = (t0.position + t1.position) * 0.5f;
         ZoomAtPosition(mid, delta * zoomSpeedTouch);
     }
 }

 private void ZoomAtPosition(Vector2 screenPos, float delta)
 {
     Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);

     float newScale = Mathf.Clamp(transform.localScale.x + delta, minScale, maxScale);
     float scaleFactor = newScale / transform.localScale.x;

     transform.localScale = Vector3.one * newScale;

     Vector3 direction = transform.position - worldPos;
     transform.position = worldPos + direction * scaleFactor;
 }

 // -----------------------------
 // CLAMP
 // -----------------------------
 private void ClampPosition()
 {
     float halfWidth = (levelSize.x * transform.localScale.x) / 2f;
     float halfHeight = (levelSize.y * transform.localScale.y) / 2f;

     Vector3 pos = transform.position;
     pos.x = Mathf.Clamp(pos.x, -halfWidth, halfWidth);
     pos.y = Mathf.Clamp(pos.y, -halfHeight, halfHeight);
     transform.position = pos;
 }

 // -----------------------------
 // Utility: bounds from children
 // -----------------------------
 private Vector2 GetChildrenBounds(GameObject go)
 {
     if (go.transform.childCount == 0)
         return Vector2.one;

     Vector3 min = go.transform.GetChild(0).localPosition;
     Vector3 max = min;

     foreach (Transform child in go.transform)
     {
         min = Vector3.Min(min, child.localPosition);
         max = Vector3.Max(max, child.localPosition);
     }

     Vector3 size = max - min;
     return new Vector2(size.x, size.y);
 }
}*/