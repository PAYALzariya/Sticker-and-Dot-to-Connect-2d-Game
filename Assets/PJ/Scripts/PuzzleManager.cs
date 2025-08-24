using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    
    public static PuzzleManager instance;
    private GameObject currentLevel;
    public Transform levelPerentTra;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
       
    }
    /// <summary>
    /// Loads a prefab level from Resources/Prefabs/ by name.
    /// </summary>
    public void LoadLevel(string levelName)
    {
        // Load prefab from Resources/Prefabs/{levelName}
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{levelName}");
        if (prefab != null)
        {
            // Destroy old level if it exists
            if (currentLevel != null)
            {
                Destroy(currentLevel);
            }

            // Instantiate new level
            currentLevel = Instantiate(prefab);
            currentLevel.transform.parent = levelPerentTra;

            currentLevel.name = levelName; // cleaner hierarchy
            Debug.Log($"Loaded level prefab: {levelName}");
        }
        else
        {
            Debug.LogError($"Level prefab '{levelName}' not found in Resources/Prefabs!");
        }
    }

    /// <summary>
    /// Unloads the current level prefab instance.
    /// </summary>
    public void UnloadLevel()
    {
        if (currentLevel != null)
        {
            Destroy(currentLevel);
            currentLevel = null;
            Debug.Log("Unloaded current level.");
        }
    }
}
/*  /* old  public PuzzleLevelData levelData;
    public GameObject piecePrefab;
    public RectTransform backgroundHolder;
    public RectTransform pieceHolder;
    public Image backgroundImage;
    public CanvasGroup canvasGroup;
    void Start()
{
    /* // Set background
     backgroundImage.sprite = levelData.backgroundImage;

     // Instantiate pieces
     foreach (var pieceData in levelData.pieces)
     {
         GameObject pieceObj = Instantiate(piecePrefab, pieceHolder);
         pieceObj.GetComponent<Image>().sprite = pieceData.pieceImage;
         pieceObj.GetComponent<Image>().SetNativeSize();
         PuzzlePiece pp = pieceObj.GetComponent<PuzzlePiece>();
         //  pp.canvas = GetComponentInParent<Canvas>();
         Debug.Log("Current position " + pieceData.correctPosition);
         pp.Init(pieceData.pieceImage, pieceData.correctPosition);

         // Randomize start position
         *//*pieceObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(
             Random.Range(-300, 300), Random.Range(-500, 500)
         );*//*
     }*/
