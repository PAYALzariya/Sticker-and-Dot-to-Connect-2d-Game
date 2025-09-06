// Dot.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class Dot : MonoBehaviour
{
    public int index; // 1-based
    public SpriteRenderer spriteRenderer;
    public TextMeshProUGUI numberText;
    public bool connected = false;

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
      //  numberText = GetComponentInChildren<TextMeshPro>();
    }

    public void Init(int idx, Vector2 pos, float scale = 1f)
    {
        index = idx;
        transform.localPosition = pos;
        transform.localScale = Vector3.one * scale;
        connected = false;
        if (numberText != null) numberText.text = idx.ToString();
        spriteRenderer.color = Color.white; // initial tint
    }

    public void SetConnectedVisual()
    {
        connected = true;
        // small scale animation
        transform.localScale = Vector3.one * 0.9f;
        // tint to indicate connected
        spriteRenderer.color = new Color(0.7f, 0.7f, 0.7f);
    }
}
