using System.Collections.Generic;
using UnityEngine;

public class DotFromImage : MonoBehaviour
{
    public Texture2D sourceImage;
    public GameObject dotPrefab;
    public float spacing = 5f; // pixel step

    void Start()
    {
        GenerateDots();
    }

    void GenerateDots()
    {
        List<Vector2> borderPoints = new List<Vector2>();

        for (int y = 1; y < sourceImage.height - 1; y += (int)spacing)
        {
            for (int x = 1; x < sourceImage.width - 1; x += (int)spacing)
            {
                Color c = sourceImage.GetPixel(x, y);
                if (c.a > 0.5f && c.grayscale < 0.5f) // dark pixel
                {
                    // Check if neighbor is lighter = edge
                    if (IsEdge(x, y))
                    {
                        Vector2 worldPos = new Vector2(
                            (float)x / sourceImage.width * 10f,   // scale to world
                            (float)y / sourceImage.height * 10f
                        );
                        Instantiate(dotPrefab, worldPos, Quaternion.identity, transform);
                    }
                }
            }
        }
    }

    bool IsEdge(int x, int y)
    {
        Color c = sourceImage.GetPixel(x, y);
        foreach (var off in new Vector2Int[] {
            new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(0,1), new Vector2Int(0,-1)})
        {
            Color n = sourceImage.GetPixel(x + off.x, y + off.y);
            if (n.grayscale > 0.5f) return true;
        }
        return false;
    }
}
