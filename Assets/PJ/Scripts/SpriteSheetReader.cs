using UnityEngine;

public class SpriteSheetReader : MonoBehaviour
{
    public Sprite[] spriteSheet; // Assign sliced sprites here

    void Start()
    {
        for (int i = 0; i < spriteSheet.Length; i++)
        {
            Sprite sprite = spriteSheet[i];
            Rect r = sprite.rect;

            Debug.Log($"Sprite {sprite.name} - Position: X = {r.x}, Y = {r.y}, Width = {r.width}, Height = {r.height}");
        }
    }
}
