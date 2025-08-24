using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using TMPro;
[RequireComponent(typeof(Draggable))]
public class Dropable : MonoBehaviour
{
    public float autoMoveArrivalThreshold = 5f;

    public Action<Dropable> OnDropComplete = delegate { };
    public Action<Dropable> OnReturnComplete = delegate { };

    public Func<Dropable, bool> OnDropAccepted = delegate { return true; };
    public Func<Dropable, bool> OnDropRejected = delegate { return true; };

   public List<Dropzone> dropZones = new List<Dropzone>();
    Dropzone targetDropzone = null;
    public Dropzone TargetDropzone => targetDropzone;

    Draggable _dragging;
    RectTransform rectTransform;

    void Awake()
    {
        _dragging = GetComponent<Draggable>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
      
        _dragging.OnDragStop = (_) => OnDragStop();
        OnDropComplete += HandleDropComplete;
    }

    void HandleDropComplete(Dropable d)
    {
        Debug.Log("Drop completed! ***" + d.name);
    }
    [SerializeField] private float dropDistanceThreshold = 0.4f; // you can tweak in Inspector

    void OnDragStop()
    {
        Dropzone newTargetDropzone = null;
        float minDist =0.4f;

        // 1. Check all dropzones manually
        dropZones.AddRange(FindObjectsOfType<Dropzone>());
        foreach (Dropzone dz in dropZones)
        {
            if (dz.CanDrop(rectTransform, null)) // sprite match
            {
                float dist = Vector2.Distance(
                    rectTransform.position,
                    dz.GetComponent<RectTransform>().position
                );

                if (dist < minDist)
                {
                    minDist = dist;
                    newTargetDropzone = dz;
                }
            }
        }

        // 2. Validate distance threshold
        if (newTargetDropzone != null && minDist <= dropDistanceThreshold)
        {
            if (targetDropzone) targetDropzone.Lift();
            targetDropzone = newTargetDropzone;

            if (OnDropAccepted(this))
            {
                Debug.Log(targetDropzone.GetComponent<TextMeshProUGUI>().text+ "Drop completed! ***" +targetDropzone.name);
                int index = int.Parse(targetDropzone.GetComponent<TextMeshProUGUI>().text);
                PuzzleManager.instance.LoadNewDragObject(index);
            }
        }
        else
        {
            // return to original position if too far or no valid zone
            if (OnDropRejected(this))
            {
                MoveToTargetUI.Go(
                    gameObject,
                    _dragging.OriginalPos,
                    autoMoveArrivalThreshold
                ).OnArrival = (_) => FinishReturn();
            }
        }
    }



    //  These were missing earlier
    bool FinishDrop()
    {
        targetDropzone.Drop(this);
        _dragging.SetOriginalPos(targetDropzone.GetComponent<RectTransform>().anchoredPosition);
        OnDropComplete(this);
        return true;
    }

    bool FinishReturn()
    {
        OnReturnComplete(this);
        return true;
    }
}
