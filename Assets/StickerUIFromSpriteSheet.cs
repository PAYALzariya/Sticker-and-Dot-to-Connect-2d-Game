using UnityEngine;
using UnityEngine.UI;

public class StickerUIFromSpriteSheet : MonoBehaviour
{
    public Sprite[] spriteSheet;             // Sliced sticker sprites
    public RectTransform backgroundImage;    // The UI Image showing the full sheet
    public GameObject stickerUIPrefab;       // Your UI prefab (Image with RectTransform)
    public float sheetWidth = 1024f;         // Width of the original sprite sheet image
    public float sheetHeight = 1024f;        // Height of the original sprite sheet image

    void Start()
    {
        foreach (var sprite in spriteSheet)
        {
            // === 1. Calculate Center Position ===
            float centerX = sprite.rect.x + sprite.rect.width / 2f;
            float centerY = sprite.rect.y + sprite.rect.height / 2f;

            float normX = centerX / sheetWidth;
            float normY = centerY / sheetHeight;

            float anchoredX = (normX - 0.5f) * backgroundImage.rect.width;
            float anchoredY = (normY - 0.5f) * backgroundImage.rect.height;
            Vector2 anchoredPos = new Vector2(anchoredX, anchoredY);

            // === 2. Calculate Size ===
            float sizeX = (sprite.rect.width / sheetWidth) * backgroundImage.rect.width;
            float sizeY = (sprite.rect.height / sheetHeight) * backgroundImage.rect.height;
            Vector2 size = new Vector2(sizeX, sizeY);

            // === 3. Instantiate and Set ===
            GameObject sticker = Instantiate(stickerUIPrefab, backgroundImage);
            RectTransform rt = sticker.GetComponent<RectTransform>();
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;  // Match size
            rt.localPosition = new Vector3(rt.localPosition.x, rt.localPosition.y, 0); // Flatten Z
            sticker.GetComponent<Image>().sprite = sprite;
            sticker.name = "Sticker_" + sprite.name;
        }
    }
}
