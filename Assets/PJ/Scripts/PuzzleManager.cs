using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PuzzleManager : MonoBehaviour
{
    
    public static PuzzleManager instance;
    private GameObject currentLevel;
    public GameObject dragObjectPrefab;
    public Transform levelParentTra,dragObjectParent;
    public int CurrentLevelCount, fillCount;
    private int fillCountTemp=1;
  
    public List<GameObject> levelFillImage, levelemptyImage; 

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            CurrentLevelCount = PlayerPrefs.GetInt("CurrentLevel");
        }
        else
        {
            CurrentLevelCount = 1;
            PlayerPrefs.SetInt("CurrentLevel", CurrentLevelCount);
        }
        LoadLevel();
    }
    /// <summary>
    /// Loads a prefab level from Resources/Prefabs/ by name.
    /// </summary>
    public void LoadLevel()
    {

        // Load prefab from Resources/Prefabs/{levelName}
        if (CurrentLevelCount == 4)
        {
            CurrentLevelCount = 1;
            PlayerPrefs.SetInt("CurrentLevel", CurrentLevelCount);
        }
        GameObject prefab = Resources.Load<GameObject>($"Level {CurrentLevelCount}");
        if (prefab != null)
        {
            // Destroy old level if it exists
            if (currentLevel != null)
            {
                Destroy(currentLevel);
            }

            // Instantiate new level under parent, keeping prefab's original size
            currentLevel = Instantiate(prefab, levelParentTra, false); 
            // Clear list first
            levelemptyImage.Clear();
            // Loop through children
            foreach (Transform child in currentLevel.transform.GetChild(1))
            {
                levelemptyImage.Add(child.gameObject);
            }
            // Clear list first
            levelFillImage.Clear();
            // Loop through children
            foreach (Transform child in currentLevel.transform.GetChild(2))
            {
                levelFillImage.Add(child.gameObject);//Add(child.GetComponent<Image>().sprite);
            }
            foreach (Transform child in dragObjectParent)
            {
                Destroy(child);
            }
            fillCountTemp=1;
            fillCount = 0;
            for (int i = 0; i < 3; i++)
            {
                GameObject dragobject = Instantiate(dragObjectPrefab, dragObjectParent,false);
                dragobject.GetComponent<Image>().sprite = levelemptyImage[i].GetComponent<Image>().sprite;
                dragobject.transform.GetChild(0).GetComponent<Image>().sprite = levelFillImage[i].GetComponent<Image>().sprite;
              //  dragobject.transform.GetChild(0).GetComponent<Image>()
                dragobject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (fillCountTemp.ToString());
                fillCountTemp++;
            }


            Debug.Log($"Loaded level prefab: " + currentLevel.name);
        }
        else
        {
            Debug.LogError($"Level {CurrentLevelCount} not found in Resources/Prefabs/");
        }
    }
    public void LoadNewDragObject(int counterFill)
    {
        if (fillCountTemp - 1 < levelFillImage.Count)
        {
            Debug.Log("callnew");
            GameObject dragobject = Instantiate(dragObjectPrefab, dragObjectParent, false);
            dragobject.GetComponent<Image>().sprite = levelemptyImage[fillCountTemp - 1].GetComponent<Image>().sprite;
            dragobject.transform.GetChild(0).GetComponent<Image>().sprite = levelFillImage[fillCountTemp - 1].GetComponent<Image>().sprite;
            //  dragobject.transform.GetChild(0).GetComponent<Image>()
            dragobject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (fillCountTemp.ToString());
            fillCountTemp++;
        }
        if (fillCount <= levelFillImage.Count)
        {
            DestroyMatchingParents((counterFill).ToString());
            levelFillImage[counterFill - 1].gameObject.SetActive(true);
            fillCount++;
            if (fillCount == levelFillImage.Count || fillCount >= levelFillImage.Count)
            {
                Debug.Log("levelFillImage finish");
                StartCoroutine(loadnewLEvel());
            }
        }
    }
    IEnumerator loadnewLEvel()
    {
        yield return new WaitForSeconds(1f);
        CurrentLevelCount += 1;
        PlayerPrefs.SetInt("CurrentLevel", CurrentLevelCount);
        LoadLevel();
    }
    public void DestroyMatchingParents(string targetValue)
    {
        // Loop through all direct children of dragObjectParent
        foreach (Transform child in dragObjectParent)
        {
            TextMeshProUGUI tmp = child.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null && tmp.text == targetValue)
            {
                Debug.Log($"Destroying {child.name} under {dragObjectParent.name} (TMP text == {targetValue})");
                Destroy(child.gameObject);
                return; // remove this return if you want to destroy ALL matches
            }
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
