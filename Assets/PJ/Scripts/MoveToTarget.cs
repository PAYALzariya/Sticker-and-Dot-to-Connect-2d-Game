using UnityEngine;
using System;

public class MoveToTargetUI : MonoBehaviour
{
    const float default_speed = 10f;
    const float default_arrival_threshold = 2f;

    public float arrivalThreshold = default_arrival_threshold;
    public float speed = default_speed;

    public Func<MoveToTargetUI, bool> OnArrival = delegate { return true; };

    RectTransform rectTransform;
    Vector2 destination;
    bool inMotion = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public static MoveToTargetUI Go(GameObject toMove, Vector2 pos,
        float arrivalThreshold = default_arrival_threshold, float speed = default_speed)
    {
        MoveToTargetUI moveTo = toMove.GetComponent<MoveToTargetUI>();
        if (moveTo == null)
            moveTo = toMove.AddComponent<MoveToTargetUI>();

        moveTo.arrivalThreshold = arrivalThreshold;
        moveTo.speed = speed;
        moveTo.Go(pos);
        return moveTo;
    }

    public void Go(Vector2 pos)
    {
        if (Vector2.Distance(rectTransform.anchoredPosition, pos) > arrivalThreshold)
        {
            destination = pos;
            inMotion = true;
        }
    }

    public void Wait()
    {
        inMotion = false;
    }

    void Update()
    {
        if (!inMotion) return;

        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            destination,
            Time.deltaTime * speed
        );

        if (Vector2.Distance(rectTransform.anchoredPosition, destination) < arrivalThreshold)
        {
            rectTransform.anchoredPosition = destination;
            inMotion = false;
            if (OnArrival(this))
                Destroy(this);
        }
    }
}
