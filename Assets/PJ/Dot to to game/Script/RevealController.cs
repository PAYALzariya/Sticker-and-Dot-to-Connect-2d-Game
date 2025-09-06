// RevealController.cs
using UnityEngine;

public class RevealController : MonoBehaviour
{
    // Simple approach: fade in a colored artwork sprite overlay
    public SpriteRenderer revealSprite;
    public float fadeDuration = 0.8f;

    public void Reveal()
    {
        if (revealSprite == null) return;
        revealSprite.gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }

    System.Collections.IEnumerator FadeIn()
    {
        float t = 0f;
        Color c = revealSprite.color;
        c.a = 0f;
        revealSprite.color = c;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(t / fadeDuration);
            revealSprite.color = c;
            yield return null;
        }
    }
}
